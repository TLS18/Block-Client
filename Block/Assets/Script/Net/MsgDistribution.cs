using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class MsgDistribution  //消息分发类
{
    //每帧处理消息数量
    public int num = 15;
    //消息列表
    public List<ProtocolBase> msgList = new List<ProtocolBase>();
    //委托类型
    public delegate void Delegate(ProtocolBase proto);
    //事件监听表,once表注册一次监听一次,event表注册一次终身执行
    private Dictionary<string, Delegate> eventDict = new Dictionary<string, Delegate>();
    private Dictionary<string, Delegate> onceDict = new Dictionary<string, Delegate>();

    public void Update()
    {
        for (int i = 0; i < num; i++)
        {
            if (msgList.Count > 0)
            {
                DispatchMsgEvent(msgList[0]);
                lock (msgList) msgList.RemoveAt(0);
            }
            else
            {
                break;
            }
        }
    }

    public void DispatchMsgEvent(ProtocolBase protocol)
    {
        string name = protocol.GetName();
        Debug.Log("分发处理消息" + name);
        if (eventDict.ContainsKey(name))
        {
            eventDict[name](protocol);
        }
        if (onceDict.ContainsKey(name))
        {
            onceDict[name](protocol);
            onceDict[name] = null;
            onceDict.Remove(name);
        }
    }

    public void AddListener(string name, Delegate cb)
    {
        if (eventDict.ContainsKey(name))
        {
            eventDict[name] += cb;
        }
        else
        {
            eventDict[name] = cb;
        }
    }

    public void AddOnceListener(string name, Delegate cb)
    {
        if (onceDict.ContainsKey(name))
        {
            onceDict[name] += cb;
        }
        else
        {
            onceDict[name] = cb;
        }
    }

    public void DelListener(string name, Delegate cb)
    {
        if (eventDict.ContainsKey(name))
        {
            eventDict[name] -= cb;
            if (eventDict[name] == null)
            {
                eventDict.Remove(name);
            }
        }
    }

    public void DelOnceListener(string name, Delegate cb)
    {
        if (onceDict.ContainsKey(name))
        {
            onceDict[name] -= cb;
            if (onceDict[name] == null)
            {
                onceDict.Remove(name);
            }
        }
    }
}

