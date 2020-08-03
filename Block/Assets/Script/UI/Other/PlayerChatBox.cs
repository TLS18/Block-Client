using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChatBox : MonoBehaviour
{
    CanvasGroup canvasGroup;
    Text text;
    public float textTime; //聊天气泡显示时间
    public bool isEnable;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        text = transform.Find("Text").GetComponent<Text>();
        textTime = 2f;
        Disable();
        isEnable = false;
    }

    void Update()
    {
        if (isEnable)
        {
            textTime -= Time.deltaTime;
        }
        if (textTime <= 0)
        {
            Disable();
            textTime = 2f;
        }
    }

    public void SetText(string recvText)
    {
        text.text = recvText;
        textTime = 2f;
        Enable();
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

}
