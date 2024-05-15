using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : NetworkBehaviour
{
    Vector3 minScale = new Vector3(1f, 1f, 1f);
    Vector3 maxScale = new Vector3(1.5f, 1.5f, 1.5f);

    [SerializeField] float gravityForceMin = 400f;
    [SerializeField] float gravityForceMax = 1000f;

    public Vector3 GravityDirection
    {
        get
        {
            if (_gravityAreas.Count == 0)
            {
                return Vector3.zero;
            }

            GravityArea maxPriorityArea = _gravityAreas[0];
            
            foreach (var area in _gravityAreas)
            {
                if (area.Priority > maxPriorityArea.Priority)
                {
                    maxPriorityArea = area;
                }
            }

            return maxPriorityArea.GetGravityDirection(this).normalized;
        }
    }

    private Rigidbody _rigidbody;
    private List<GravityArea> _gravityAreas;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _gravityAreas = new List<GravityArea>();

        if (IsOwner)
        {
            _rigidbody.isKinematic = false;
        }
        else
        {
            _rigidbody.isKinematic = true;
        }
    }

    private void FixedUpdate()
    {
        Vector3 currentScale = transform.localScale;

        currentScale.x = Mathf.Clamp(currentScale.x, minScale.x, maxScale.x);
        currentScale.y = Mathf.Clamp(currentScale.y, minScale.y, maxScale.y);
        currentScale.z = Mathf.Clamp(currentScale.z, minScale.z, maxScale.z);

        //현재 스케일에 따라 중력을 계산
        float normalizedScale = Mathf.InverseLerp(minScale.x, maxScale.x, currentScale.x);
        float gravityForce = Mathf.Lerp(gravityForceMin, gravityForceMax, normalizedScale);

        // 중력 적용
        _rigidbody.AddForce(GravityDirection * (gravityForce * Time.fixedDeltaTime), ForceMode.Acceleration);

        Quaternion upRotation = Quaternion.FromToRotation(transform.up, -GravityDirection);
        Quaternion newRotation = Quaternion.Slerp(_rigidbody.rotation, upRotation * _rigidbody.rotation, Time.fixedDeltaTime * 3f);
        _rigidbody.MoveRotation(newRotation);
    }

    public void AddGravityArea(GravityArea gravityArea)
    {
        _gravityAreas.Add(gravityArea);
    }

    public void RemoveGravityArea(GravityArea gravityArea)
    {
        _gravityAreas.Remove(gravityArea);
    }
}
