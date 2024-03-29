using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer2 : MonoBehaviour
{
    public float Speed;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 delta = Camera.main.transform.forward;
            delta.y = 0;
            delta.Normalize();
            transform.position += delta * Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            Vector3 delta = -Camera.main.transform.forward;
            delta.y = 0;
            delta.Normalize();
            transform.position += delta * Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            Vector3 delta = -Camera.main.transform.right;
            delta.y = 0;
            delta.Normalize();
            transform.position += delta * Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            Vector3 delta = Camera.main.transform.right;
            delta.y = 0;
            delta.Normalize();
            transform.position += delta * Speed * Time.deltaTime;
        }
    }
}
