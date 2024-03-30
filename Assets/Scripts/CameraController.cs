using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float m_cameraSmoothSpeed = 1;

    [Header("Reference")]
    public Transform target;

    private Vector3 m_cameraVelocity;

    void HandleCameraAction()
    {
        if (target != null)
        {

        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetCameraPosition = Vector3.SmoothDamp(
            transform.position, target.transform.position, ref m_cameraVelocity, m_cameraSmoothSpeed * Time.deltaTime
        );
        transform.position = targetCameraPosition;
    }
}
