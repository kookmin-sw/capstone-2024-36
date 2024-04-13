using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Collections.Generic;

public class NetworkSceneManager : NetworkSingletoneComponent<NetworkSceneManager>
{
    #region static
    private static NetworkSpawnInfoMap s_spawnInfoMap = new NetworkSpawnInfoMap();

    public static void SetSpawnInfo(ulong networkObjectId, SpawnInfo info)
    {
        s_spawnInfoMap[networkObjectId] = info;
    }

    public static void SetSpawnInfo(ulong networkObjectId, ushort sceneSequenceNumber, int currentSceneIndex, int registerId)
    {
        s_spawnInfoMap[networkObjectId] = new SpawnInfo(sceneSequenceNumber, currentSceneIndex, registerId);
    }

    public static void RemoveSpawnInfo(ulong networkObjectId)
    {
        s_spawnInfoMap.Remove(networkObjectId);
    }

    public static void SetSpawnInfo(MyNetworkTransform netTr)
    {
        s_spawnInfoMap[netTr.NetworkObjectId] = new SpawnInfo(netTr);
    }

    public static SpawnInfo GetSpawnInfo(ulong networkObjectId)
    {
        if (s_spawnInfoMap.ContainsKey(networkObjectId))
            return s_spawnInfoMap[networkObjectId];

        return null;
    }

    public static IEnumerator<MyNetworkTransform> GetSpawnedEnumerator()
    {
        return s_spawnInfoMap.GetSpawnedIterator();
    }

    private static Dictionary<int, MyNetworkTransform> s_registeredMap = new Dictionary<int, MyNetworkTransform>();
    
    public static void Register(int registerId, MyNetworkTransform netTr)
    {
        if (registerId == -1)
            return;

        s_registeredMap[registerId] = netTr;
    }

    public static void Unregister(int registerId)
    {
        if (s_registeredMap.ContainsKey(registerId))
            s_registeredMap.Remove(registerId);
    }
    public static MyNetworkTransform GetRegistered(int registerId)
    {
        if (s_registeredMap.ContainsKey(registerId))
            return s_registeredMap[registerId];

        return null;
    }
    #endregion

    [Header("Setting")]
    public string OnDisconnectionSceneName = "";

    [Header("Reference")]
    public List<NetworkRegisterList> RegisterLists = new List<NetworkRegisterList>();

    private static UnityTransport s_unityTransport;
    public static UnityTransport GetUnityTransport() { return s_unityTransport; }

    private Dictionary<int, GameObject> m_registeredPrefab = new Dictionary<int, GameObject>();

    private void Awake()
    {
        if (RegisterLists == null)
            Debug.LogError("FATAL: RegisterLists is NULL");
        else
        {
            foreach(var list in RegisterLists)
            {
                if (list == null)
                {
                    Debug.LogError("list is NULL!");
                    continue;
                }

                foreach(var item in list.List)
                {
                    if (item.Prefab == null)
                    {
                        Debug.LogError("Prefab is NULL!");
                        continue;
                    }

                    if (m_registeredPrefab.ContainsKey(item.RegisterId))
                    {
                        Debug.LogError("Duplicated register id found!");
                        continue;
                    }
                    m_registeredPrefab.Add(item.RegisterId, item.Prefab);
                }
            }
        }

        s_unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        s_unityTransport.OnTransportEvent += UnityTransport_OnTransportEvent;

        // 호스트 서버가 꺼졌을 때는 호출 안됨.. 
        // NetworkManager.Singleton.OnServerStopped += Singleton_OnServerStopped;

        // 호스트 서버 꺼졌을 때 클라이언트에서 호출되는 콜백
        NetworkManager.Singleton.OnClientStopped += Singleton_OnClientStopped;
    }

    private void UnityTransport_OnTransportEvent(NetworkEvent eventType, ulong clientId, System.ArraySegment<byte> payload, float receiveTime)
    {
        switch (eventType)
        {
            case NetworkEvent.Disconnect:
                Debug.Log($"UnityTransport_OnTransportEvent {eventType} {clientId}");

                if (clientId == NetworkManager.Singleton.LocalClientId)
                {
                    // catch disconnect here
                    SceneManager.LoadScene(OnDisconnectionSceneName);
                }

                break;
        }
    }

    private void Singleton_OnClientStopped(bool obj)
    {
        Debug.Log($"Singleton_OnClientStopped");

        // 이때는 이거 쓰지 말것
        // LoadNextScene(OnDisconnectionSceneIndex);

        // 삭제 안하면 여러 번 호출됨
        s_unityTransport.OnTransportEvent -= UnityTransport_OnTransportEvent;
        NetworkManager.Singleton.OnClientStopped -= Singleton_OnClientStopped;

        SceneManager.LoadScene(OnDisconnectionSceneName);
    }

