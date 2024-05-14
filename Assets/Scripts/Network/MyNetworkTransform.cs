using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
// using UnityEditor;

public class MyNetworkTransform : NetworkBehaviour
{
    [Header("Status")]
    public bool IsPlacedByDesigner = false;
    public int CurrentSceneIndex = -1;   // SceneLoader에서 관리해주는 값

    // SceneLoader에서 관리해주는 값
    public ushort SceneSequenceNumber = 0;

    // 내부적으로 관리해야 하는 값
    [SerializeField] private bool m_isReadyForPlaced = false;
    [SerializeField] private bool m_isPlaced = false;

    [Header("Setting")]
    public int RegisterId = -1;
    [SerializeField] float smoothDampTime = 0.1f;
    [SerializeField] int sendPerSec = 10;
    public bool bKeepSendWhenNotExist;

    // public UnityAction<bool> SetExistEvent;  // true: visible, collidable
    // SendMessage("SetExist", true/false, SendMessageOptions.DontRequireReceiver); 로 수정

    // SendMessage("SetSpawnPosition", SendMessageOptions.DontRequireReceiver); 로 수정
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
        // IsPlacedByDesigner: 이 값을 여기서 초기화하면 이 gameObject가
        // 맵 디자이너에 의해 배치된 것인지, Spawn하기 위해 생성한 것인지
        // 구분을 못하기 때문에 초기화하면 안된다. 
        // IsRegistered도 마찬가지다. 

        // 무조건 다음 값으로 초기화해야함
        m_isReadyForPlaced = false;
        m_isPlaced = false;

        // 어차피 Awake할 때는 알 수 없고, 외부에서 주입받아야 함. 
        CurrentSceneIndex = -1;
        SceneSequenceNumber = 0;

        gameObject.SendMessage("SetExist", false, SendMessageOptions.DontRequireReceiver);
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        if (IsPlacedByDesigner)
        {
            if (NetworkSceneManager.GetRegistered(RegisterId))
            {
                DestroyImmediate(gameObject);
                return;
            }

            // Spawn 해야한다. 
            Scene activeScene = SceneManager.GetActiveScene();
            NetworkTransformData data = new NetworkTransformData(transform);
            ulong localClientId = NetworkManager.Singleton.LocalClientId;
            NetworkSceneManager.Instance.SpawnIfNotSpawnedRpc(
                RegisterId, 
                activeScene.buildIndex, 
                data, 
                localClientId
            );

            DestroyImmediate(gameObject);
        }
        else
        {
            // OK
        }
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"OnNetworkSpawn: {this.NetworkObjectId}");

        base.OnNetworkSpawn();

        if (RegisterId != -1)
            NetworkSceneManager.Register(RegisterId, this);

        SpawnInfo info = NetworkSceneManager.GetSpawnInfo(NetworkObjectId);
        if (info != null)
        {
            CurrentSceneIndex = info.CurrentSceneIndex;
            SceneSequenceNumber = info.SceneSequenceNumber;
            info.NetTransform = this;
        }
        NetworkSceneManager.SetSpawnInfo(this);

        Scene scene = SceneManager.GetActiveScene();

        if (CurrentSceneIndex == scene.buildIndex)
        {
            gameObject.SendMessage("SetExist", true, SendMessageOptions.DontRequireReceiver);
            FixedOnPlace();
        }

        if (IsOwner)
        {
            if (NetworkSceneManager.IsSingletoneRegistered())    // 맵에 배치되어 스폰된 경우
            {
                NetworkSceneManager.Instance.NotifyReadyForPlacedRpc(
                    NetworkObjectId, RegisterId,
                    SceneSequenceNumber, CurrentSceneIndex,
                    NetworkSceneManager.ReadyForPlacedType.EchoBack
                );
            }
            else                                                 // host 플레이어의 경우
            {
                gameObject.SendMessage("SetExist", true, SendMessageOptions.DontRequireReceiver);
                FixedOnPlace();
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        // clean up.
        NetworkSceneManager.Unregister(RegisterId);
        NetworkSceneManager.RemoveSpawnInfo(NetworkObjectId);
    }

    public void FixedOnPlace()
    {
        if (IsOwner)
        {
            NetworkTransformData data = new NetworkTransformData(
                transform.position, transform.localScale, transform.rotation.eulerAngles
            );
            data.SetSceneSequenceNumber(SceneSequenceNumber);

            transformData.Value = data;

            m_isPlaced = true;
        }
        else
        {
            m_isPlaced = false;
        }

        m_isReadyForPlaced = true;
    }

    public void FixedOnUnplace()
    {
        if (IsOwner)
        {
            transformData.Value.SetSceneSequenceNumber(SceneSequenceNumber);
            transformData.SetDirty(true);
        }

        m_isReadyForPlaced = false;
        m_isPlaced = false;
    }

    void Update()
    {
        if (IsOwner)    // send, write
        {
            deltaLastSend += Time.deltaTime;

            if (!m_isPlaced && !bKeepSendWhenNotExist)
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
                (SceneManager.GetActiveScene().buildIndex != CurrentSceneIndex);    // 다른 룸에 있는 놈임

            if (shloud_ignore)
            {
                // Debug.Log($"{value.SceneSequenceNumber} {SceneSequenceNumber} {GetOwnerTransform().CurrentSceneIndex} {CurrentSceneIndex} {CurrentSceneIndex} {NetworkSceneLoader.TitleSceneIndex}");
                return;
            }
                
            if (m_isPlaced)   // lerp
            {
                transform.position = Vector3.SmoothDamp(transform.position, transformData.Value.Position, ref positionVelocity, smoothDampTime);
                transform.localScale = Vector3.SmoothDamp(transform.localScale, transformData.Value.Scale, ref scaleVelocity, smoothDampTime);
                // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transformData.Value.Rotation), smoothDampTime);
                transform.rotation = SmoothDampQuaternion2(transform.rotation, Quaternion.Euler(transformData.Value.Rotation), ref deriv, smoothDampTime);
            }
            else if (m_isReadyForPlaced)  // place
            {
                transform.position = transformData.Value.Position;
                transform.localScale = transformData.Value.Scale;
                transform.rotation = Quaternion.Euler(transformData.Value.Rotation);

                m_isPlaced = true;

                gameObject.SendMessage("SetExist", true, SendMessageOptions.DontRequireReceiver);
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
