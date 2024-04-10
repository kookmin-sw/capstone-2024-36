using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer
    : NetworkBehaviour
{
    public static NetworkPlayer LocalIstance;

    enum eMesh
    {
        Man, Woman, None
    }
    [Header("Status")]
    [SerializeField] eMesh currentMesh = eMesh.None;
    [SerializeField] bool isLocal = false;
    [SerializeField] ulong ownerClientId;
    [SerializeField] private float m_speedY;

    [Header("Setting")]
    [SerializeField] int walkSpeed;
    [SerializeField] float Gravity;
    [SerializeField] float MaxGravity;
    [SerializeField] float JumpPower;

    [Header("Input")]
    [SerializeField] Vector2 movementInput;

    [Header("Reference")]
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform manTransform;
    [SerializeField] Transform womanTransform;
    [SerializeField] MyNetworkTransform networkTransform;
    [SerializeField] CapsuleCollider rigidbodyCollider;
    [SerializeField] Animator animator;

    private PlayerControl m_playerControl;
    private Vector3 m_lastPostion;

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

    private void Update()
    {
        // move character tranform by CharacterController
        if (IsOwner && characterController.enabled)
        {
            Vector3 new_forward = Camera.main.transform.forward;
            new_forward.y = 0;
            transform.forward = new_forward;

            Vector3 speedDelta =
                Camera.main.transform.forward * movementInput.y +
                Camera.main.transform.right * movementInput.x;
            speedDelta.y = 0;
            speedDelta.Normalize();
            speedDelta *= walkSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                speedDelta *= 1.7f;
            }

            characterController.Move(speedDelta * Time.deltaTime);

            if (animator != null)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    animator.SetFloat("forward", movementInput.y * 2.0f);
                    animator.SetFloat("right", movementInput.x * 2.0f);
                }
                else
                {
                    animator.SetFloat("forward", movementInput.y);
                    animator.SetFloat("right", movementInput.x);
                }

                if (speedDelta.magnitude > float.Epsilon)
                {
                    animator.SetBool("is_walking", true);
                }
                else
                {
                    animator.SetBool("is_walking", false);
                }
            }

            m_speedY -= Gravity * Time.deltaTime;
            if (m_speedY < -MaxGravity)
                m_speedY = -MaxGravity;

            if (characterController.isGrounded)
            {
                m_speedY = 0;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_speedY = JumpPower;
            }

            characterController.Move(Vector3.up * m_speedY * Time.deltaTime);
        }
        else
        {
            Vector3 delta = transform.position;
            delta.y = 0.0f;
            delta -= m_lastPostion;

            if (delta.magnitude > float.Epsilon)
            {
                if (animator != null)
                    animator.SetBool("is_walking", true);
            }
            else
            {
                if (animator != null)
                    animator.SetBool("is_walking", false);
            }
        }

        m_lastPostion = transform.position;
        m_lastPostion.y = 0;
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

            characterController.enabled = true;
            rigidbodyCollider.enabled = true;
            m_playerControl.Enable();
        }
        else
        {
            m_playerControl.Disable();

            manTransform.gameObject.SetActive(false);
            womanTransform.gameObject.SetActive(false);

            currentMesh = eMesh.None;

            characterController.enabled = false;
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
            transform.position += new Vector3(0, characterController.skinWidth, 0);
        }
        else
        {
            transform.position = SpawnPosManager.Instance.GuestSpawnPos.position;
            transform.position += new Vector3(0, characterController.skinWidth, 0);
        }
    }
}