    public void LoadScene(int newSceneIndex)
    {
        if (SceneManager.GetActiveScene().buildIndex == newSceneIndex)
        {
            Debug.Log($"LoadNextScene: already in same scene: {newSceneIndex}");
            return;
        }

        // 모든 MyNetworkTransform이 안보이게 된다. 
        IEnumerator < MyNetworkTransform> iterator = GetSpawnedEnumerator();
        while(iterator.MoveNext())
        {
            MyNetworkTransform iTransform = iterator.Current;

            iTransform.gameObject.SendMessage("SetExist", false, SendMessageOptions.DontRequireReceiver);
            iTransform.FixedOnUnplace();
        }

        StartCoroutine(loadSceneEnumerator(newSceneIndex));
    }

    private IEnumerator loadSceneEnumerator(int newSceneIndex)
    {
        SceneManager.LoadScene(newSceneIndex);

        yield return null;

        // 내가 owner인 것들은 궂이 RPC 안 때려도 보인다. 
        IEnumerator<MyNetworkTransform> iterator = GetSpawnedEnumerator();
        while (iterator.MoveNext())
        {
            MyNetworkTransform iTransform = iterator.Current;
            
            if (iTransform == null) continue;

            if (iTransform.CurrentSceneIndex == newSceneIndex && iTransform.IsOwner)
            {
                iTransform.FixedOnPlace();
                iTransform.gameObject.SendMessage("SetExist", true, SendMessageOptions.DontRequireReceiver);
            }
        }

        // 호스트 접속 후 worldScene1에서 2로 이동.
        // 게스트 접속 후 worldScene2을 봤을 때 호스트가 worldScene2에 안보이는 문제 해결
        NotifyReadyForPlacedRpc(
            0, -1, 0, newSceneIndex, ReadyForPlacedType.LoadScene
        );
    }

    public void MoveScene(int newSceneIndex, MyNetworkTransform netTr)
    {
        if (SceneManager.GetActiveScene().buildIndex == newSceneIndex)
        {
            Debug.Log($"LoadNextScene: already in same scene: {newSceneIndex}");
            return;
        }

        netTr.SceneSequenceNumber += 1;
        netTr.CurrentSceneIndex = -1;

        // 모든 MyNetworkTransform이 안보이게 된다. 
        IEnumerator<MyNetworkTransform> iterator = GetSpawnedEnumerator();
        while (iterator.MoveNext())
        {
            MyNetworkTransform iTransform = iterator.Current;

            iTransform.gameObject.SendMessage("SetExist", false, SendMessageOptions.DontRequireReceiver);
            iTransform.FixedOnUnplace();
        }

        NotifyLeaveSceneRpc(
            netTr.NetworkObjectId, netTr.RegisterId, netTr.SceneSequenceNumber
        );

        StartCoroutine(moveSceneEnumerator(newSceneIndex, netTr));
    }

    private IEnumerator moveSceneEnumerator(int newSceneIndex, MyNetworkTransform netTr)
    {
        SceneManager.LoadScene(newSceneIndex);

        yield return null;

        IEnumerator<MyNetworkTransform> iterator = GetSpawnedEnumerator();
        while (iterator.MoveNext())
        {
            MyNetworkTransform iTransform = iterator.Current;

            if (iTransform.CurrentSceneIndex == newSceneIndex && iTransform.IsOwner)
            {
                if (iTransform == netTr)
                    continue;

                iTransform.FixedOnPlace();
                iTransform.gameObject.SendMessage("SetExist", true, SendMessageOptions.DontRequireReceiver);
            }
        }

        netTr.CurrentSceneIndex = newSceneIndex;
        netTr.SetSpawnPositionEvent.Invoke();
        netTr.gameObject.SendMessage("SetExist", true, SendMessageOptions.DontRequireReceiver);
        netTr.FixedOnPlace();

        NotifyReadyForPlacedRpc(
            netTr.NetworkObjectId,
            netTr.RegisterId,
            netTr.SceneSequenceNumber,            
            newSceneIndex, 
            ReadyForPlacedType.MoveScene
        );
    }

    #region Rpc
    [Rpc(SendTo.NotMe)]
    public void NotifyLeaveSceneRpc(ulong networkObjectId, int registerId, ushort sceneSequenceNumber)
    {
        SpawnInfo info = GetSpawnInfo(networkObjectId);
        if (info == null)
        {
            info = new SpawnInfo();
            SetSpawnInfo(networkObjectId, info);
        }

        info.SceneSequenceNumber = sceneSequenceNumber;
        info.CurrentSceneIndex = -1;
        info.RegisterId = registerId;

        MyNetworkTransform netTr = info.NetTransform;
        if (netTr == null)
        {
            Debug.LogError("neTr is NULL");
            return;
        }

        netTr.SceneSequenceNumber = sceneSequenceNumber;
        netTr.CurrentSceneIndex = -1;
        netTr.RegisterId = registerId;

        netTr.gameObject.SendMessage("SetExist", false, SendMessageOptions.DontRequireReceiver);
        netTr.FixedOnUnplace();
    }

    public enum ReadyForPlacedType
    {
        MoveScene, // request echo and set
        EchoBack,   // set
        LoadScene   // request echo
    };

