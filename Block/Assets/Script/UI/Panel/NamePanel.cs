using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamePanel : PanelBase
{
    private InputField nameInput;
    private Button confirmBtn;
    private Button closeBtn;

    #region 生命周期
    //初始化
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "NamePanel";
        layer = PanelMgr.PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        nameInput = skinTrans.Find("NameInput").GetComponent<InputField>();
        confirmBtn = skinTrans.Find("ConfirmBtn").GetComponent<Button>();
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        confirmBtn.onClick.AddListener(OnConfirmClick);
        closeBtn.onClick.AddListener(OnCloseClick);
    }
    #endregion

    public void OnConfirmClick()
    {
        if(nameInput.text == "")
        {
            Debug.Log("姓名不能为空!");
            return;
        }
        if (NetMgr.srvConn.status != Connection.Status.Connected)
        {
            Debug.Log("网络出问题了");
            return;
        }
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("EditName");
        protocol.AddString(nameInput.text);
        Debug.Log("发送" + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, OnConfirmBack);
    }

    public void OnConfirmBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if(ret == 0)
        {
            Debug.Log("修改名字成功!");
            PlayerMgr.instance.UpdateName(nameInput.text);
            EditNameBtn.instance.Enable();
            Chat.instance.Enable();
            Close();
        }
        else
        {
            Debug.Log("修改名字失败!");
        }
    }

    public void OnCloseClick()
    {
        EditNameBtn.instance.Enable();
        Chat.instance.Enable();
        Close();
    }
}
