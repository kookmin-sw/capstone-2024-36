using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySetObject : MonoBehaviour
{    
    [SerializeField] GameObject setObject;
    private bool isSet;

    void Start()
    {
        isSet = setObject.activeSelf;
    }

    private void OnTriggerEnter(Collider other)
    {
        setObject.SetActive(false);
 
        /*
        else
        {
            setObject.SetActive(true);
            isSet = true;
        }
        */
    }
    
    private void OnTriggerExit(Collider other)
    {
        setObject.SetActive(true);
    
        /*
        else
        {
            setObject.SetActive(true);
            isSet = true;
        }
        */
    }
}



