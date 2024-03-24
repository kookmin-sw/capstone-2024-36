using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections.Generic;

// TODO:
// CharacterController를 사용할 때는 position을 설정하기 전에 CharacterController를 꺼야하는 이슈가 있다. 
// Owner가 아닌 경우, 콜라이더만 사용하는게 나을까? 
// 혹은, NetworkTransform이 CharacterController를 사용하도록 수정?

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

    [Header("Input")]
    [SerializeField] Vector2 movementInput;

    [Header("Reference")]
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform manTransform;
    [SerializeField] Transform womanTransform;
    [SerializeField] MyNetworkTransform networkTransform;
    [SerializeField] CapsuleCollider rigidbodyCollider;

    private PlayerControl playerControl;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (playerControl == null)
        {
            playerControl = new PlayerControl();

            playerControl.PlayerMovement.Movement.performed += (i) =>
            {
                movementInput = i.ReadValue<Vector2>();
            };
            playerControl.Enable();
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
    }

    private void Update()
    {
        if (PlayerCamera.Instance() == null)
            return;

        // move character tranform by CharacterController
        if (IsOwner && characterController.enabled)
        {
            Vector3 speedDelta =
                PlayerCamera.Instance().transform.forward * movementInput.y +
                PlayerCamera.Instance().transform.right * movementInput.x;
            speedDelta.y = 0;
            speedDelta.Normalize();
            speedDelta *= walkSpeed;

            characterController.Move(speedDelta * Time.deltaTime);
        }
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
                currentMesh = eMesh.Man;
            }
            else
            {
                manTransform.gameObject.SetActive(false);
                womanTransform.gameObject.SetActive(true);
                currentMesh = eMesh.Woman;
            }

            characterController.enabled = true;
            rigidbodyCollider.enabled = true;
            playerControl.Enable();
        }
        else
        {
            playerControl.Disable();

            manTransform.gameObject.SetActive(false);
            womanTransform.gameObject.SetActive(false);

            currentMesh = eMesh.None;

            characterController.enabled = false;
            rigidbodyCollider.enabled = false;
        }
    }

    private void SetSpawnPosition()
    {
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
