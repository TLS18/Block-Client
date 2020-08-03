using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMgr : MonoBehaviour
{
    //预设
    public GameObject prefab;
    //出生点
    public GameObject spawnPoint;
    //管理玩家
    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    List<string> keys = new List<string>();
    //这个客户端的玩家id
    public string playerID = "";
    //单例
    public static PlayerMgr instance;
    //更改动画机
    public RuntimeAnimatorController playerCtrl;
    public RuntimeAnimatorController netCtrl;

    void Start()
    {
        instance = this;
    }

    //添加玩家
    void AddPlayer(string id,Vector3 pos,string name)
    {
        Debug.Log("添加玩家");
        GameObject player = (GameObject)Instantiate(prefab, pos, Quaternion.identity);
        if(id == playerID)
        {
            CinemachineVirtualCamera vcm = player.transform.Find("vcam").GetComponent<CinemachineVirtualCamera>();
            vcm.enabled = true;
            player.GetComponent<Animator>().runtimeAnimatorController = playerCtrl;
            player.GetComponent<Character>().ctrlType = CtrlType.player;
        }
        else
        {
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
        Text text = player.transform.Find("Canvas").transform.Find("Name").GetComponent<Text>();
        text.text = name;
        players.Add(id, player);
    }

    void DelPlayer(string id)
    {
        //玩家已经初始化
        if (players.ContainsKey(id))
        {
            Destroy(players[id]);
            players.Remove(id);
        }
    }

    public void DelAllPlayer()
    {
        foreach(KeyValuePair<string,GameObject> player in players)
        {
            keys.Add(player.Key);
        }
        for(int i =0; i < keys.Count; i++) 
        {     
            DelPlayer(keys[i]);
        }
    }

    //更新名字
    public void UpdateName(string name)
    {
        UpdateName(playerID, name);
    }
    public void  UpdateName(string id,string name)
    {
        GameObject player = players[id];
        if (player == null) 
        {
            return;
        }
        Text text = player.transform.Find("Canvas").transform.Find("Name").GetComponent<Text>();
        text.text = name;
        //SendPos();
    }

    public void UpdateInfo(string id,Vector3 pos,float xScale,int animInfo,string name)
    {
        //只更新名字，不更新位置，优化操作手感
        if(id == playerID)
        {
            UpdateName(id, name);
            return;
        }
        //其他玩家
        if (players.ContainsKey(id))
        {
            players[id].GetComponent<Character>().NetCtrl(pos, xScale, animInfo);
            UpdateName(id, name);
        }
        else
        {
            AddPlayer(id, pos, name);
        }

    }

    public void StartGame(string id)
    {
        playerID = id;
        Vector3 pos = spawnPoint.transform.position;
        AddPlayer(playerID, pos, "加载名字中");
        //同步
        SendInfo();
        //获取列表
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("GetList");
        NetMgr.srvConn.Send(proto, GetList);
        NetMgr.srvConn.msgDist.AddListener("UpdateInfo", UpdateInfo);
        NetMgr.srvConn.msgDist.AddListener("PlayerLeave", PlayerLeave);
        NetMgr.srvConn.msgDist.AddListener("PlayerChat", RecvMsg);
        EditNameBtn.instance.Enable();
        Chat.instance.Enable();
    }

    //发送位置
    public void SendInfo()
    {
        GameObject player = players[playerID];
        Vector3 pos = player.transform.position;
        //消息
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("UpdateInfo");
        proto.AddFloat(pos.x);
        proto.AddFloat(pos.y);
        proto.AddFloat(pos.z);
        proto.AddFloat(player.transform.localScale.x); //角色朝向
        proto.AddInt(player.GetComponent<Character>().animInfo); //动画状态
        NetMgr.srvConn.Send(proto);
    }

    //更新列表
    public void GetList(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int count = proto.GetInt(start, ref start);
        for(int i = 0; i < count; i++)
        {
            string id = proto.GetString(start, ref start);
            float x = proto.GetFloat(start, ref start);
            float y = proto.GetFloat(start, ref start);
            float z = proto.GetFloat(start, ref start);
            float xScale = proto.GetFloat(start, ref start);
            int animInfo = proto.GetInt(start, ref start);
            string name = proto.GetString(start, ref start);
            Vector3 pos = new Vector3(x, y, z);
            UpdateInfo(id, pos, xScale, animInfo, name);
        }
    }

    public void UpdateInfo(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        float x = proto.GetFloat(start, ref start);
        float y = proto.GetFloat(start, ref start);
        float z = proto.GetFloat(start, ref start);
        float xScale = proto.GetFloat(start, ref start);
        int animInfo = proto.GetInt(start, ref start);
        string name = proto.GetString(start, ref start);
        Vector3 pos = new Vector3(x, y, z);
        UpdateInfo(id, pos, xScale, animInfo, name);
    }

    public void RecvMsg(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        Debug.Log(proto.GetDesc());
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        string msg = proto.GetString(start, ref start);
        GameObject player = players[id];
        string name = player.transform.Find("Canvas").transform.Find("Name").GetComponent<Text>().text;
        PlayerChatBox chatBox = player.transform.Find("Canvas").transform.Find("Chat").GetComponent<PlayerChatBox>();
        if (chatBox != null)
        {
            chatBox.SetText(msg);
        }
        Chat.instance.GetMessage(name, msg);
    }

    public void PlayerLeave(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        //获取数值
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        DelPlayer(id);
    }

    public enum CtrlType
    {
        player,
        net,
    }
}
