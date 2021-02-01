﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System;
using System.Linq;

public class ByteArray
{
    public byte[] bytes;
    public int readIdx = 0;
    public int writeIdx = 0;
    public int length { get { return writeIdx - readIdx; } }

    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        readIdx = 0;
        writeIdx = defaultBytes.Length;
    }
}
public static class NetManager
{
    private const int BUFF_SIZE = 1024;

    private static Socket socket;

    private static byte[] readBuff = new byte[BUFF_SIZE];

    private static int buffCount = 0;

    public delegate void MsgListener(string str);

    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();

    private static List<string> msgList = new List<string>();   // receive 

    private static Queue<ByteArray> writeQueue = new Queue<ByteArray>();    // send queue

    public static void AddListener(string msgName, MsgListener listener)
    {
        listeners[msgName] = listener;
    }

    public static string GetDesc()
    {
        if (socket == null) return "";
        if (!socket.Connected) return "";
        return socket.LocalEndPoint.ToString();
    }

    public static void Connect(string ip, int port)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(ip, port);
        socket.BeginReceive(readBuff, buffCount, BUFF_SIZE - buffCount, SocketFlags.None, ReceiveCallback, socket);
    }

    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            buffCount += count;

            OnReceiveData();
            socket.BeginReceive(readBuff, buffCount, BUFF_SIZE - buffCount, SocketFlags.None, ReceiveCallback, socket);
        }catch(SocketException ex)
        {
            Debug.LogError("socket receive fail " + ex.ToString());
        }
    }

    private static void OnReceiveData()
    {
        if(buffCount <= 2) { return; }
        Int16 bodyLength = BitConverter.ToInt16(readBuff, 0);
        if(buffCount < 2 + bodyLength) { return; }
        int end = 2 + bodyLength;
        string recvStr = System.Text.Encoding.Default.GetString(readBuff, 2, end);
        msgList.Add(recvStr);
        int count = buffCount - end;
        Array.Copy(readBuff, end, readBuff, 0, count);
        buffCount -= end;
        OnReceiveData();
    }
    public static void Send(string sendStr)
    {
        if (socket == null) return;
        if (!socket.Connected) return;

        byte[] bodyByte = System.Text.Encoding.Default.GetBytes(sendStr);
        Int16 len = (Int16)bodyByte.Length;
        byte[] headByte = BitConverter.GetBytes(len);
        byte[] sendByte = headByte.Concat(bodyByte).ToArray();
        ByteArray ba = new ByteArray(sendByte);
        int count = 0;
        lock (writeQueue)
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }
        if (count == 1)
        {
            socket.BeginSend(sendByte, 0, sendByte.Length, SocketFlags.None, SendCallback, socket);
        }
        //socket.Send(sendByte);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        Socket socket = (Socket)ar.AsyncState;
        int count = socket.EndReceive(ar);

        ByteArray ba;
        lock (writeQueue)
        {
            ba = writeQueue.First();
        }
        ba.readIdx += count;
        if(count == ba.length)
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                ba = writeQueue.First();
            }
        }
        if(ba != null)
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, SocketFlags.None, SendCallback, socket);
        }
    }

    public static void Update()
    {
        if (msgList.Count <= 0) return;
        string msgStr = msgList[0];
        msgList.RemoveAt(0);
        string[] split = msgStr.Split('|');
        string msgName = split[0];
        string msgArgs = split[1];

        if (listeners.ContainsKey(msgName))
        {
            listeners[msgName](msgArgs);
        }
    }
}
