using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : PanelBase
{
    private Button startBtn;
    private Button infoBtn;
    private Image ping;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "TitlePanel";
        layer = PanelMgr.PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        startBtn = skinTrans.Find("StartBtn").GetComponent<Button>();
        infoBtn = skinTrans.Find("InfoBtn").GetComponent<Button>();
        ping = skinTrans.Find("Ping").GetComponent<Image>();
        startBtn.onClick.AddListener(OnStartClick);
        infoBtn.onClick.AddListener(OnInfoClick);
    }
    #endregion

    public void OnStartClick()
    {
        if(NetMgr.srvConn.status == Connection.Status.Connected)
        {
            PanelMgr.instance.OpenPanel<LoginPanel>("");
            Close();
        }
        else
        {
            Debug.Log("网络连接失败");
        }
    }

    public void OnInfoClick()
    {
        PanelMgr.instance.OpenPanel<InfoPanel>("");
    }

    public override void Update()
    {
        if(NetMgr.srvConn.status != Connection.Status.Connected)
        {
            ping.sprite = PanelMgr.instance.none;
            ping.color = Color.red;
        }
        else
        {
            ping.sprite = PanelMgr.instance.connected;
            ping.color = Color.green;
        }
    }
}
