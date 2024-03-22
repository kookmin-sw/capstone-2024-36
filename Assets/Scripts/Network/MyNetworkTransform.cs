using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
// using UnityEditor;

public class MyNetworkTransform : NetworkBehaviour
{
    // key: NetworkObjectId
    private static Dictionary<ulong, MyNetworkTransform> s_spawnedMap = new Dictionary<ulong, MyNetworkTransform>();

    // key: RegisterId
    private static Dictionary<int, MyNetworkTransform> s_registeredMap = new Dictionary<int, MyNetworkTransform>();

    public static MyNetworkTransform GetSpawnedByNetworkId(ulong networkObjectId)
    {
        if (s_spawnedMap.ContainsKey(networkObjectId))
            return s_spawnedMap[networkObjectId];

        return null;
    }

    public static IEnumerator<MyNetworkTransform> GetAllTransforms()
    {
        return s_spawnedMap.Values.GetEnumerator();
    }

    public static bool Register(int registerId, MyNetworkTransform tr)
    {
        if (s_registeredMap.ContainsKey(registerId))
        { 
            if (s_registeredMap[registerId] != tr)
            {
                Debug.LogError("duplicated MyNetworkTransform found!");
                return false;
            }

            return true;
        }
        
        if (tr.IsPlacedByDesigner)
        {
            Debug.LogError("try to register designer placed MyNetworkTransform!");
            return false;
        }

        s_registeredMap[registerId] = tr;

        return true;
    }

    public static MyNetworkTransform GetRegistered(int registerId)
    {
        if (s_registeredMap.ContainsKey(registerId))
            return s_registeredMap[registerId];

        return null;
    }

    [Header("Status")]
    public bool IsPlacedByDesigner = false;
    public int CurrentSceneIndex;   // SceneLoader에서 관리해주는 값

    // SceneLoader에서 관리해주는 값
    public ushort SceneSequenceNumber = 0;

    // 내부적으로 관리해야 하는 값
    [SerializeField] private bool isReadyForPlaced = false;
    [SerializeField] private bool isPlaced = false;

    [Header("Setting")]
    public int RegisterId = -1;
    [SerializeField] float smoothDampTime = 0.1f;
    [SerializeField] int sendPerSec = 10;
    public bool bKeepSendWhenNotExist;

    [Header("Component")]
    [SerializeField] Transform m_existanceTransform;

    [Header("Prefab")]
    [SerializeField] GameObject m_networkPrefapInProject;

    public UnityAction<bool> SetExistEvent;  // true: visible, collidable
    public UnityAction SetSpawnPositionEvent;    // set spawn position on current scene

    private NetworkVariable<NetworkTransformData> transformData = new NetworkVariable<NetworkTransformData>(
        NetworkTransformData.Zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    );

    private Vector3 positionVelocity;
    private Vector3 scaleVelocity;
    // private Vector3 rotationVelocity;
    private Quaternion deriv;

    private float deltaLastSend;

