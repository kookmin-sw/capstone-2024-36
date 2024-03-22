using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour
{
    private const float GRAVITY_FORCE = 800f;

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
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce(GravityDirection * (GRAVITY_FORCE * Time.fixedDeltaTime), ForceMode.Acceleration);

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
