using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
public class Net : MonoBehaviour
{
    Socket socket;
    public InputField hostInput;
    public InputField portInput;
    public InputField sendInput;
    public Button connect;
    public Text recvText;
    public Text clientText;
    public string recv = "";
    const int BUFFER_SIZE = 1024;
    byte[] readBuff = new byte[BUFFER_SIZE];
    bool connecting;
    private void Start()
    {
        
    }
    private void Update()
    {
        recvText.text = recv;
    }

    public void Connection()
    {
        if (connecting)
        {
            socket.Close();
            connect.GetComponentInChildren<Text>().text = "连接";
            connecting = false;
        }
        else
        {
            //Socket 
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            //Connect
            string host = hostInput.text;
            int port = int.Parse(portInput.text);
            socket.Connect(host, port);
            connecting = true;
            connect.GetComponentInChildren<Text>().text = "断开";
            clientText.text = "客户端地址: " + socket.LocalEndPoint.ToString();
            //Recv
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }    
    }

    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            string str = Encoding.UTF8.GetString(readBuff, 0, count);
            if (recv.Length > 300) recv = "";
            recv += str + "\n";
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }
        catch(Exception e)
        {
            recvText.text += "连接已断开";
            socket.Close();
        }
    }

    public void Send()
    {
        string str = sendInput.text;
        byte[] bytes = Encoding.Default.GetBytes(str);
        try
        {
            socket.Send(bytes);
        }
        catch
        {

        }
    }
}
