using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TPSCharacterController : MonoBehaviour
{
    [SerializeField]
    private Transform characterBody;

    Animator animator;

    void Start()
    {
        animator = characterBody.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public float moveSpeed = 0.12f;
    public float strafeSpeed = 0.12f;

    public float horizontal;
    public float vertical;

    public Vector3 move;

    public CharacterController playerController;

    [Header("PlayerRotationReset")]

    public float rotationSpeed;
    public Quaternion newResetAngle;
    public Camera cam;

    void Awake()
    {
        playerController = GetComponent<CharacterController>();
        CinemachineCore.GetInputAxis = clickControl;
    }

    public float clickControl(string axis)
    {
        if (Input.GetMouseButton(1))
            return UnityEngine.Input.GetAxis(axis);

        return 0;
    }

    void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        move = new Vector3(horizontal, 0, vertical);
        playerController.Move(move * Time.deltaTime * moveSpeed);
        bool isMove = move.magnitude != 0;
        animator.SetBool("isMove", isMove);

        if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
        {
            newResetAngle = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, newResetAngle, rotationSpeed * Time.deltaTime).normalized;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            playerController.Move(transform.TransformDirection(Vector3.forward) * moveSpeed);
        }

        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            playerController.Move(transform.TransformDirection(Vector3.back) * moveSpeed);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            playerController.Move(transform.TransformDirection(Vector3.left) * strafeSpeed);
        }

        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            playerController.Move(transform.TransformDirection(Vector3.right) * strafeSpeed);
        }
    }
}
