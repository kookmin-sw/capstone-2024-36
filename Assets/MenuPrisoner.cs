using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPrisoner : MonoBehaviour
{
    public float minX = -23.0f;
    public float maxX = 23.0f;
    public float walkSpeed = 10.0f;
    public bool isLeft;

    // Update is called once per frame
    void Update()
    {
        if (isLeft)
        {
            transform.position += new Vector3(-walkSpeed * Time.deltaTime, 0, 0);
        }
        else
        {
            transform.position += new Vector3(walkSpeed * Time.deltaTime, 0, 0);
        }

        if (transform.position.x > maxX || transform.position.x < minX)
        {
            Destroy(gameObject);
        }
    }
}
