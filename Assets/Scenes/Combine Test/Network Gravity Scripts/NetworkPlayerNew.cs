using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;


public class NetworkPlayerNew
    : NetworkBehaviour
{
    public static NetworkPlayerNew LocalIstance;

    enum eMesh
    {
        Man, Woman, None
    }
    [Header("Status")]
    [SerializeField] eMesh currentMesh = eMesh.None;
    [SerializeField] bool isLocal = false;
    [SerializeField] ulong ownerClientId;

    [Header("Setting")]
    [SerializeField] int walkSpeed;
    [SerializeField] float Gravity;

    [Header("Input")]
    [SerializeField] Vector2 movementInput;

    [Header("Reference")]
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] Transform manTransform;
    [SerializeField] Transform womanTransform;
    [SerializeField] MyNetworkTransform networkTransform;
    [SerializeField] CapsuleCollider rigidbodyCollider;
    [SerializeField] Animator animator;

    private PlayerControl m_playerControl;
    private Vector3 m_lastPostion;
    private Vector3 playerDirection;

    private float m_speedY;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        if (m_playerControl == null)
        {
            m_playerControl = new PlayerControl();

            m_playerControl.PlayerMovement.Movement.performed += (i) =>
            {
                movementInput = i.ReadValue<Vector2>();
            };
            m_playerControl.Enable();
        }

        networkTransform.SetSpawnPositionEvent += SetSpawnPosition;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        ownerClientId = OwnerClientId;

        if (IsOwner)
        {
            TitleScreen.IsLocalPlayerFound = true;
            LocalIstance = this;
            isLocal = true;
        }

        m_lastPostion = transform.position;
    }

    void Update()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    private void FixedUpdate()
    {
        /*
        if (isMoving)
        {
            Vector3 direction = transform.forward * playerDirection.z + transform.right * playerDirection.x;
            playerRigidbody.MovePosition(playerRigidbody.position + direction * (walkSpeed * Time.fixedDeltaTime));
        }

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        //cameraForward.y = 0f; // y축 방향은 고려하지 않음
        cameraRight.y = 0f;

        // 방향 벡터 계산
        Vector3 moveDirection = cameraForward.normalized * movementInput.y + cameraRight.normalized * movementInput.x;
        moveDirection.Normalize(); // 방향 벡터 정규화

        // 플레이어 이동
        playerRigidbody.MovePosition(playerRigidbody.position + moveDirection * walkSpeed * Time.fixedDeltaTime);

        */
        bool isMoving = playerDirection.magnitude > 0.1f;
        
        if (isMoving)
        {
            Vector3 direction = transform.forward * playerDirection.z + transform.right * playerDirection.x;
            playerRigidbody.MovePosition(playerRigidbody.position + direction * (walkSpeed * Time.fixedDeltaTime));
        }

        //animator.SetBool("isMove", isMoving);
    }


    private void Move()
    {
        playerDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
    }

    public override void OnNetworkDespawn()
    {
        if (LocalIstance == this)
        {
            LocalIstance = null;
        }
    }

    private void SetExist(bool bExist)
    {
        if (bExist)
        {
            if (IsHost == IsOwner)
            {
                manTransform.gameObject.SetActive(true);
                womanTransform.gameObject.SetActive(false);
                animator = manTransform.GetComponent<Animator>();
                currentMesh = eMesh.Man;
            }
            else
            {
                manTransform.gameObject.SetActive(false);
                womanTransform.gameObject.SetActive(true);
                animator = womanTransform.GetComponent<Animator>();
                currentMesh = eMesh.Woman;
            }

            rigidbodyCollider.enabled = true;
            m_playerControl.Enable();
        }
        else
        {
            m_playerControl.Disable();

            manTransform.gameObject.SetActive(false);
            womanTransform.gameObject.SetActive(false);

            currentMesh = eMesh.None;

            rigidbodyCollider.enabled = false;
        }
    }

    private void SetSpawnPosition()
    {
        if (IsOwner)
        {
            CameraController camCtrl = Camera.main.GetComponentInParent<CameraController>();
            if (camCtrl != null)
                camCtrl.Target = transform;
        }

        if (IsHost == IsOwner)
        {
            transform.position = SpawnPosManager.Instance.HostSpawnPos.position;
            transform.position += new Vector3(0, transform.position.y + 0.1f, 0);
        }
        else
        {
            transform.position = SpawnPosManager.Instance.GuestSpawnPos.position;
            transform.position += new Vector3(0, transform.position.y + 0.1f, 0);
        }
    }

}