    // TODO: Specified In Pram을 사용하여 Echo Back을 구현하기
    [Rpc(SendTo.NotMe)]
    public void NotifyReadyForPlacedRpc(ulong networkObjectId, int registerId, ushort sceneSequenceNumber, int newSceneIndex, ReadyForPlacedType type)
    {
        if (type != ReadyForPlacedType.LoadScene)
        {
            // set
            SpawnInfo info = GetSpawnInfo(networkObjectId);
            if (info == null)
            {
                info = new SpawnInfo();
                SetSpawnInfo(networkObjectId, info);
            }

            info.SceneSequenceNumber = sceneSequenceNumber;
            info.CurrentSceneIndex = newSceneIndex;
            info.RegisterId = registerId;

            // 게스트가 호스트가 가본 적 없는 씬에 입장한 경우
            MyNetworkTransform netTr = info.NetTransform;
            if (netTr == null) return;

            netTr.SceneSequenceNumber = sceneSequenceNumber;
            netTr.CurrentSceneIndex = newSceneIndex;
            netTr.RegisterId = registerId;

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex == newSceneIndex)
            {
                netTr.gameObject.SendMessage("SetExist", true, SendMessageOptions.RequireReceiver);
                netTr.FixedOnPlace();
            }
        }

        if (type != ReadyForPlacedType.EchoBack)
        {
            // echo
            IEnumerator<MyNetworkTransform> iterator = GetSpawnedEnumerator();
            while (iterator.MoveNext())
            {
                MyNetworkTransform iTransform = iterator.Current;

                if (iTransform.IsOwner && iTransform.CurrentSceneIndex == newSceneIndex)
                {
                    NotifyReadyForPlacedRpc(
                        iTransform.NetworkObjectId,
                        iTransform.RegisterId, 
                        iTransform.SceneSequenceNumber, 
                        iTransform.CurrentSceneIndex, 
                        ReadyForPlacedType.EchoBack
                    );
                }
            }
        }

    } /* end of Rpc() */

    [Rpc(SendTo.Server)]
    public void SpawnIfNotSpawnedRpc(int registerId, int sceneIndex, NetworkTransformData transformData, ulong clientId)
    {
        if (GetRegistered(registerId) != null)
        {
            Debug.Log($"alreay regisered: {registerId}");
            return;
        }

        if (!m_registeredPrefab.ContainsKey(registerId))
        {
            Debug.LogError("registeredPrefab not found");
            return;
        }

        MyNetworkTransform prefabTr = m_registeredPrefab[registerId].GetComponent<MyNetworkTransform>();

        GameObject go = Instantiate(m_registeredPrefab[registerId]);
        MyNetworkTransform netTr = go.GetComponent<MyNetworkTransform>();

        netTr.IsPlacedByDesigner = false;
        netTr.transform.position = transformData.Position;
        netTr.transform.rotation = Quaternion.Euler(transformData.Rotation);
        netTr.transform.localScale= transformData.Scale;

        netTr.CurrentSceneIndex = sceneIndex;
        netTr.RegisterId = registerId;
        netTr.bKeepSendWhenNotExist = prefabTr.bKeepSendWhenNotExist;
        NetworkObject netGo = go.GetComponent<NetworkObject>();
        
        // TODO: register go netTr here

        netGo.Spawn();

        // DON'T DO THIS
        // if (netTr.OwnerClientId != clientId)
        // {
        //     netGo.ChangeOwnership(clientId);
        // }
    }

    [Rpc(SendTo.Server)]
    public void RequestOwnerRpc(ulong networkObjectId, ulong clientId)
    {
        if (!s_spawnInfoMap.ContainsKey(networkObjectId))
        {
            Debug.LogError("SpawnInfo not found");
            return;
        }

        SpawnInfo spawnInfo = s_spawnInfoMap[networkObjectId];
        MyNetworkTransform netTr = spawnInfo.NetTransform;
        if (netTr == null)
        {
            Debug.LogError("NetTransform is NULL");
            return;
        }

        if (netTr.OwnerClientId != clientId)
        {
            NetworkObject netGo = netTr.GetComponent<NetworkObject>();
            netGo.ChangeOwnership(clientId);
        }
    }

    /*
    [Rpc(SendTo.Server)]
    public void DestoryNetworkObjectRpc(ulong networkObjectId, ulong clientId) 
    {
        MyNetworkTransform.RegisterInfo spawnInfo = MyNetworkTransform.GetSpawned(networkObjectId);
        if (spawnInfo == null)
        {
            Debug.LogError("SpawnInfo is NULL");
            return;
        }

        MyNetworkTransform netTr = spawnInfo.NetTransform;
        if (netTr == null)
        {
            Debug.LogError("SpawnInfo is NULL");
            return;
        }

        if (netTr.OwnerClientId == clientId)
        {
            NetworkObject netGo = netTr.GetComponent<NetworkObject>();
            netGo.Despawn();
        }
    }
    */

    #endregion
}
