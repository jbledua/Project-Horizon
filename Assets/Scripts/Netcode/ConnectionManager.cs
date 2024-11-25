using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Scenes;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{

    [SerializeField] private string _listenIp = "127.0.0.1";
    [SerializeField] private string _connectIp = "127.0.0.1";
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
        // Determine Roles
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
        StartCoroutine(Connect());
    }

    private IEnumerator Connect()
    {
        // Create Server World
        if (_role == Role.ServerClient || _role == Role.Server)
        {
            serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
        }

        // Create Client World
        if (_role == Role.ServerClient || _role == Role.Client)
        {
            clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
        }

        // Remove Default World
        foreach (var world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }

        // Setting the Default Injection World
        if (serverWorld != null)
        {
            World.DefaultGameObjectInjectionWorld = serverWorld;
        }
        else if (clientWorld != null)
        {
            World.DefaultGameObjectInjectionWorld = clientWorld;
        }

        SubScene[] subScenes = FindObjectsByType<SubScene>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        // Connting the worlds together
        if (serverWorld != null)
        {
            // Wait for world to be created
            while (!serverWorld.IsCreated)
            {
                yield return null;

            }

            if (subScenes != null)
            {
                for (int i = 0; i < subScenes.Length; i++)
                {
                    SceneSystem.LoadParameters loadParameters = new SceneSystem.LoadParameters() { Flags = SceneLoadFlags.BlockOnStreamIn };
                    var sceneEntity = SceneSystem.LoadSceneAsync(serverWorld.Unmanaged, new Unity.Entities.Hash128(subScenes[i].SceneGUID.Value), loadParameters);

                    while (!SceneSystem.IsSceneLoaded(serverWorld.Unmanaged, sceneEntity))
                    {
                        serverWorld.Update();
                    }
                }


            }

            using var query = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(NetworkEndpoint.Parse(_listenIp, _port));
        }

        if (clientWorld != null)
        {
            // Wait for clientWorld to be created
            while (!clientWorld.IsCreated)
            {
                yield return null;
            }

            if (subScenes != null)
            {
                for (int i = 0; i < subScenes.Length; i++)
                {
                    SceneSystem.LoadParameters loadParameters = new SceneSystem.LoadParameters() { Flags = SceneLoadFlags.BlockOnStreamIn };
                    var sceneEntity = SceneSystem.LoadSceneAsync(clientWorld.Unmanaged, new Unity.Entities.Hash128(subScenes[i].SceneGUID.Value), loadParameters);

                    while (!SceneSystem.IsSceneLoaded(clientWorld.Unmanaged, sceneEntity))
                    {
                        clientWorld.Update();
                    }
                }


            }


            using var query = clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, NetworkEndpoint.Parse(_connectIp, _port));
        }
    }

}