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
        yield return new WaitForSeconds(3f);
        bool isPositioned = false;
        Vector3 otherPosition = other.transform.position;
        Transform thisTransform = GetComponent<Transform>();


        Vector3 minBounds = new Vector3(
            thisTransform.position.x - thisTransform.localScale.x / 2f,
            thisTransform.position.y - thisTransform.localScale.y / 2f,
            thisTransform.position.z - thisTransform.localScale.z / 2f
        );
        Vector3 maxBounds = new Vector3(
            thisTransform.position.x + thisTransform.localScale.x / 2f,
            thisTransform.position.y + thisTransform.localScale.y / 2f,
            thisTransform.position.z + thisTransform.localScale.z / 2f
        ); 

        isPositioned = (
            (otherPosition.x >= minBounds.x && otherPosition.x <= maxBounds.x) &&
            (otherPosition.y >= minBounds.y && otherPosition.y <= maxBounds.y) &&
            (otherPosition.z >= minBounds.z && otherPosition.z <= maxBounds.z));

        if (isPositioned)
        {
            if (!isHandlingCollision && other.CompareTag("moveable"))
            {
                Rigidbody cubeRigidbody = other.GetComponent<Rigidbody>();
                BoxCollider cubeCollider = other.GetComponent<BoxCollider>();

                isHandlingCollision = true;
                cubeCollider.enabled = false;
                
                yield return new WaitForSeconds(3f);

                if (cubeRigidbody != null)
                {
                    other.transform.rotation = Quaternion.identity;

                    yield return new WaitForSeconds(0.1f);

                    cubeRigidbody.isKinematic = true;
                    other.transform.position = Vector3.Lerp(other.transform.position, thisTransform.position, 1 * Time.deltaTime);
                }

                isHandlingCollision = false;

                nextObsObj.SetActive(true);
                Destroy(GetComponent<BoxCollider>());
            }
        }
        //int otherRegId = other.GetComponent<MyNetworkTransform>().RegisterId;
        //BackToSavedPosServerRpc(otherRegId);
    }

    /*
    bool IsPositionWithinColliderBounds(Vector3 position)
    {
        Transform thisTransform = GetComponent<Transform>();
        Debug.Log(thisTransform.position);
        Debug.Log(position);

        Vector3 minBounds = new Vector3(
            thisTransform.position.x - thisTransform.localScale.x / 2f,
            thisTransform.position.y - thisTransform.localScale.y / 2f,
            thisTransform.position.z - thisTransform.localScale.z / 2f
        );
        Vector3 maxBounds = new Vector3(
            thisTransform.position.x + thisTransform.localScale.x / 2f,
            thisTransform.position.y + thisTransform.localScale.y / 2f,
            thisTransform.position.z + thisTransform.localScale.z / 2f
        );

        Debug.Log(minBounds + " : minBounds");
        Debug.Log(maxBounds + " : maxBounds");  
        
        Debug.Log(position.x >= minBounds.x && position.x <= maxBounds.x);  
        Debug.Log(position.y >= minBounds.y && position.y <= maxBounds.y);  
        Debug.Log(maxBounds + " : maxBounds");  


        // 주어진 위치가 Collider의 경계 내에 있는지 확인
        return (position.x >= minBounds.x && position.x <= maxBounds.x) &&
               (position.y >= minBounds.y && position.y <= maxBounds.y) &&
               (position.z >= minBounds.z && position.z <= maxBounds.z);
    }
    */


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
        }
            {
                int clientObjectRegisterID = clientNetworkTransform.RegisterId;

                if (regId == clientObjectRegisterID)
                {
                    BackToInitialPos backToInitialPos = clientObject.GetComponent<BackToInitialPos>();
                    backToInitialPos.ToInitialPos(clientObject);
                }
            }
    } 
    */
}
