using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System;
using System.Linq;

public class ByteArray
{
    const int DEFAULT_SIZE = 1024;
    int initSize = 0;
    private int capacity = 0;
    public byte[] bytes;
    public int readIdx = 0;
    public int writeIdx = 0;
    public int length { get { return writeIdx - readIdx; } }
    public int remain { get { return capacity - writeIdx; } }
    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        capacity = defaultBytes.Length;
        initSize = defaultBytes.Length;
        readIdx = 0;
        writeIdx = defaultBytes.Length;
    }
    public ByteArray(int size = DEFAULT_SIZE)
    {
        bytes = new byte[size];
        capacity = size;
        initSize = size;
        readIdx = 0;
        writeIdx = 0;
    }

    public void Resize(int size)
    {
        if (size < length) return;
        if (size < initSize) return;
        int n = 1;
        while (n < size) n *= 2;
        capacity = n;
        byte[] newBytes = new byte[capacity];
        Array.Copy(bytes, readIdx, newBytes, 0, writeIdx - readIdx);
        bytes = newBytes;
        writeIdx = length;
        readIdx = 0;
    }

    public void CheckAndMoveBytes()
    {
        if(length < 8)
        {
            MoveBytes();
        }
    }

    public void MoveBytes()
    {
        Array.Copy(bytes, readIdx, bytes, 0, length);
        writeIdx = length;
        readIdx = 0;
    }

    public int Write(byte[] bs, int offset, int count)
    {
        if(remain < count)
        {
            Resize(length + count);
        }
        Array.Copy(bs, offset, bytes, writeIdx, count);
        writeIdx += count;
        return count;
    }

    //public int Read(byte[] bs, int offset, int count)
    //{
    //    count = Math.Min(count, length);
    //    Array.Copy(bytes, 0, bs, offset, count);
    //    readIdx += count;
    //    CheckAndMoveBytes();
    //    return count;
    //}

    public Int16 ReadInt16()
    {
        if (length < 2) return 0;
        Int16 ret = (Int16)((bytes[1] << 8) | bytes[0]);
        readIdx += 2;
        CheckAndMoveBytes();
        return ret;
    }

    public Int32 ReadInt32()
    {
        if (length < 4) return 0;
        Int16 ret = (Int16)((bytes[3] << 24) | 
                                (bytes[2] << 16) |
                                (bytes[1] << 8) |
                                bytes[0]);
        readIdx += 4;
        CheckAndMoveBytes();
        return ret;
    }
}
public static class NetManager
{
    private static Socket socket;

    private static ByteArray readBuff = new ByteArray();

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
        socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, SocketFlags.None, ReceiveCallback, socket);
    }

    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            readBuff.writeIdx += count;

            OnReceiveData();
            if(readBuff.remain < 8)
            {
                readBuff.MoveBytes();
                readBuff.Resize(readBuff.length * 2);
            }
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, SocketFlags.None, ReceiveCallback, socket);
        }
        catch(SocketException ex)
        {
            Debug.LogError("socket receive fail " + ex.ToString());
        }
    }

    private static void OnReceiveData()
    {
        if(readBuff.length <= 2) { return; }
        Int16 bodyLength = BitConverter.ToInt16(readBuff.bytes, 0);
        if (readBuff.length < 2 + bodyLength) { return; }
        readBuff.readIdx += 2;

        string recvStr = System.Text.Encoding.Default.GetString(readBuff.bytes, readBuff.readIdx, bodyLength);
        readBuff.readIdx += bodyLength;
        readBuff.CheckAndMoveBytes();

        msgList.Add(recvStr);
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
