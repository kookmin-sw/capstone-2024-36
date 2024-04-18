using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityCtrl : MonoBehaviour
{
    public GravityOrbit Gravity;
    private Rigidbody Rb;

    public float RotationSpeed = 20;

    void Start()
    {
        Rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if(Gravity)
        {
            Vector3 gravityUp = Vector3.zero;

            if(Gravity.FixedDirections)
            {
                gravityUp = Gravity.transform.up;
            }
            else
            {
                gravityUp = (transform.position - Gravity.transform.position).normalized;
            }

            gravityUp = (transform.position - Gravity.transform.position).normalized;

            Vector3 localUp = transform.up;

            Quaternion targetrotation = Quaternion.FromToRotation(localUp, gravityUp) * transform.rotation;
            
            transform.up = Vector3.Lerp(transform.up, gravityUp, RotationSpeed * Time.deltaTime);

            Rb.AddForce((-gravityUp * Gravity.Gravity) * Rb.mass);
        }
    }

    public Vector3 CurrentGravityDirection
    {
        get
        {
            if (Gravity != null)
            {
                return (transform.position - Gravity.transform.position).normalized;
            }
            return Vector3.up; // 기본 중력 방향
        }
    }




}
