using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetRoot : MonoBehaviour
{

    System.Timers.Timer timer = new System.Timers.Timer(1000);
    void Start()
    {
        string host = "127.0.0.1";
        int port = 1234;
        NetMgr.srvConn.proto = new ProtocolBytes();
        NetMgr.srvConn.Connect(host, port);
        timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
        timer.AutoReset = false; //只执行一次
        timer.Enabled = true;
    }
    void Update()
    {
        NetMgr.Update();
    }

    void HandleMainTimer(object sender,System.Timers.ElapsedEventArgs e)
    {
        if (NetMgr.srvConn.status != Connection.Status.Connected)
        {
            Debug.Log("尝试重新连接中");
            string host = "127.0.0.1";
            int port = 1234;
            NetMgr.srvConn.proto = new ProtocolBytes();
            NetMgr.srvConn.Connect(host, port);
        }
        timer.Start();
    }


}
