using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallControl : MonoBehaviour
{
    public GameObject wall1;
    public bool isWall;

    // Update is called once per frame
    void Update()
    {
        if(isWall){
            wall1.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("walltrigger on");

    }

    void OnTriggerEnter(Collider collision){
        Debug.Log("trigger");
        isWall = true;
    }
}
