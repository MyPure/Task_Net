using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace Net
{
    class Serv
    {
        public Socket listenfd;
        public Conn[] conns;
        public int maxConn = 50;
        MySqlConnection sqlConn;

        public int NewIndex()
        {
            if(conns == null)
            {
                return -1;
            }
            for(int i = 0; i < conns.Length; i++)
            {
                if(conns[i] == null)
                {
                    conns[i] = new Conn();
                    return i;
                }
                else if(conns[i].isUse == false)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Start(string host,int port)
        {
            //数据库
            //string connStr = "Database=msgboard;Data Source=127.0.0.1;";
            //connStr += "User Id=root;Password=Aa000309;port=3306";
            //sqlConn = new MySqlConnection(connStr);
            //try
            //{
            //    sqlConn.Open();
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine("数据库连接失败: "+e.Message);
            //    return;
            //}

            conns = new Conn[maxConn];
            for(int i = 0; i < maxConn; i++)
            {
                conns[i] = new Conn();
            }
            //Socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //BInd
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 1234);
            listenfd.Bind(ipEp);
            //Listen
            listenfd.Listen(maxConn);
            //Accept
            listenfd.BeginAccept(AcceptCb, null);
            Console.WriteLine("[服务器]启动成功");
        }

        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                Socket socket = listenfd.EndAccept(ar);
                int index = NewIndex();

                if(index < 0)
                {
                    socket.Close();
                    Console.WriteLine("连接已满");
                }
                else
                {
                    Conn conn = conns[index];
                    conn.Init(socket);
                    string adr = conn.GetAdress();
                    Console.WriteLine("客户端连接 [" + adr + "] conn池ID: "+ index);
                    conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);
                    listenfd.BeginAccept(AcceptCb, null);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("AcceptCb失败:" + e.Message);
            }
        }

        private void ReceiveCb(IAsyncResult ar)
        {
            Conn conn = (Conn)ar.AsyncState;
            try
            {
                int count = conn.socket.EndReceive(ar);
                if (count <= 0)
                {
                    Console.WriteLine("收到 [" + conn.GetAdress() + "] 断开连接");
                    conn.Close();
                    return;
                }
                string str = Encoding.UTF8.GetString(conn.readBuff, 0, count);
                Console.WriteLine("收到 ["+conn.GetAdress()+"] 数据:" + str);
                

                HandleMsg(conn, str);

                conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);
            }
            catch(Exception e)
            {
                Console.WriteLine("["+ conn.GetAdress()+"] 断开连接");
                conn.Close();
            }
        }

        public void HandleMsg(Conn conn,string str)
        {
            //if(str == "_GET")
            //{
            //    string cmdStr = "select * from msg order by id desc limit 10;";
            //    MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            //    try
            //    {
            //        MySqlDataReader dataReader = cmd.ExecuteReader();
            //        str = "";
            //        while (dataReader.Read())
            //        {
            //            str += dataReader["name"] + ":" + dataReader["msg"] + "\n\r";
            //        }
            //        dataReader.Close();
            //        byte[] bytes = Encoding.Default.GetBytes(str);
            //        conn.socket.Send(bytes);
            //    }
            //    catch(Exception e)
            //    {
            //        Console.WriteLine("数据库查询失败: " + e.Message);
            //    }
            //}
            //else
            //{
            //    string cmdStrFormat = "insert into msg set name = '{0}' ,msg = '{1}';";
            //    string cmdStr = string.Format(cmdStrFormat, conn.GetAdress(), str);
            //    MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            //    try
            //    {
            //        cmd.ExecuteNonQuery();
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("数据库插入失败: " + e.Message);
            //    }
            //}
            byte[] bytes = Encoding.Default.GetBytes(str);
            for(int i = 0; i < conns.Length; i++)
            {
                if(conns[i]==null||conns[i].isUse == false)
                {
                    continue;
                }
                Console.WriteLine("转播消息给 " + conns[i].GetAdress());
                conns[i].socket.Send(bytes);
            }
        }
    }
}
