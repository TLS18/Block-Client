using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class NetMgr
{
    public static Connection srvConn = new Connection();
    public static void Update()
    {
        srvConn.Update();
    }
    public static ProtocolBase GetHeartBeatProtocol()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("HeartBeat");
        return protocol;
    }
}
