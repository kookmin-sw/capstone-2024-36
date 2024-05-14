using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkGrabbable : NetworkBehaviour
{
    public NetworkVariable<bool> IsHolding = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    );

    [SerializeField] private Rigidbody m_rigidBody;
    [SerializeField] public bool isGrabbed;
    [SerializeField] public bool toggleGravity;

    private void Awake()
    {
        IsHolding.OnValueChanged += (bool before, bool after) =>
        {
            if (after)
            {
                GetRigidbody().useGravity = false;
            }
            else if(!after && toggleGravity)
            {
                Debug.Log("중력 true awa");
                GetRigidbody().useGravity = false;
            }
            else
            {
                GetRigidbody().useGravity = true;
            }
        };
    }

    public override void OnNetworkSpawn()
    {
        if (IsHolding.Value)
        {
            GetRigidbody().useGravity = false;
        }
        else if (toggleGravity)
        {
            Debug.Log("중력 true ons");
            GetRigidbody().useGravity = false;
        }
        else
        {
            GetRigidbody().useGravity = true;
        }
    }

    public Rigidbody GetRigidbody() { return m_rigidBody; }

    public void SetExist(bool bExist)
    {
        if (bExist)
        {
            m_rigidBody.detectCollisions = true;

            if (IsHolding.Value)
            {
                m_rigidBody.useGravity = false;
            }
            else if (toggleGravity)
            {
                Debug.Log("중력 true bex");
                GetRigidbody().useGravity = false;
            }
            else
            {
                GetRigidbody().useGravity = true;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            // TODO: 들고 이동 시 초기화 위치로 돌아가도록 설정 필요
            // 안그러면 기존 맵에 남아있는 사람 입장에서는 그냥 공중에 떠있게 됨

            m_rigidBody.detectCollisions = false;
            m_rigidBody.useGravity = false;
            m_rigidBody.velocity = Vector3.zero;
            m_rigidBody.angularVelocity = Vector3.zero;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("collision detect");
        isGrabbed = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("collision end");
        isGrabbed = false;
    }

}
