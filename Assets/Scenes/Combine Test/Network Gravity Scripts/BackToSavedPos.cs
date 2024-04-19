using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class BackToSavedPos : NetworkBehaviour
{
    void Start()
    {
        transform.GetComponent<Collider>().isTrigger = true;
        Debug.Log("check BackToSavedPos Start");
    }

    [ServerRpc]
    private void BackToSavedPosServerRpc(int regId)
    {
        BackToSavedPosClientRpc(regId);
    }

    [ClientRpc]
    void BackToSavedPosClientRpc(int regId)
    {
        if (regId == -1)
        {
            Debug.Log("check regId " + regId);
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            Debug.Log("check playerObj " + playerObj);
            SavedPlayerPos savedPlayerPos = playerObj.GetComponent<SavedPlayerPos>();
            Debug.Log("check playerObj " + savedPlayerPos);
            playerObj.transform.position = savedPlayerPos.playerPos;
        }
        else
        {
            GameObject[] allClientObjects = GameObject.FindGameObjectsWithTag("moveable");

            foreach (GameObject clientObject in allClientObjects)
            {
                MyNetworkTransform clientNetworkTransform = clientObject.GetComponent<MyNetworkTransform>();
                if (clientNetworkTransform != null)
                {
                    int clientObjectRegisterID = clientNetworkTransform.RegisterId;
                    Debug.Log("check client obj regId " + clientObjectRegisterID);
                    if (regId == clientObjectRegisterID)
                    {
                        BackToInitialPos backToInitialPos = clientObject.GetComponent<BackToInitialPos>();
                        backToInitialPos.ToInitialPos(clientObject);
                    }
                }
            }
        } 
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("check if player : " + other);

            BackToSavedPosServerRpc(-1);

        }
        else if (other.CompareTag("moveable"))
        {
            Debug.Log("check if other obj : " + other);
            BackToInitialPos moveableTransform = other.GetComponent<BackToInitialPos>();

            MyNetworkTransform myNetworkTransform = other.gameObject.GetComponent<MyNetworkTransform>();
            int otherRegisterID = myNetworkTransform.RegisterId;

            BackToSavedPosServerRpc(otherRegisterID);
        }
    }

    
}
