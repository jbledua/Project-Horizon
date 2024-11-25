using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.NetCode;
using UnityEngine.Scripting;

[Preserve]
public class AutoConnectBootstap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        AutoConnectPort = 0;
        return false;
    }
}
