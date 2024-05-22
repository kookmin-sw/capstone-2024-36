using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReciprocatingCircleMotion : MonoBehaviour
{
    Vector3 position; 
    [SerializeField] float delta = 8.0f; 
    [SerializeField] float Xspeed = 3.0f; 
    [SerializeField] float Yspeed = 0.0f; 

    void Start()
    {
        position = transform.position;
    }


    void Update()
    {
        Vector3 v = position;
        v.x += delta * Mathf.Sin(Time.time * Xspeed);
        v.y += delta * Mathf.Cos(Time.time * Yspeed);
        transform.position = v;
    }
}
