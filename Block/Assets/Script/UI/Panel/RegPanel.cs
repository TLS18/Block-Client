﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegPanel : PanelBase
{
    private InputField idInput;
    private InputField pwInput;
    private Button regBtn;
    private Button closeBtn;

    #region 生命周期
    //初始化
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RegPanel";
        layer = PanelMgr.PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("PWInput").GetComponent<InputField>();
        regBtn = skinTrans.Find("RegBtn").GetComponent<Button>();
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();

        regBtn.onClick.AddListener(OnRegClick);
        closeBtn.onClick.AddListener(OnCloseClick);
    }
    #endregion

    public void OnCloseClick()
    {
        PanelMgr.instance.OpenPanel<LoginPanel>("");
        Close();
    }

    public void OnRegClick()
    {
        if(idInput.text == "" || pwInput.text == "")
        {
            Debug.Log("用户名密码不能为空!");
            return;
        }

        if (NetMgr.srvConn.status != Connection.Status.Connected)
        {
            Debug.Log("网络出问题了");
            return;
        }
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Register");
        protocol.AddString(idInput.text);
        protocol.AddString(pwInput.text);
        Debug.Log("发送" + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, OnRegBack);
    }

    public void OnRegBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if (ret == 0)
        {
            Debug.Log("注册成功!");
            PanelMgr.instance.OpenPanel<LoginPanel>("");
            Close();
        }
        else
        {
            Debug.Log("注册失败!");
        }
    }
}
