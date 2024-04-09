using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkGrabManager : NetworkBehaviour
{
    [Header("Status")]
    [SerializeField] private NetworkGrabbable m_catchTarget;
    [SerializeField] private float m_currentRotY;

    [Header("Setting")]
    [SerializeField] private KeyCode m_grabKey;
    [SerializeField] private float m_grabDistance;
    [SerializeField] private LayerMask m_layerMask;
    [SerializeField] private float m_holdDistance;
    [SerializeField] private float m_deltaY;
    [SerializeField] float m_smoothDampTime = 0.1f;

    private Vector3 m_posVelocity;
    private float m_rotationVelocity;

    private void Update()
    {
        if (IsLocalPlayer && Input.GetKeyDown(m_grabKey))
        {
            if (Camera.main.GetComponentInParent<CameraController>() == null)
                return;

            // 내려놓기
            if (m_catchTarget != null)
            {
                if (m_catchTarget.IsOwner)
                    m_catchTarget.GetRigidbody().isKinematic = false;

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
                // ... 
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
            // 뺏긴 경우
            if (!m_catchTarget.IsOwner)
            {
                m_catchTarget = null;
                return;
            }

            m_catchTarget.GetRigidbody().isKinematic = true;

            // lerp rot
            // m_catchTarget.transform.forward = transform.forward;

            m_currentRotY = Mathf.SmoothDampAngle(
                m_currentRotY, transform.rotation.eulerAngles.y, ref m_rotationVelocity, m_smoothDampTime
            );

            m_catchTarget.transform.rotation = Quaternion.Euler(0.0f, m_currentRotY, 0.0f);


            // lerp pos
            /*
            Vector3 targetPos =
                transform.position +
                transform.forward * m_holdDistance +
                Vector3.up * m_deltaY;

            m_catchTarget.transform.position = Vector3.SmoothDamp(
                m_catchTarget.transform.position,
                targetPos, ref m_posVelocity, m_smoothDampTime);
            */

            CameraController camCtrl = Camera.main.GetComponentInParent<CameraController>();
            Debug.Log("m_holdDistance : " + m_holdDistance);
            m_catchTarget.transform.position =
                transform.position +
                Quaternion.AngleAxis(m_currentRotY, Vector3.up) * Vector3.forward * m_holdDistance +
                Vector3.up * m_deltaY;

            /*
            if (camCtrl != null)
            {
                Quaternion.AngleAxis(camCtrl.VerticalAngle, Vector3.right)
                
            }
            else
            {

            }
            */


        }
    }
}
