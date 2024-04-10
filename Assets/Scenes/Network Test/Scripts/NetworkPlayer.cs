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

    [Header("Setting")]
    [SerializeField] int walkSpeed;
    [SerializeField] float Gravity;

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

    private void Update()
    {
        // move character tranform by CharacterController
        if (IsOwner && characterController.enabled)
        {
            Vector3 speedDelta =
                Camera.main.transform.forward * movementInput.y +
                Camera.main.transform.right * movementInput.x;
            speedDelta.y = 0;
            speedDelta.Normalize();
            speedDelta *= walkSpeed;

            characterController.Move(speedDelta * Time.deltaTime);

            if (speedDelta.magnitude > float.Epsilon)
            {
                animator?.SetBool("is_walking", true);
                transform.forward = speedDelta;
            }
            else
            {
                animator?.SetBool("is_walking", false);
            }

            characterController.Move(Vector3.down * Gravity * Time.deltaTime);
        }
        else
        {
            Vector3 delta = transform.position - m_lastPostion;
            if (delta.magnitude > float.Epsilon)
            {
                if (animator != null)
                    animator?.SetBool("is_walking", true);
            }
            else
            {
                if (animator != null)
                    animator?.SetBool("is_walking", false);
            }
        }

        m_lastPostion = transform.position;
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
