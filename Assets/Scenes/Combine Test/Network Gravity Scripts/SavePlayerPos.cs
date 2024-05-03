using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePlayerPos : MonoBehaviour
{
    void Start()
    {
        transform.GetComponent<Collider>().isTrigger = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerForGravity"))
        {
            Debug.Log("check : " + other);
            SavedPlayerPos savedPlayerPos = other.GetComponent<SavedPlayerPos>();
            
            if (savedPlayerPos != null)
            {
                savedPlayerPos.playerPos = transform.position;
            }
        }
    }
}
