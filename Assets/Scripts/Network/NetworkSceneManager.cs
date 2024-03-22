using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Collections.Generic;

public class NetworkSceneManager : NetworkSingletoneComponent<NetworkSceneManager>
{
    public enum BuildIndex : int
    {
        LoadingScene = 0,
        TitleScene = 1,
        WorldScene1 = 2,
        WorldScene2 = 3,
    }

    public static bool IsPlayableScene()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene == null)
            return false;

        BuildIndex buildIndex = (BuildIndex)scene.buildIndex;

        switch (buildIndex)
        {
            case BuildIndex.LoadingScene:
                return false;
            case BuildIndex.TitleScene:
                return false;
            case BuildIndex.WorldScene1:
                return true;
            case BuildIndex.WorldScene2:
                return true;
            default:
                Debug.LogError("not scene index");
                return false;
        }
    }

    [Header("Setting")]
    public int OnDisconnectionSceneIndex = 0;

    private static UnityTransport s_unityTransport;
    public static UnityTransport GetUnityTransport() { return s_unityTransport; }

    private void Awake()
    {
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
                    SceneManager.LoadScene(OnDisconnectionSceneIndex);
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

        SceneManager.LoadScene(OnDisconnectionSceneIndex);
    }

    public void LoadScene(int newSceneIndex)
    {
        if (SceneManager.GetActiveScene().buildIndex == newSceneIndex)
        {
            Debug.Log($"LoadNextScene: already in same scene: {newSceneIndex}");
            return;
        }

        // 모든 MyNetworkTransform이 안보이게 된다. 
        IEnumerator<MyNetworkTransform> iterator = MyNetworkTransform.GetAllTransforms();
        while(iterator.MoveNext() )
        {
            MyNetworkTransform iTransform = iterator.Current;

            iTransform.SetExistEvent?.Invoke(false);
            iTransform.FixedOnUnplace();
        }

        StartCoroutine(loadSceneEnumerator(newSceneIndex));
    }

    private IEnumerator loadSceneEnumerator(int newSceneIndex)
    {
        SceneManager.LoadScene(newSceneIndex);

        yield return null;

        // 내가 owner인 것들은 궂이 RPC 안 때려도 보인다. 
        Scene currentScene = SceneManager.GetActiveScene();
        IEnumerator<MyNetworkTransform> iterator = MyNetworkTransform.GetAllTransforms();
        while (iterator.MoveNext())
        {
            MyNetworkTransform iTransform = iterator.Current;

            if (iTransform.CurrentSceneIndex == newSceneIndex && iTransform.IsOwner)
            {
                iTransform.FixedOnPlace();
                iTransform.SetExistEvent?.Invoke(true);
            }
        }

        // 호스트 접속 후 worldScene1에서 2로 이동.
        // 게스트 접속 후 worldScene2을 봤을 때 호스트가 worldScene2에 안보이는 문제 해결
        NotifyReadyForPlacedRpc(
            0, 0, newSceneIndex, ReadyForPlacedType.LoadScene
        );
    }

    public void MoveScene(int newSceneIndex, MyNetworkTransform networkTransform)
    {
        if (SceneManager.GetActiveScene().buildIndex == newSceneIndex)
        {
            Debug.Log($"LoadNextScene: already in same scene: {newSceneIndex}");
            return;
        }

        networkTransform.SceneSequenceNumber += 1;
        networkTransform.CurrentSceneIndex = -1;

        // 모든 MyNetworkTransform이 안보이게 된다. 
        IEnumerator<MyNetworkTransform> iterator = MyNetworkTransform.GetAllTransforms();
        while (iterator.MoveNext())
        {
            MyNetworkTransform iTransform = iterator.Current;

            iTransform.SetExistEvent?.Invoke(false);
            iTransform.FixedOnUnplace();
        }

        NotifyLeaveSceneRpc(
            networkTransform.NetworkObjectId, networkTransform.SceneSequenceNumber
        );

        StartCoroutine(moveSceneEnumerator(newSceneIndex, networkTransform));
    }

    private IEnumerator moveSceneEnumerator(int newSceneIndex, MyNetworkTransform networkTransform)
    {
        SceneManager.LoadScene(newSceneIndex);

        yield return null;

        IEnumerator<MyNetworkTransform> iterator = MyNetworkTransform.GetAllTransforms();
        while (iterator.MoveNext())
        {
            MyNetworkTransform iTransform = iterator.Current;

            if (iTransform.CurrentSceneIndex == newSceneIndex && iTransform.IsOwner)
            {
                if (iTransform == networkTransform)
                    continue;

                iTransform.FixedOnPlace();
                iTransform.SetExistEvent?.Invoke(true);
            }
        }

        networkTransform.CurrentSceneIndex = newSceneIndex;
        networkTransform.SetSpawnPositionEvent?.Invoke();
        networkTransform.SetExistEvent?.Invoke(true);
        networkTransform.FixedOnPlace();

        NotifyReadyForPlacedRpc(
            networkTransform.NetworkObjectId, 
            networkTransform.SceneSequenceNumber, 
            newSceneIndex, 
            ReadyForPlacedType.MoveScene
        );
    }

    #region Rpc
    [Rpc(SendTo.NotMe)]
    public void NotifyLeaveSceneRpc(ulong networkObjectId, ushort sceneSequenceNumber)
    {
        // Debug.Log($"AlertLeaveSceneRpc received networkObjectId: {networkObjectId} sceneSequenceNumber: {sceneSequenceNumber}");

        // ulong clientId = rpcRarams.Receive.SenderClientId;
        MyNetworkTransform networkTransform = MyNetworkTransform.GetSpawnedByNetworkId(networkObjectId);

        if (networkTransform != null)
        {
            networkTransform.SceneSequenceNumber = sceneSequenceNumber;
            networkTransform.CurrentSceneIndex = -1;
            
            networkTransform.SetExistEvent?.Invoke(false);
            networkTransform.FixedOnUnplace();
        }
        else
        {
            Debug.LogError("network Transform not found");
        }
    }

    public enum ReadyForPlacedType
    {
        MoveScene, // request echo and set
        EchoBack,   // set
        LoadScene   // request echo
    };

    // TODO: Specified In Pram을 사용하여 Echo Back을 구현하기
    [Rpc(SendTo.NotMe)]
    public void NotifyReadyForPlacedRpc(ulong networkObjectId, ushort sceneSequenceNumber, int newSceneIndex, ReadyForPlacedType type)
    {
        // Debug.Log($"AlertReadyForPlacedRpc received from: {senderClientId} new: {newSceneIndex} echo: {requestEcho}");
        // ulong clientId = rpcRarams.Receive.SenderClientId;

        if (type != ReadyForPlacedType.LoadScene)
        {
            // set
            MyNetworkTransform networkTransform = MyNetworkTransform.GetSpawnedByNetworkId(networkObjectId);

            if (networkTransform != null)
            {
                networkTransform.CurrentSceneIndex = newSceneIndex;
                networkTransform.SceneSequenceNumber = sceneSequenceNumber;

                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                if (currentSceneIndex == newSceneIndex)
                {
                    networkTransform.FixedOnPlace();
                }
            }
        }

        if (type == ReadyForPlacedType.MoveScene || type == ReadyForPlacedType.LoadScene)
        {
            // echo
            IEnumerator<MyNetworkTransform> iterator = MyNetworkTransform.GetAllTransforms();
            while (iterator.MoveNext())
            {
                MyNetworkTransform iTransform = iterator.Current;
                
                // if (transform.IsOwner && transform.CurrentSceneIndex == currentSceneIndex)
                if (iTransform.IsOwner && iTransform.CurrentSceneIndex == newSceneIndex)
                {
                    NotifyReadyForPlacedRpc(
                        iTransform.NetworkObjectId, iTransform.SceneSequenceNumber, iTransform.CurrentSceneIndex, ReadyForPlacedType.EchoBack
                    );
                }
            }
        }

    } /* end of Rpc() */

    void SpawnIfNotSpawnedRpc(int registerId, int sceneIndex, int transform, int clientId)
    {

    }

    #endregion
}
