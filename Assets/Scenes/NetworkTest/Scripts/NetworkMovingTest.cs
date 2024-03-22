using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Unity.Netcode;

public class NetworkMovingTest : NetworkBehaviour
{
    public Vector3 m_initialPosition;

    private Vector3 m_random;

    void Start()
    {
        m_random = new Vector3();
        if (Random.value > 0.5f)
            m_random.x = 1.0f;
        else
            m_random.x = -1.0f;

        if (Random.value > 0.5f)
            m_random.y = 1.0f;
        else
            m_random.y = -1.0f;

        if (Random.value > 0.5f)
            m_random.z = 1.0f;
        else
            m_random.z = -1.0f;
    }

    public override void OnNetworkSpawn()
    {
        m_initialPosition = transform.position;
    }

    private void Update()
    {
        // orbit XZ centered m_initialPosition
        if (IsOwner)
        {
            float speed = 2.0f;

            transform.position =
                m_initialPosition +
                new Vector3(Mathf.Cos(Time.time * speed), 0, Mathf.Sin(Time.time * speed)
            );

            transform.rotation = Quaternion.Euler(
                Time.time * speed * 60.0f * m_random.x,
                Time.time * speed * 60.0f * m_random.y,
                Time.time * speed * 60.0f * m_random.z
            );

            transform.localScale = new Vector3(
                1.0f + Mathf.Cos(Time.time * speed) * 0.5f * m_random.x,
                1.0f - Mathf.Cos(Time.time * speed) * 0.5f * m_random.y,
                1.0f + Mathf.Sin(Time.time * speed) * 0.5f * m_random.z
            );
        }
    }
}
