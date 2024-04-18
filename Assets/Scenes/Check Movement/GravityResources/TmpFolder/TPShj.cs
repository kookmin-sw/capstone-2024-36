using UnityEngine;

public class TPSHj : MonoBehaviour
{
    [SerializeField] private Transform characterBody;
    [SerializeField] private Transform cameraArm;

    //check ground
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _groundCheck;

    Camera characterCamera;

    private Animator _animator;

    private Vector3 _direction;
    private Rigidbody _rigidbody;
    private float _speed = 10;
    private float _turnSpeed = 1500f;

    private bool _isGrounded; // 캐릭터가 땅에 있는지 여부
    private float _groundCheckRadius = 0.3f;



    void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
        _animator = characterBody.GetComponent<Animator>();
        characterCamera = GetComponentInChildren<Camera>();

    }

    void Update()
    {
        Move();
        Catch();
        if (Input.GetMouseButton(1))
            LookAround();
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {

        }
    }

    void FixedUpdate()
    {
        bool isMoving = _direction.magnitude > 0.1f;

        if (isMoving)
        {
            Vector3 direction = transform.forward * _direction.z;
            _rigidbody.MovePosition(_rigidbody.position + direction * (_speed * Time.fixedDeltaTime));

            Quaternion rightDirection = Quaternion.Euler(0f, _direction.x * (_turnSpeed * Time.fixedDeltaTime), 0f);
            Quaternion newRotation = Quaternion.Slerp(_rigidbody.rotation, _rigidbody.rotation * rightDirection, Time.fixedDeltaTime * 3f);;
            _rigidbody.MoveRotation(newRotation);
        }

        _animator.SetBool("isMove", isMoving);
    }

    private void Move()
    {
        _direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundMask);
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;
        if (x < 180f)
            x = Mathf.Clamp(x, -1f, 70f);
        else
            x = Mathf.Clamp(x, 335f, 361f);

        cameraArm.rotation = Quaternion.Euler(x, camAngle.y - mouseDelta.x, camAngle.z);
    }


    public void Catch()
    {
        Ray ray = characterCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;
        if (Physics.Raycast(ray, out hitResult))
        {
            //Debug.Log(Input.mousePosition);
            Vector3 mouseDir = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
            //hitResult.transform.position = mouseDir;
            if (hitResult.collider.gameObject.tag == "moveable")
            {
                if (Input.GetMouseButton(0))
                {
                    hitResult.transform.position = mouseDir + _animator.transform.position + new Vector3(0, hitResult.transform.localScale.y / 2, 0);
                }
            }
        }
    }
}
