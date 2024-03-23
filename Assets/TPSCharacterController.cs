using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TPSCharacterController : MonoBehaviour
{
    [SerializeField]
    private Transform characterBody;

    Animator animator;
    public Camera getCamera;
    public Material lasermaterial;
    private RaycastHit hit;

    public Color lasercolor = Color.red;

    private enum State
    {
        Size,
        Laser,
        Gravity,
    }
    private State _state;

    void Start()
    {
        animator = characterBody.GetComponent<Animator>();
        _state = State.Size;
    }

    // Update is called once per frame
    void Update()
    {
        //R로 물체 회전
        if (Input.GetKey(KeyCode.Q))
        {
            hit.transform.eulerAngles += new Vector3(0f, 0.9f, 0f);
            Debug.Log("Q");
        }
        //R로 물체 회전
        if (Input.GetKey(KeyCode.E))
        {
            hit.transform.eulerAngles += new Vector3(0f, -0.9f, 0f);
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
                if (Input.GetKeyDown(KeyCode.G))
                {
                    ToggleLaser();
                }
                Ray laserray = getCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(laserray, out hit))
                {
                    if (Input.GetKeyDown(KeyCode.F)){
                            Vector3 direction = (hit.point - transform.position).normalized;
                            LaserBeam beam = new LaserBeam(transform.position,direction, lasermaterial,lasercolor ,"laserability"); 
                              
                            Invoke(nameof(DestroyLaserAbility), 0.3f);
                        }
                    
                    
                }
                break;

            case State.Gravity:
                break;
        }
    }

    void ToggleLaser()
    {
       if(lasercolor == Color.red){
        lasercolor = Color.blue;
       }
       else if(lasercolor == Color.blue){
        lasercolor = Color.green;
       }
       else if(lasercolor == Color.green){
        lasercolor = Color.red;
       }
    }
    public void DestroyLaserAbility()
{
    // 1초 후에 실행될 코드
    GameObject laserAbility = GameObject.Find("laserability");
    if (laserAbility != null)
    {
        Destroy(laserAbility);
    }
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
