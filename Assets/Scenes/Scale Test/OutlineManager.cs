using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;


public class OutlineManager : NetworkBehaviour
{
    [SerializeField] private NetworkGrabbable m_lookingTarget;
    [SerializeField] private NetworkGrabbable m_previousTarget;


    [SerializeField] private float m_grabDistance;
    [SerializeField] private LayerMask m_layerMask;
    [SerializeField] private float m_holdDistance;
    [SerializeField] float distance;

    void Update()
    {
        RaycastHit hit;
        bool bHit = Physics.Raycast(
            Camera.main.transform.position,
            Camera.main.transform.forward,
            out hit, m_grabDistance, m_layerMask
        );
        if (!bHit)

        m_lookingTarget = hit.transform.GetComponent<NetworkGrabbable>();
        if (m_lookingTarget != null)
        {
            Debug.Log(m_lookingTarget);

            if (m_previousTarget != null && m_lookingTarget != m_previousTarget)
            {
                m_previousTarget.GetComponentInChildren<Outline>().enabled = false;
            }
            distance = (transform.position - m_lookingTarget.transform.position).magnitude;
            //Debug.Log(IsLocalPlayer);
            //Debug.Log("��? = " + distance.ToString() + "���ƾ�" + m_holdDistance * 1.5f);
            if (IsLocalPlayer && distance < m_holdDistance)
            {
                //Debug.Log("���� ��?");
                if (m_lookingTarget.GetComponentInChildren<Outline>() != null)
                {
                    //Debug.Log("Ȱ��ȭ");
                    m_previousTarget = m_lookingTarget;
                    m_lookingTarget.GetComponentInChildren<Outline>().enabled = true;
                }
            }
            else
            {
                m_previousTarget.GetComponentInChildren<Outline>().enabled = false;
                m_lookingTarget.GetComponentInChildren<Outline>().enabled = false;
            }
        }
        else
        {
            m_previousTarget.GetComponentInChildren<Outline>().enabled = false;
        }

    }
}
