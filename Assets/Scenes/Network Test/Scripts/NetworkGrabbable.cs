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

    private void Awake()
    {
        IsHolding.OnValueChanged += (bool before, bool after) =>
        {
            if (after)
            {
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


    }

        public Rigidbody GetRigidbody() { return m_rigidBody; }

    public void SetExist(bool bExist)
    {
        if (bExist)
        {
            m_rigidBody.detectCollisions = true;

            for(int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            m_rigidBody.detectCollisions = false;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }
    }
}
