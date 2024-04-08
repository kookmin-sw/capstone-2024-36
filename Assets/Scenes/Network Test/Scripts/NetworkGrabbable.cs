using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkGrabbable : NetworkBehaviour
{
    [SerializeField] private Rigidbody m_rigidBody;

    public Rigidbody GetRigidbody() { return m_rigidBody; }
}
