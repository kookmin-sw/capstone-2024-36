using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityOrbit : MonoBehaviour
{
    public float Gravity;

    public bool FixedDirections;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GravityCtrl>())
        {
            other.GetComponent<GravityCtrl>().Gravity = this.GetComponent<GravityOrbit>();
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }
}
