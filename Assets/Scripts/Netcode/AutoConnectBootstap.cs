using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.NetCode;

public class AutoConnectBootstap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        return false;
    }
}
