using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TPSCharacterController : MonoBehaviour
{
    [SerializeField] private Transform characterBody;
    Animator animator;

    public Camera getCamera;
    private RaycastHit hit;

    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMask;
    private float _groundCheckRadius = 1f;
    private Vector3 _direction;
    private Rigidbody _rigidbody;
    private bool _isGrounded; 

    private enum State
    {
        Size,
        Laser,
        Gravity,
    }
    private State _state;

    void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
        animator = characterBody.GetComponent<Animator>();
        _state = State.Size;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        //R로 물체 회전
        if (Input.GetKey(KeyCode.Q))
        {
            hit.transform.eulerAngles += new Vector3(0f, 0.2f, 0f);
            Debug.Log("Q");
        }
        //R로 물체 회전
        if (Input.GetKey(KeyCode.E))
        {
            hit.transform.eulerAngles += new Vector3(0f, -0.2f, 0f);
            Debug.Log("E");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _state = State.Size;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _state = State.Laser;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _state = State.Gravity;
        }

        switch (_state)
        {
            case State.Size:
                Ray ray = getCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "moveable")
                    {
                        //Q로 물체의 크기 키우기
                        if (Input.GetKeyDown(KeyCode.R) && hit.transform.localScale[0] < 10.0f && hit.transform.localScale[1] < 10.0f && hit.transform.localScale[2] < 10.0f)
                        {
                            hit.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                        }
                        else if (Input.GetKey(KeyCode.R) && hit.transform.localScale[0] < 10.0f && hit.transform.localScale[1] < 10.0f && hit.transform.localScale[2] < 10.0f)
                        {
                            hit.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                        }
                        //E로 물체의 크기 줄이기
                        if (Input.GetKeyDown(KeyCode.T) && hit.transform.localScale[0] > 1.0f && hit.transform.localScale[1] > 1.0f && hit.transform.localScale[2] > 1.0f)
                        {
                            hit.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                        }
                        else if (Input.GetKey(KeyCode.T) && hit.transform.localScale[0] > 1.0f && hit.transform.localScale[1] > 1.0f && hit.transform.localScale[2] > 1.0f)
                        {
                            hit.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                        }
                    }
                }
                break;

            case State.Laser:

                break;

            case State.Gravity:
                break;
        }
    }


    public float moveSpeed = 0.12f;
    public float strafeSpeed = 0.12f;

    public float horizontal;
    public float vertical;

    public Vector3 move;

    [Header("PlayerRotationReset")]

    public float rotationSpeed;
    public Quaternion newResetAngle;
    public Camera cam;

    void Awake()
    {

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
        bool isMoving = _direction.magnitude > 0.1f;

        if (isMoving)
        {
            Vector3 direction = transform.forward * _direction.z + transform.right * _direction.x;
            _rigidbody.MovePosition(_rigidbody.position + direction * (moveSpeed * Time.fixedDeltaTime));
        }

        if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
        {
            newResetAngle = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, newResetAngle, rotationSpeed * Time.deltaTime).normalized;
        }

        
    }

    private void Move()
    {
        _direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundMask);
    }
}
