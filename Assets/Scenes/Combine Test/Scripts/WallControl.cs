using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallControl : MonoBehaviour
{
    public GameObject wall1;
    public bool isWall;
    private Vector3 currentPosition;
    public float speed = 5.0f;

    // Update is called once per frame
    void Start(){
        currentPosition = wall1.transform.position;
    }
    void Update()
    {
        if(isWall){
            if (currentPosition.x > -45)
        {
            wall1.transform.Translate(Vector3.back * speed * Time.deltaTime);
        }

        // x좌표가 -45에 도달하거나 지나치지 않도록 합니다
        if (wall1.transform.position.x < -45)
        {
            Vector3 newPosition = wall1.transform.position;
            newPosition.x = -45;
            wall1.transform.position = newPosition;
        }
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
