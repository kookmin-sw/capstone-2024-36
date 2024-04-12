using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class NetworkGrabManager : NetworkBehaviour
{
    [Header("Status")]
    [SerializeField] private NetworkGrabbable m_catchTarget;
    [SerializeField] private NetworkGrabbable m_lookingTarget;
    [SerializeField] private NetworkGrabbable m_previousTarget;
    [SerializeField] private float m_currentRotY;

    [Header("Setting")]
    [SerializeField] private KeyCode m_grabKey;
    [SerializeField] private float m_grabDistance;
    [SerializeField] private LayerMask m_layerMask;
    [SerializeField] private float m_holdDistance;
    [SerializeField] private float m_deltaY;
    [SerializeField] float m_smoothDampTime = 0.1f;
    [SerializeField] float m_forceSize = 5.0f;
    [SerializeField] float m_deltabuff = 0.1f;
    [SerializeField] float m_velocityReduce = 5.0f;

    private float m_rotationVelocity;
    private void Update()
    {
        RaycastHit Lhit;
        bool LbHit = Physics.Raycast(
            Camera.main.transform.position,
            Camera.main.transform.forward,
            out Lhit, m_grabDistance, m_layerMask
        );
        if (!LbHit)
            return;

        m_lookingTarget = Lhit.transform.GetComponent<NetworkGrabbable>();
        if (m_lookingTarget != null)
        {
            Debug.Log(m_lookingTarget);

            if (m_previousTarget != null && m_lookingTarget != m_previousTarget)
            {
                m_previousTarget.GetComponentInChildren<Outline>().enabled = false;
            }
            float distance = (transform.position - m_lookingTarget.transform.position).magnitude;
            //Debug.Log(IsLocalPlayer);
            //Debug.Log("잉? = " + distance.ToString() + "갸아악" + m_holdDistance * 1.5f);
            if (IsLocalPlayer && distance < m_holdDistance * 1.5f)
            {
                //Debug.Log("여기 옴?");
                if (m_lookingTarget.GetComponentInChildren<Outline>() != null)
                {
                    //Debug.Log("활성화");
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

        if (IsLocalPlayer && Input.GetKeyDown(m_grabKey))
        {
            if (Camera.main.GetComponentInParent<CameraController>() == null)
                return;

            // 내려놓기
            if (m_catchTarget != null)
            {
                if (m_catchTarget.IsOwner)
                {
                    m_catchTarget.IsHolding.Value = false;
                }

                m_catchTarget = null;
                return;
            }

            RaycastHit hit;
            bool bHit = Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out hit, m_grabDistance, m_layerMask
            );
            if (!bHit)
                return;

            m_catchTarget = hit.transform.GetComponent<NetworkGrabbable>();


            if (m_catchTarget == null)
                return;

            if (m_catchTarget.IsOwner)
            {
                m_catchTarget.IsHolding.Value = true;
            }
            else
            {
                NetworkSceneManager.Instance.RequestOwnerRpc(
                    m_catchTarget.NetworkObjectId,
                    NetworkManager.Singleton.LocalClientId
                );
                return;
            }
            m_holdDistance = Vector3.Distance(transform.position, m_catchTarget.transform.position);
        }

        if (m_catchTarget != null)
        {
            float distance = (transform.position - m_catchTarget.transform.position).magnitude;
            if (distance > m_holdDistance * 1.5f)
            {
                if (m_catchTarget.IsOwner)
                {
                    m_catchTarget.IsHolding.Value = false;
                }
                m_catchTarget = null;
                return;
            }

            // 뺏긴 경우
            if (!m_catchTarget.IsOwner)
            {
                m_catchTarget = null;
                return;
            }

            //스케일
            if (m_catchTarget.IsOwner && Input.GetKeyDown(KeyCode.R))
            {
                m_catchTarget.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            }
            if (m_catchTarget.IsOwner && Input.GetKeyDown(KeyCode.T))
            {
                m_catchTarget.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            }

            //앵글
            if (m_catchTarget.IsOwner && Input.GetKeyDown(KeyCode.Q))
            {
                m_catchTarget.transform.eulerAngles += new Vector3(0, 15f, 0);
            }


            Rigidbody rBody = m_catchTarget.GetRigidbody();

            m_currentRotY = Mathf.SmoothDampAngle(
                m_currentRotY, transform.rotation.eulerAngles.y, ref m_rotationVelocity, m_smoothDampTime
            );

            //rBody.MoveRotation(Quaternion.Euler(0.0f, m_currentRotY, 0.0f));

            CameraController camCtrl = Camera.main.GetComponentInParent<CameraController>();

            Vector3 newPosition =
                transform.position +
                Quaternion.AngleAxis(m_currentRotY, Vector3.up) * Vector3.forward * m_holdDistance +
                Vector3.up * -Mathf.Tan(camCtrl.getPivot().localRotation.x) * m_holdDistance * 2;
            if (newPosition.y < 0.65f)
                newPosition.y = 0.65f;

            Vector3 delta = newPosition - m_catchTarget.transform.position;
            if (delta.magnitude >= m_deltabuff)
            {
                rBody.velocity = (delta / Time.deltaTime) / m_velocityReduce;
            }
            else
            {
                rBody.velocity = Vector3.zero;
            }
        }
        else
        {
            m_rotationVelocity = 0;
            m_currentRotY = transform.rotation.eulerAngles.y;
        }
    }
}
