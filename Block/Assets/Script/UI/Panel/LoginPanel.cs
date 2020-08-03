using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : PanelBase
{
    private InputField idInput;
    private InputField pwInput;
    private Button loginBtn;
    private Button regBtn;

    #region 生命周期
    //初始化
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "LoginPanel";
        layer = PanelMgr.PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("PWInput").GetComponent<InputField>();
        loginBtn = skinTrans.Find("LoginBtn").GetComponent<Button>();
        regBtn = skinTrans.Find("RegBtn").GetComponent<Button>();

        loginBtn.onClick.AddListener(OnLoginClick);
        regBtn.onClick.AddListener(OnRegClick);
    }
    #endregion

    public void OnRegClick()
    {
        PanelMgr.instance.OpenPanel<RegPanel>("");
        Close();
    }

    public void OnLoginClick()
    {
        if(idInput.text == "" || pwInput.text == "")
        {
            Debug.Log("用户名密码不能为空!");
            return;
        }
        if(NetMgr.srvConn.status != Connection.Status.Connected)
        {
            Debug.Log("网络出问题了");
            return;
        }
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Login");
        protocol.AddString(idInput.text);
        protocol.AddString(pwInput.text);
        Debug.Log("发送" + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, OnLoginBack);
    }

    public void OnLoginBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        switch (ret)
        {
            case 0:
                {
                    Debug.Log("登录成功!");
                    PlayerMgr.instance.StartGame(idInput.text);
                    Close();
                    break;
                }
            case -1:
                {
                    Debug.Log("密码错误!");
                    break;
                }
            case -2:
                {
                    Debug.Log("该id已在线!");
                    break;
                }
            case -3:
                {
                    Debug.Log("无角色数据!");
                    PanelMgr.instance.OpenPanel<CreatePanel>("",idInput.text);
                    Close();
                    break;
                }
        }
    }
}
