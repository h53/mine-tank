using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System;
public static class NetManager
{
    const int BUFF_SIZE = 1024;

    static Socket socket;

    static byte[] readBuff = new byte[BUFF_SIZE];

    public delegate void MsgListener(string str);

    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();

    static List<string> msgList = new List<string>();

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
        socket.BeginReceive(readBuff, 0, BUFF_SIZE, SocketFlags.None, ReceiveCallback, socket);
    }

    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            msgList.Add(recvStr);
            socket.BeginReceive(readBuff, 0, BUFF_SIZE, SocketFlags.None, ReceiveCallback, socket);
        }catch(SocketException ex)
        {
            Debug.LogError("socket receive fail" + ex.ToString());
        }
    }

    public static void Send(string sendStr)
    {
        if (socket == null) return;
        if (!socket.Connected) return;

        byte[] sendByte = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.Send(sendByte);
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
