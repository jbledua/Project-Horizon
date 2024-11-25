using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{

    [SerializeField] private string _ip = "127.0.0.1";
    [SerializeField] private ushort _port = 7979;

    public static World serverWorld = null;
    public static World clientWorld = null;

    public enum Role
    {
        ServerClient = 0, Server = 1, Client = 2
    }

    private static Role _role = Role.ServerClient;

    private void Start()
    {
        if (Application.isEditor)
        {
            _role = Role.ServerClient;
        }
        else if (Application.platform == RuntimePlatform.WindowsServer || Application.platform == RuntimePlatform.LinuxServer || Application.platform == RuntimePlatform.OSXServer)
        {
            _role = Role.Server;
        }
        else
        {
            _role = Role.Client;
        }
        Connect();
    }

    private void Connect()
    {
        if (_role == Role.ServerClient || _role == Role.Server)
        {
            serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
        }

        if (_role == Role.ServerClient || _role == Role.Client)
        {
            clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
        }

        foreach (var world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }

        if (serverWorld != null)
        {
            World.DefaultGameObjectInjectionWorld = serverWorld;
        }
        else if (clientWorld != null)
        {
            World.DefaultGameObjectInjectionWorld = clientWorld;
        }

        if (serverWorld != null)
        {
            using var query = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(ClientServerBootstrap.DefaultListenAddress.WithPort(_port));
        }

        if (clientWorld != null)
        {
            IPAddress serverAddress = IPAddress.Parse(_ip);
            NativeArray<byte> nativeArrayAddress = new NativeArray<byte>(serverAddress.GetAddressBytes().Length, Allocator.Temp);
            nativeArrayAddress.CopyFrom(serverAddress.GetAddressBytes());
            NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4;
            endpoint.SetRawAddressBytes(nativeArrayAddress);
            endpoint.Port = _port;
            using var query = clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, endpoint);
        }
    }

}