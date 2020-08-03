﻿using System;
using System.Collections.Generic;
using System.Text;

public class ProtocolBase
{
    public virtual ProtocolBase Decode(byte[] readbuff, int start, int length)
    {
        return new ProtocolBase();
    }
    public virtual byte[] Encode()
    {
        return new byte[] { };
    }

    public virtual string GetName()
    {
        return "";
    }

    public virtual string GetDesc()
    {
        return "";
    }
}