using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class NetManager : MonoBehaviour
{
    Socket socket;
    const int BUFFER_SIZE = 1024;
    public byte[] readBuff = new byte[BUFFER_SIZE];
    //玩家列表
    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    //消息列表
    List<string> msgList = new List<string>();
    //Player预设
    public GameObject prefab;
    //本机的ID和端口
    string id;

    //添加玩家
    void AddPlayer(string id,Vector3 pos)
    {
        GameObject player = (GameObject)Instantiate(prefab, pos, Quaternion.identity);
        Text name = player.transform.Find("Name").GetComponent<Text>();
        name.text = id;
        players.Add(id, player);
    }

    void SendPos()
    {
        GameObject player = players[id];
        Vector3 pos = player.transform.position;
        //组装传输协议
        string str = "POS ";
        str += id + " ";
        str += pos.x.ToString() + " ";
        str += pos.y.ToString() + " ";
        str += pos.z.ToString() + " ";

        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        socket.Send(bytes);
        Debug.Log("发送 " + str);
    }

    void SendLeave()
    {
        string str = "LEAVE ";
        str += id + " ";
        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        socket.Send(bytes);
        Debug.Log("发送 " + str);
    }

    void OnDestory()
    {
        SendLeave();
    }

    void Start()
    {
        Connect();

    }

    void Connect()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect("127.0.0.1", 1234);
        id = socket.LocalEndPoint.ToString();
        //Recv
        socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
    }

    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            msgList.Add(str);
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }
        catch(Exception e)
        {

        }
    }

    void Update()
    {
        for(int i = 0; i < msgList.Count; i++)
        {
            HandleMsg();
        }
    }

    void HandleMsg()
    {
        if (msgList.Count <= 0) return;
        string str = msgList[0];
        msgList.RemoveAt(0);
        //根据协议做出不同的消息处理
        string[] args = str.Split(' ');
        if (args[0] == "POS")
        {
            OnRecvPos(args[1], args[2], args[3], args[4]);
        }
        else if (args[0]　== "LEAVE")
        {
            OnRecvLeave(args[1]);
        }
    }

    public void OnRecvPos(string id,string xStr,string yStr,string zStr)
    {
        if (id == this.id) return;
        float x = float.Parse(xStr);
        float y = float.Parse(yStr);
        float z = float.Parse(zStr);
        Vector3 pos = new Vector3(x, y, z);
        //玩家已经初始化
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
            players[id] = null;
        }
    }
}
