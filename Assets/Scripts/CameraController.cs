using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Status")]
    public float VerticalAngle;
    public float HorizontalAngle;
    [SerializeField] private Vector2 m_mouseDelta;

    [Header("Setting")]
    [SerializeField] private float m_smoothSpeed = 1;
    [SerializeField] private float m_verticalSpeed= 220;
    [SerializeField] private float m_horizontalSpeed = 220;
    [SerializeField] private float m_minPivotV = -30;     // lowest point look down
    [SerializeField] private float m_maxPivotV = 80;     // highest point look up
    [SerializeField] private float m_collisionRadius = 0.2f;
    [SerializeField] private LayerMask m_collisionMask;

    [Header("Reference")]
    public Transform Target;
    [SerializeField] private Transform Pivot;
    [SerializeField] private Camera Cam;

    private Vector3 m_smoothVelocity;
    private PlayerControl m_playerControl;

    private float m_defaultCameraZ;
    private float m_targetCameraZ;
    private Vector3 m_cameraPosition = Vector3.zero;

    private void Awake()
    {
        if (m_playerControl == null)
        {
            m_playerControl = new PlayerControl();

            m_playerControl.PlayerCamera.Mouse.performed += (i) =>
            {
                m_mouseDelta = i.ReadValue<Vector2>();
            };
            m_playerControl.Enable();
        }
    }

    private void Start()
    {
        if (Cam != null)
            m_defaultCameraZ = Cam.transform.localPosition.z;
    }


    private void LateUpdate()
    {
        if (Target == null)
            return;

        if (Input.GetMouseButtonDown(1)){
            Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        // follow target
            Vector3 _targetCameraPosition = Vector3.SmoothDamp(
            transform.position, Target.transform.position, ref m_smoothVelocity, m_smoothSpeed * Time.deltaTime
        );
<<<<<<< Updated upstream
        transform.position = _targetCameraPosition + new Vector3(0, 1.0f, 0);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
=======
        transform.position = _targetCameraPosition; // + new Vector3(0, 1.0f, 0);
        
        if (m_bLockMouse)
        {
            //Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
>>>>>>> Stashed changes

        // ratation
        HorizontalAngle += m_mouseDelta.x * m_horizontalSpeed * Time.deltaTime;

        VerticalAngle -= m_mouseDelta.y * m_horizontalSpeed * Time.deltaTime;
        VerticalAngle = Mathf.Clamp(VerticalAngle, m_minPivotV, m_maxPivotV);

        Vector3 rot = Vector3.zero;
        rot.y = HorizontalAngle;
        transform.rotation = Quaternion.Euler(rot);

        rot = Vector3.zero;
        rot.x = VerticalAngle;
        Pivot.localRotation = Quaternion.Euler(rot);

        // handle collision
        m_targetCameraZ = m_defaultCameraZ;
        RaycastHit hit;
        Vector3 dir = Cam.transform.position - Pivot.transform.position;
        dir.Normalize();
        if (
            Physics.SphereCast(
                Pivot.transform.position, 
                m_collisionRadius, 
                dir, out hit, 
                Mathf.Abs(m_targetCameraZ),
                m_collisionMask
            )
        )
        {
            float distance = Vector3.Distance(Pivot.transform.position, hit.point);
            m_targetCameraZ = -(distance - m_collisionRadius);
        }   

        if (Mathf.Abs(m_targetCameraZ) < m_collisionRadius)
        {
            m_targetCameraZ = -m_collisionRadius;
        }

        m_cameraPosition.z = Mathf.Lerp(Cam.transform.localPosition.z, m_targetCameraZ, 0.2f);
        Cam.transform.localPosition = m_cameraPosition;

    }
}
