using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public static Chat instance;
    public int maxMessages = 25;
    public InputField chatInput;  //暴露给玩家控制脚本来锁定角色移动
    public GameObject chatPanel, textObject;
    public bool isEnable;
    private CanvasGroup canvasGroup;
    [SerializeField] List<Message> messageList = new List<Message>();

    void Awake()
    {
        instance = this;
        chatInput = transform.Find("InputField").GetComponent<InputField>();
        canvasGroup = GetComponent<CanvasGroup>();
        Disable();
        isEnable = false;
    }

    void Update()
    {
        if (chatInput.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnMsgSend();
                chatInput.text = string.Empty;
                chatInput.ActivateInputField();
            }
            else if (!chatInput.isFocused)
            {
                chatInput.text = string.Empty;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return) && !chatInput.isFocused)
            {
                chatInput.ActivateInputField();
            }
        }
    }

    public void GetMessage(string name, string text)
    {
        if(messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }
        Message newMessage = new Message();
        newMessage.name = name;
        newMessage.text = text;
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = "["+name+"]:"+newMessage.text;
        messageList.Add(newMessage);
    }

    public void OnMsgSend()
    {
        if(chatInput.text == "")
        {
            return;
        }
        if (NetMgr.srvConn.status != Connection.Status.Connected)
        {
            Debug.Log("网络出问题了");
            return;
        }
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("SendChat");
        protocol.AddString(chatInput.text);
        Debug.Log("发送" + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, OnMsgBack);
    }

    public void OnMsgBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if(ret == 0)
        {
            Debug.Log("消息发送成功");
            chatInput.text = "";
        }
        else
        {
            Debug.Log("消息发送失败");
        }
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

[System.Serializable]
public class Message
{
    public string name;
    public string text;
    public Text textObject;
}
