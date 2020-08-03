using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePanel : PanelBase
{
    private InputField nameInput;
    private Button confirmBtn;

    #region 生命周期
    //初始化
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "CreatePanel";
        layer = PanelMgr.PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        nameInput = skinTrans.Find("NameInput").GetComponent<InputField>();
        confirmBtn = skinTrans.Find("ConfirmBtn").GetComponent<Button>();
        confirmBtn.onClick.AddListener(OnCreateClick);
    }
    #endregion

    public void OnCreateClick()
    {
        if (nameInput.text == "")
        {
            Debug.Log("姓名不能为空");
            return;
        }
        if (NetMgr.srvConn.status != Connection.Status.Connected)
        {
            string host = "127.0.0.1";
            int port = 1234;
            NetMgr.srvConn.proto = new ProtocolBytes();
            NetMgr.srvConn.Connect(host, port);
        }
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("CreatePlayer");
        protocol.AddString(nameInput.text);
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        Debug.Log(protoName);
        Debug.Log("start" + start);
        Debug.Log(sizeof(Int32));
        string test = protocol.GetString(start, ref start);
        Debug.Log("发送" + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, OnCreateBack);
    }

    public void OnCreateBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if(ret == 0)
        {
            Debug.Log("创建角色成功!");
            Close();
            Debug.Log((string)args[0]);
            PlayerMgr.instance.StartGame((string)args[0]);
        }
        else
        {
            Debug.Log("创建角色失败");
        }

    }
}
