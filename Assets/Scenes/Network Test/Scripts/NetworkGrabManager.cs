using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class NetworkGrabManager : NetworkBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] Transform holdArea;
    private GameObject heldObj;
    private Rigidbody heldObjRB;

    [Header("Status")]

    [SerializeField] private NetworkGrabbable m_catchTarget;
    [SerializeField] private NetworkGrabbable m_lookingTarget;
    private Rigidbody m_catchTargetRB;

    [Header("Setting")]
    [SerializeField] private KeyCode m_grabKey;
    [SerializeField] private KeyCode m_rotateKey;
    [SerializeField] private LayerMask m_layerMask;


    [SerializeField] private float m_holdAreaZ = 2.5f;
    [SerializeField] private float m_rayDistance = 20.0f;
    [SerializeField] private float m_maxHoldDistance = 5.0f;
    [SerializeField] private float m_minHoldDistance = 1.0f;
    [SerializeField] private float m_upDownDegree = 4.0f;
    [SerializeField] private float m_largestScale = 4.0f;
    [SerializeField] private float m_smallestScale = 0.1f;

    [SerializeField] private float m_pickupForce = 150.0f;
    [SerializeField] float m_deltabuff = 0.1f;
    [SerializeField] float m_dragSpeed = 10.0f;
    [SerializeField] float m_dropAtDrag = 0.0f;
    [SerializeField] float distance;

    private float m_rotationVelocity;

    public void SetExist(bool bExist)
    {
        if (!bExist)
        {
            DropObject();
        }

    }

        private void Update()
    {
        if (IsLocalPlayer)
        {
            RaycastHit hit;
            bool bHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, m_rayDistance, m_layerMask);
            if(bHit)
            {
                m_lookingTarget = hit.transform.GetComponent<NetworkGrabbable>();
            }
            if (Input.GetKeyDown(m_grabKey))
            {
                //if (Camera.main.GetComponentInParent<CameraController>() == null) //카메라 없으면 밴?
                //    return;

                if(m_catchTarget == null) 
                {
                    // 눌렸는데 들려있지 않다면 줍기
                    if (bHit)
                    {
                        //Debug.Log("들어오냐?");
                        PickupObject(hit.transform.gameObject);
                    }
                }
                else 
                {
                    // 들려있는데 다시 눌리면 드랍
                    DropObject();
                }
            }
            if(m_catchTarget != null)
            {
                //들려있는 상태에서의 처리
                MoveObject();
            }
        }
    }

    void PickupObject(GameObject pickObj)
    {
        m_catchTarget = pickObj.GetComponent<NetworkGrabbable>(); //선택된 타겟의 grabbagble
        m_catchTargetRB = m_catchTarget.GetRigidbody();
        if (m_catchTarget.IsOwner) //잡힌 물체의 owner 처리ㅇ
        {
            //오너인경우
            m_catchTarget.IsHolding.Value = true; //잡음 변수 처리
        }
        else
        {
            //오너가 아닌경우
            NetworkSceneManager.Instance.RequestOwnerRpc(m_catchTarget.NetworkObjectId, NetworkManager.Singleton.LocalClientId); // 오너 요청
            return;
        }

        m_catchTargetRB.useGravity = false;
        m_catchTargetRB.drag = m_dragSpeed;
        m_catchTargetRB.constraints = RigidbodyConstraints.FreezeRotation;
        m_catchTargetRB.interpolation = RigidbodyInterpolation.None;
        //m_catchTargetRB.transform.parent = holdArea;

        if (!pickObj.GetComponent<Rigidbody>()) //선택된 오브젝트에 리지드바디확인
        {
            Debug.LogError("neTr is NULL");
        }
    }

    void DropObject()
    {
        // 내려놓기
        if (m_catchTarget != null)
        {
            if (m_catchTarget.IsOwner) //오너일경우
            {
                m_catchTarget.IsHolding.Value = false; //잡기 해제 반영
            }
            /*
            //놓았을때 속도 조정
            if (m_catchTarget.GetRigidbody().velocity.magnitude > m_dragSpeed) 
            {
                Vector3 dir = m_catchTarget.GetRigidbody().velocity.normalized;
                m_catchTarget.GetRigidbody().velocity = dir * m_dragSpeed;
            }
            */

            m_catchTargetRB.interpolation = RigidbodyInterpolation.Interpolate;
            m_catchTargetRB.constraints = RigidbodyConstraints.None;
            m_catchTargetRB.drag = m_dropAtDrag;
            m_catchTargetRB.useGravity = true;
            //m_catchTargetRB.transform.parent = null;
            m_catchTarget = null;
        }
    }

    void MoveObject()
    {
        distance = Vector3.Distance(transform.position, m_catchTarget.transform.position); //벡터길이 위의 m_holdDistance 하고 다른점은?


        float targetScale = m_catchTarget.transform.localScale.x - 1;
        holdArea.transform.localPosition = new Vector3(0, 0, 2.5f + targetScale / 2);
        //Debug.Log(distance.ToString() + "      " + (m_minHoldDistance + targetScale / 2).ToString());

        if (distance > m_maxHoldDistance + targetScale / 2) //물체가 들려있는상태로 distance가 잡은 것보다 1.5배 이상 멀어지면 떨
        {
            DropObject();
        }
        if (distance < m_minHoldDistance + targetScale / 2) //물체가 들려있는상태로 distance가 잡은 것보다 1.5배 이상 멀어지면 떨
        {
            //DropObject();
        }

        if (!m_catchTarget.IsOwner) // 뺏긴경우 떨
        {
            m_catchTarget = null;
            return;
        }


        //스케일
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (m_catchTarget.IsOwner && wheelInput > 0) //들고 있는 상태에서 크기조정
        {
            m_catchTarget.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        }
        if (m_catchTarget.IsOwner && wheelInput < 0)
        {
            m_catchTarget.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
        }

        //앵글
        if (m_catchTarget.IsOwner && Input.GetKey(m_rotateKey)) //들고있는 상태에서 돌
        {
            Debug.Log(Quaternion.AngleAxis(0, m_catchTarget.GetRigidbody().rotation.eulerAngles));
            
        }
        //else if (m_catchTarget.IsOwner && Input.GetKeyDown(m_rotateKey))
        //{
        //    m_catchTarget.GetRigidbody().rotation = m_catchTarget.GetRigidbody().rotation * Quaternion.Euler(0f, 15.0f, 0f);
        //}

        CameraController camCtrl = Camera.main.GetComponentInParent<CameraController>();

        Vector3 newPosition =  Vector3.up * -Mathf.Tan(camCtrl.getPivot().localRotation.x - Mathf.PI / 6) * m_upDownDegree;

        if (distance > m_deltabuff)
        {
            Vector3 moveDirection = (holdArea.transform.position - m_catchTargetRB.position);
            //Debug.Log(newPosition);
            moveDirection += newPosition;
            m_catchTargetRB.AddForce(moveDirection * m_pickupForce);
        }
    }



    /*
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
    [SerializeField] float m_forceSize = 5.0f;
    [SerializeField] float m_deltabuff = 0.1f;
    [SerializeField] float m_velocityReduce = 5.0f;
    [SerializeField] float m_velocity = 10.0f;
    `
    private Rigidbody rBody;
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
                {
                    m_catchTarget.IsHolding.Value = false;
                }
                if(m_catchTarget.GetRigidbody().velocity.magnitude > m_velocity)
                {
                    Vector3 dir = m_catchTarget.GetRigidbody().velocity.normalized;
                    m_catchTarget.GetRigidbody().velocity = dir * m_velocity;
                    Debug.Log(m_catchTarget.GetRigidbody().velocity);
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
            rBody = m_catchTarget.GetRigidbody();

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
            if (m_holdDistance < 2.5f)
                m_holdDistance = 2.5f;
            
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


            m_currentRotY = Mathf.SmoothDampAngle(
                m_currentRotY, transform.rotation.eulerAngles.y, ref m_rotationVelocity, m_smoothDampTime
            );

            //rBody.MoveRotation(Quaternion.Euler(0.0f, m_currentRotY, 0.0f));

            CameraController camCtrl = Camera.main.GetComponentInParent<CameraController>();

            Vector3 newPosition =
                transform.position +
                Quaternion.AngleAxis(m_currentRotY, Vector3.up) * Vector3.forward * m_holdDistance +
                Vector3.up * -Mathf.Tan(camCtrl.getPivot().localRotation.x-Mathf.PI/6) * m_holdDistance;
            //Debug.Log("rotation" + camCtrl.getPivot().localRotation.eulerAngles.x);
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
    */
}
