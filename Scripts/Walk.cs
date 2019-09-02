using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Text;


public class Walk : MonoBehaviour
{
    Socket socket;
    const int BUFFER_SIZE = 1024;
    public byte[] readBuff = new byte[BUFFER_SIZE];
    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    List<string> msgList = new List<string>();
    public GameObject prefeb;
    string id;

    void AddPlayer(string id,Vector3 pos)
    {
        GameObject player = Instantiate(prefeb, pos, Quaternion.identity);
        TextMesh textMesh = player.GetComponentInChildren<TextMesh>();
        textMesh.text = id;
        players.Add(id, player);
    }

    void SendPos()
    {
        GameObject player = players[id];
        Vector3 pos = player.transform.position;
        string str = "POS " + id + " " + pos.x.ToString() + " " + pos.y.ToString() + " " + pos.z.ToString() + " ";
        byte[] bytes = Encoding.Default.GetBytes(str);
        socket.Send(bytes);
        Debug.Log("发送: " + str);
    }
    
    void SendLeave()
    {
        string str = "LEAVE " + id + " ";
        byte[] bytes = Encoding.Default.GetBytes(str);
        socket.Send(bytes);
        Debug.Log("发送: " + str);
    }

    void Move()
    {
        if(id == "")
        {
            return;
        }
        GameObject player = players[id];
        if (Input.GetKey(KeyCode.UpArrow))
        {
            player.transform.Translate(Vector3.forward * 5 * Time.deltaTime);
            SendPos();
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            player.transform.Translate(Vector3.back * 5 * Time.deltaTime);
            SendPos();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            player.transform.Translate(Vector3.left * 5 * Time.deltaTime);
            SendPos();
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            player.transform.Translate(Vector3.right * 5 * Time.deltaTime);
            SendPos();
        }
    }

    private void OnDestroy()
    {
        SendLeave();
    }

    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect("127.0.0.1", 1234);
        id = socket.LocalEndPoint.ToString();
        socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);

        int x = UnityEngine.Random.Range(-10, 10);
        int z = UnityEngine.Random.Range(-10, 10);

        Vector3 pos = new Vector3(x, 0, z);
        AddPlayer(id, pos);
        SendPos();
    }

    void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            string str = Encoding.UTF8.GetString(readBuff, 0, count);
            msgList.Add(str);
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            socket.Close();
        }
    }

    void Update()
    {
        for (int i = 0; i < msgList.Count; i++)
        {
            string str = msgList[0];
            msgList.RemoveAt(0);
            string[] args = str.Split(' ');
            if (args[0] == "POS")
            {
                OnRecvPos(args[1], args[2], args[3], args[4]);
            }
            else if (args[0] == "LEAVE")
            {
                OnRecvLeave(args[1]);
            }
        }
        Move();
    }

    public void OnRecvPos(string id,string _x,string _y,string _z)
    {
        if(id == this.id)
        {
            return;
        }
        float x = float.Parse(_x);
        float y = float.Parse(_y);
        float z = float.Parse(_z);
        Vector3 pos = new Vector3(x, y, z);
        if (players.ContainsKey(id))
        {
            players[id].transform.position = pos;
        }
        else
        {
            AddPlayer(id, pos);
        }
    }

    public void OnRecvLeave(string id)
    {
        if (players.ContainsKey(id))
        {
            Destroy(players[id]);
            players.Remove(id);
        }
    }
}
