using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionStay(Collision collision)
    {
        //plane에 충돌하는 경우
        if (collision.collider.gameObject.CompareTag("plane"))
        {
            Debug.Log("tag = " + transform.name);
        }
    }
}