    private void Awake()
    {
        FixedOnUnplace();
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        if (
            IsPlacedByDesigner == true &&
            (m_networkPrefapInProject == this || m_networkPrefapInProject == null)
        )
        {
            Debug.Log("NetworkPrefapInProject MUST NO be this, but network prefab in PROJECT");
        }

        if (IsPlacedByDesigner)
        {
            if (s_registeredMap.ContainsKey(RegisterId))
            {
                DestroyImmediate(gameObject);
                return;
            }

            if (!IsSpawned)
            {
                bool canSpawnSelf = NetworkManager.IsServer || NetworkManager.IsHost;

                if (canSpawnSelf)
                {
                    GameObject go = Instantiate(m_networkPrefapInProject);

                    MyNetworkTransform created = go.GetComponent<MyNetworkTransform>();
                    created.RegisterId = RegisterId;
                    created.IsPlacedByDesigner = false;
                    created.transform.position = transform.position;
                    created.bKeepSendWhenNotExist = bKeepSendWhenNotExist;

                    go.GetComponent<NetworkObject>().Spawn();
                }
                else
                {
                    // 스스로 Spawn() 할 수 없으니 RPC로 서버에 요청하여 Spawn한다. 
                    // 요청할 때 위치 같이 넘기고 
                    // 서버는 Ownership도 돌려줘야 한다. 

                    NetworkTransformData data = new NetworkTransformData(this.transform);

                    NetworkSceneManager.Instance.SpawnIfNotSpawnedRpc(
                        RegisterId,
                        SceneManager.GetActiveScene().buildIndex,
                        data,
                        NetworkManager.Singleton.LocalClientId
                    );

                    // Debug.Log("Not Implemented: client side spawning");
                }
            }

            DestroyImmediate(gameObject);
        }
        else
        {
            // OK   
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        s_spawnedMap[NetworkObjectId] = this;

        Scene scene = SceneManager.GetActiveScene();

        if (scene.buildIndex == this.CurrentSceneIndex)
        {
            SetExistEvent?.Invoke(true);
            FixedOnPlace();
        }
        else
        {
            SetExistEvent?.Invoke(false);
            FixedOnUnplace();
        }

        if (IsOwner)
        {
            CurrentSceneIndex = scene.buildIndex;
            SetExistEvent?.Invoke(true);
            FixedOnPlace();

            // 플레이어는 NetworkSceneManager보다 먼저 생성된다. 
            // 이 게임오브젝트가 플레어이 아닌 경우 항상 NetworkSceneManager는
            // 등록돼있어야 한다!
            if (!NetworkSceneManager.IsRegistered())
            {
                Debug.Log($"MyNetworkTransform::Start() NetworkSceneManager is not registered. GameObjectName: {this.name}");
                return;
            }

            NetworkSceneManager.Instance.NotifyReadyForPlacedRpc(
                NetworkObjectId, SceneSequenceNumber, scene.buildIndex, NetworkSceneManager.ReadyForPlacedType.MoveScene);
        }

        if (RegisterId != -1)
        {
            Register(RegisterId, this);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        // clean up.
        if (s_spawnedMap.ContainsKey(NetworkObjectId))
        {
            s_spawnedMap.Remove(NetworkObjectId);
        }
        else
        {
            Debug.Log("trying to remove transform that not exist. ");
        }

        if (RegisterId != -1)
        {
            s_registeredMap.Remove(RegisterId);
        }
    }

    public void FixedOnPlace()
    {
        if (m_existanceTransform != null)
        {
            m_existanceTransform.gameObject.SetActive(true);
        }

        if (IsOwner)
        {
            NetworkTransformData data = new NetworkTransformData(
                transform.position, transform.localScale, transform.rotation.eulerAngles
            );
            data.SetSceneSequenceNumber(SceneSequenceNumber);

            transformData.Value = data;

            isPlaced = true;
        }
        else
        {
            isPlaced = false;
        }

        isReadyForPlaced = true;
    }

    public void FixedOnUnplace()
    {
        if (m_existanceTransform != null)
        {
            m_existanceTransform.gameObject.SetActive(false);
        }

        if (IsOwner)
        {
            transformData.Value.SetSceneSequenceNumber(SceneSequenceNumber);
            transformData.SetDirty(true);
        }

        isReadyForPlaced = false;
        isPlaced = false;
    }

    void Update()
    {
        if (IsOwner)    // send, write
        {
            deltaLastSend += Time.deltaTime;

            if (!isPlaced && !bKeepSendWhenNotExist)
                return;

            if (deltaLastSend >= 1.0f / sendPerSec)
            {
                NetworkTransformData newData = new NetworkTransformData(
                    transform.position, transform.localScale, transform.rotation.eulerAngles
                );
                newData.SetSceneSequenceNumber(SceneSequenceNumber);

                transformData.Value = newData;

                deltaLastSend = 0.0f;
            }
        }
        else    // receive, read
        {
            NetworkTransformData value = transformData.Value;

            // 동기화 불필요한 경우 확인
            bool shloud_ignore = 
                (value.SceneSequenceNumber != SceneSequenceNumber) ||               // SceneSequenceNumber 불일치
                (SceneManager.GetActiveScene().buildIndex != CurrentSceneIndex) ||     // 다른 룸에 있는 놈임
                (CurrentSceneIndex == ((int)NetworkSceneManager.BuildIndex.TitleScene));          // 타이틀 화면에 있음

            if (shloud_ignore)
            {
                // Debug.Log($"{value.SceneSequenceNumber} {SceneSequenceNumber} {GetOwnerTransform().CurrentSceneIndex} {CurrentSceneIndex} {CurrentSceneIndex} {NetworkSceneLoader.TitleSceneIndex}");
                return;
            }
                
            if (isPlaced)   // lerp
            {
                transform.position = Vector3.SmoothDamp(transform.position, transformData.Value.Position, ref positionVelocity, smoothDampTime);
                transform.localScale = Vector3.SmoothDamp(transform.localScale, transformData.Value.Scale, ref scaleVelocity, smoothDampTime);
                // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transformData.Value.Rotation), smoothDampTime);
                transform.rotation = SmoothDampQuaternion2(transform.rotation, Quaternion.Euler(transformData.Value.Rotation), ref deriv, smoothDampTime);
            }
            else if (isReadyForPlaced)  // place
            {
                transform.position = transformData.Value.Position;
                transform.localScale = transformData.Value.Scale;
                transform.rotation = Quaternion.Euler(transformData.Value.Rotation);

                isPlaced = true;

                SetExistEvent?.Invoke(true);
                if (m_existanceTransform != null)
                {
                    m_existanceTransform.gameObject.SetActive(true);
                }
            }
        }
    }   /* end of Update */

    // REF: https://forum.unity.com/threads/quaternion-smoothdamp.793533/
    public static Quaternion SmoothDampQuaternion1(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
    {
        Vector3 c = current.eulerAngles;
        Vector3 t = target.eulerAngles;
        return Quaternion.Euler(
          Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
          Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
          Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
        );
    }

    // REF: https://gist.github.com/maxattack/4c7b4de00f5c1b95a33b
    public static Quaternion SmoothDampQuaternion2(Quaternion rot, Quaternion target, ref Quaternion deriv, float time)
    {
        if (Time.deltaTime < Mathf.Epsilon) return rot;
        // account for double-cover
        var Dot = Quaternion.Dot(rot, target);
        var Multi = Dot > 0f ? 1f : -1f;
        target.x *= Multi;
        target.y *= Multi;
        target.z *= Multi;
        target.w *= Multi;
        // smooth damp (nlerp approx)
        var Result = new Vector4(
            Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
            Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
            Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
            Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
        ).normalized;

        // ensure deriv is tangent
        var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
        deriv.x -= derivError.x;
        deriv.y -= derivError.y;
        deriv.z -= derivError.z;
        deriv.w -= derivError.w;

        return new Quaternion(Result.x, Result.y, Result.z, Result.w);
    }
}
