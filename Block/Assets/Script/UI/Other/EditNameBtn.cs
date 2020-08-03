using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditNameBtn : MonoBehaviour
{
    public static EditNameBtn instance;
    public bool isEnable; //暴露可见状态给角色控制脚本,true表示按钮可见
    private Button panelBtn;
    private CanvasGroup canvasGroup;
    void Awake()
    {
        instance = this;
        panelBtn = transform.GetComponent<Button>();
        canvasGroup = transform.GetComponent<CanvasGroup>();
        if (panelBtn != null)
        {
            panelBtn.onClick.AddListener(OnBtnClick);
        }
        Disable();
        isEnable = false;
    }

    public void Disable()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        isEnable = false;
    }

    public void Enable()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        isEnable = true;
    }

    private void OnBtnClick()
    {
        PanelMgr.instance.OpenPanel<NamePanel>("");
        Disable();
        Chat.instance.Disable();
    }

}
