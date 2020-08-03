using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBase : MonoBehaviour
{
    public string skinPath;
    public GameObject skin;
    public PanelMgr.PanelLayer layer;
    public object[] args;

    #region 生命周期
    //初始化
    public virtual void Init(params object[] args)
    {
        this.args = args;
    }
    //开始面板前
    public virtual void OnShowing() { }
    public virtual void OnShowed() { }
    public virtual void Update() { }
    public virtual void OnClosing() { }
    public virtual void OnClosed() { }
    #endregion

    #region 操作
    protected virtual void Close()
    {
        string name = this.GetType().ToString();
        PanelMgr.instance.ClosePanel(name);
    }
    #endregion
}
