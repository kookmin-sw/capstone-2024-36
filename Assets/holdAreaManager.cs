using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class holdAreaManager : NetworkBehaviour
{
    //public Camera mainCamera;
    private void Start()
    {
        //mainCamera = Camera.main;
    }
    private void Update()
    {
        //Vector3 temp = transform.position;
        //temp.x = mainCamera.transform.position.x; 
        //transform.position = temp;
    }
}
