using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitObject : MonoBehaviour
{
    [SerializeField] private Transform centerObject; 
    public float orbitSpeed = 20f; 
    public float initialForce = 5f; 

    void Start()
    {
        GetComponent<Rigidbody>().velocity = Vector3.right * initialForce;
    }

    void Update()
    {
        transform.RotateAround(centerObject.position, Vector3.up, orbitSpeed * Time.deltaTime);
    }
}
