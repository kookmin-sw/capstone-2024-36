using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GravityArea : MonoBehaviour
{
    [SerializeField] public int priority;
    public int Priority => priority;
    
    void Start()
    {
        transform.GetComponent<Collider>().isTrigger = true;
    }
    
    public abstract Vector3 GetGravityDirection(GravityBody _gravityBody);
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out GravityBody gravityBody))
        {
            gravityBody.AddGravityArea(this);
            Debug.Log("The Object Entered GravityArea");
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out GravityBody gravityBody))
        {
            gravityBody.RemoveGravityArea(this);
        }
    }
}
