using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : PanelBase
{
    private Button closeBtn;
    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "InfoPanel";
        layer = PanelMgr.PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        closeBtn.onClick.AddListener(OnCloseClick);
    }
    #endregion

    public void OnCloseClick()
    {
        Close();
    }
}
