using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class SubmitCube : NetworkBehaviour
{
    bool isHandlingCollision = false;
    [SerializeField] GameObject nextObsObj;
    [SerializeField] GameObject nextObsObjPrefab; 

    IEnumerator HandleCollision(Collider other)
    {
        if (!isHandlingCollision && other.CompareTag("moveable"))
        {
            isHandlingCollision = true;
            
            yield return new WaitForSeconds(6f);

            Rigidbody cubeRigidbody = other.GetComponent<Rigidbody>();

            if (cubeRigidbody != null)
            {
                other.transform.rotation = Quaternion.identity;

                yield return new WaitForSeconds(0.1f);

                cubeRigidbody.isKinematic = true;
            }

            isHandlingCollision = false;
        }
        nextObsObj.SetActive(true);
        Destroy(GetComponent<BoxCollider>());
        //int otherRegId = other.GetComponent<MyNetworkTransform>().RegisterId;
        //BackToSavedPosServerRpc(otherRegId);
    }


    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(HandleCollision(other));
    }

    void OnTriggerExit(Collider other)
    {
        StopCoroutine(HandleCollision(other));
    }
    /*
    [ServerRpc]
    private void BackToSavedPosServerRpc(int regId)
    {
        BackToSavedPosClientRpc(regId);
    }

    [ClientRpc]
    void BackToSavedPosClientRpc(int regId)
    {
        GameObject[] allClientObjects = GameObject.FindGameObjectsWithTag("moveable");

        foreach (GameObject clientObject in allClientObjects)
        {
            MyNetworkTransform clientNetworkTransform = clientObject.GetComponent<MyNetworkTransform>();
            if (clientNetworkTransform != null)
            {
                int clientObjectRegisterID = clientNetworkTransform.RegisterId;

                if (regId == clientObjectRegisterID)
                {
                    BackToInitialPos backToInitialPos = clientObject.GetComponent<BackToInitialPos>();
                    backToInitialPos.ToInitialPos(clientObject);
                }
            }
        }
    } 
    */
}
