using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace Net
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            //Socket
            Socket listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 1234);
            listenfd.Bind(ipEp);
            //Listen
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功");
            while (true)
            {
                //Accept
                Socket connfd = listenfd.Accept();
                Console.WriteLine("[服务器]Accept");
                //Recv
                byte[] readBuff = new byte[1024];
                int count = connfd.Receive(readBuff);
                string str = Encoding.UTF8.GetString(readBuff, 0, count);
                Console.WriteLine("[服务器接收] " + str);
                //Send
                byte[] bytes = Encoding.Default.GetBytes("serv echo " + str);
                connfd.Send(bytes);
            }
            */

            Serv serv = new Serv();
            serv.Start("127.0.0.1", 1234);
            while (true)
            {
                string str = Console.ReadLine();
                switch (str)
                {
                    case "quit":
                        return;
                }
            }
        }
    }
}
