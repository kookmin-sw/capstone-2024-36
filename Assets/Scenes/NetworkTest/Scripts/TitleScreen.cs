using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;

// TODO: 예외 처리 및 버튼 이벤트로 구현된 것 코드로 정리하기
// TODO: infText 세분화
public class TitleScreen : MonoBehaviour
{
    [Header("Status")]
    public bool isStarted = false;
    public bool isLocalPlayerFound = false;
    public bool isNetworkSceneLoaderFound = false;
    // public bool isHost;

    [Header("Reference")]
    public TMPro.TMP_Text InfoText;

    public TMPro.TMP_InputField IpInputField;
    public TMPro.TMP_InputField PortInputField;

    public Button StartAsHostButton;
    public Button StartAsGuestButton;

    [Header("Prefab")]
    public GameObject NetworkSceneManagerPrefap;

    private UnityTransport unityTransport;

    private void UnityTransport_OnTransportEvent(NetworkEvent eventType, ulong clientId, System.ArraySegment<byte> payload, float receiveTime)
    {
        switch (eventType)
        {
            case NetworkEvent.Disconnect:
                InfoText.text = $"unityTransport.OnTransportEvent: {eventType}";
                Debug.Log($"OnTransportEvent {eventType}");
                break;
            case NetworkEvent.Data:
                break;
            default:
                InfoText.text = $"unityTransport.OnTransportEvent: {eventType}";
                Debug.Log($"OnTransportEvent {eventType}");
                break;
        }

        StartAsHostButton.gameObject.SetActive(true);
        StartAsGuestButton.gameObject.SetActive(true);
    }

    private static bool s_isCallbackAdded = false;

    public void Start()
    {
        Debug.Log("TitleScreen start");
        unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        unityTransport.OnTransportEvent += UnityTransport_OnTransportEvent;
        if (!s_isCallbackAdded)
        {
            
            s_isCallbackAdded = true;
        }
    }

    public void OnDestroy()
    {
        unityTransport.OnTransportEvent -= UnityTransport_OnTransportEvent;
    }

    public void StartAsHost()
    {
        unityTransport.ConnectionData.Address = IpInputField.text;
        unityTransport.ConnectionData.Port = ushort.Parse(PortInputField.text);

        NetworkManager.Singleton.StartHost();

        if (!NetworkSceneManager.IsRegistered())
        {
            GameObject myNetworkTransformManagerGo = Instantiate(NetworkSceneManagerPrefap);
            NetworkObject myNetworkTransformManagerNetGo = myNetworkTransformManagerGo.GetComponent<NetworkObject>();
            myNetworkTransformManagerNetGo.Spawn();
        }

        isStarted = true;
    }

    public void StartAsGuest()
    {
        unityTransport.ConnectionData.Address = IpInputField.text;
        unityTransport.ConnectionData.Port = ushort.Parse(PortInputField.text);

        bool bRet = NetworkManager.Singleton.StartClient();
        if (!bRet)
        {
            Debug.LogError("StartClient() failed");
            return;
        }
        else
        {
            StartAsGuestButton.gameObject.SetActive(false);
        }

        isStarted = true;
        // isHost = false;

        InfoText.text = "Loading Network Object... ";
    }

    private void Update()
    {
        if (isStarted)
        {
            // network spawn 될 때까지 기다린다. 
            if (!NetworkSceneManager.IsRegistered())
            {
                isNetworkSceneLoaderFound = true;
                return;
            }

            // network spawn 될 때까지 기다린다. 
            if (NetworkPlayer.LocalIstance == null)
            {
                return;
            }
            isLocalPlayerFound = true;

            InfoText.text = "Loading Scene... ";

            MyNetworkTransform playerTransform = NetworkPlayer.LocalIstance.GetComponent<MyNetworkTransform>();
            NetworkSceneManager.Instance.MoveScene(
                (int)NetworkSceneManager.BuildIndex.WorldScene1, 
                playerTransform
                );

            // call it only once
            isStarted = false;
        }
    }
}
