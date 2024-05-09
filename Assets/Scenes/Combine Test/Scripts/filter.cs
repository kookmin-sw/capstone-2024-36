using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class filter : MonoBehaviour
{
    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player");
        }
        else if (collision.gameObject.CompareTag("moveable"))
        {
            Debug.Log("Not Player");
        }
    }
}
