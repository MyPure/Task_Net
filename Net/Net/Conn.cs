using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections;
namespace Net
{
    class Conn
    {
        public const int BUFFER_SIZE = 1024;

        //Socket
        public Socket socket;

        //是否使用
        public bool isUse = false;
        public byte[] readBuff = new byte[BUFFER_SIZE];
        public int buffCount = 0;

        public Conn()
        {
            readBuff = new byte[BUFFER_SIZE];
        }

        public void Init(Socket _socket)
        {
            socket = _socket;
            isUse = true;
            buffCount = 0;
        }

        public int BuffRemain()
        {
            return BUFFER_SIZE - buffCount;
        }

        public string GetAdress()
        {
            if (!isUse)
            {
                return "无法获取地址";
            }
            return socket.RemoteEndPoint.ToString();
        }

        public void Close()
        {
            if (!isUse)
            {
                return;
            }
            Console.WriteLine(GetAdress() + "断开连接");
            socket.Close();
            isUse = false;
        }
    }
}
