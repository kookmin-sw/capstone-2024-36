using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Active_Laser : MonoBehaviour
{
    public bool isClear;
    public GameObject laser;
    // Start is called before the first frame update

    // Update is called once per frame

    void Start()
    {
        laser = GameObject.Find("LaserPointer_Green 1(Clone)");
    }
    void Update()
    {
        if(laser == null){
            laser = GameObject.Find("LaserPointer_Green 1(Clone)");
        }
        isClear = gameObject.GetComponent<DoorUpdate>().isClear;
        if(isClear){
            laser.GetComponent<NetworkLaserPointerShoot>().isLaserActive.Value = true;
        }
    }
}
