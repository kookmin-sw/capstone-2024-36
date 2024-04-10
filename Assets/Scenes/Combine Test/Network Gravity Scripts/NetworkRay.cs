
using UnityEngine;
using Unity.Netcode;
using System.Runtime.CompilerServices;

public class NetworkRay : NetworkBehaviour
{
    public GravityHandler gravityHandler;
    private BackToInitialPos backToInitialPos;
    //private TestReturnToStart testReturnToStart;
    private Camera playerCamera;
    //private NetworkObject networkObject;
    //ulong networkObjectId;

    private void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.R))
        {
            ShootRaycast();
        }
    }

    [ServerRpc]
    private void ShootRaycastServerRpc(int hitObjectRegisterID)
    {
        ShootRaycastClientRpc(hitObjectRegisterID);
    }


/*
    [ClientRpc]
    void ShootRaycastClientRpc(ulong networkObjectId)
    {
        FindNetworkObjectById(networkObjectId);
        hitObject = networkObject.gameObject;
        gravityHandler.ToggleGravity(hitObject);
        
        
        GameObject networkStarTestObject = GameObject.Find("Network Star Test");
        hitObject = networkStarTestObject;
        gravityHandler.ToggleGravity(hitObject);
        
    }
*/

    [ClientRpc]
    void ShootRaycastClientRpc(int hitObjectRegisterID)
    {
        //클라이언트의 모든 GravityObject 태그의 오브젝트를 가져오기
        GameObject[] allClientObjects = GameObject.FindGameObjectsWithTag("GravityObject");

        foreach (GameObject clientObject in allClientObjects)
        {
            //RegisterId를 비교하기 위한 NetworkObject 
            MyNetworkTransform clientNetworkTransform = clientObject.GetComponent<MyNetworkTransform>();

            //RegisterID 값 비교
            if (clientNetworkTransform != null)
            {
                int clientObjectRegisterID = clientNetworkTransform.RegisterId;

                if (hitObjectRegisterID == clientObjectRegisterID)
                {
                    gravityHandler.ToggleGravity(clientObject);
                }
            }
        }
    }

    [ServerRpc]
    private void ToIPosServerRpc(int hitObjectRegisterID)
    {
        Debug.Log("check ToIPosServerRpc");
        ToIPosClientRpc(hitObjectRegisterID);
    }

    [ClientRpc]
    void ToIPosClientRpc(int hitObjectRegisterID)
    {
        backToInitialPos = GetComponent<BackToInitialPos>();
        

        GameObject[] allClientObjects = GameObject.FindGameObjectsWithTag("moveable");

        foreach (GameObject clientObject in allClientObjects)
        {
            MyNetworkTransform clientNetworkTransform = clientObject.GetComponent<MyNetworkTransform>();
            if (clientNetworkTransform != null)
            {
                int clientObjectRegisterID = clientNetworkTransform.RegisterId;

                if (hitObjectRegisterID == clientObjectRegisterID)
                {
                    //testReturnToStart = clientObject.GetComponent<TestReturnToStart>();
                    backToInitialPos.ToInitialPos(clientObject);
                    //testReturnToStart.StartRecord();
                }
            }
        }
    }



    void ShootRaycast()
    {
        if(IsLocalPlayer)
        {
            playerCamera = Camera.main;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layerMask = 1 << LayerMask.NameToLayer("GravityArea");
            layerMask = ~layerMask;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.CompareTag("GravityObject"))
                {   
                    /*
                    Find NetworkObjectId : Fail

                    networkObject = hit.collider.gameObject.GetComponent<NetworkObject>();
                    Debug.Log(networkObject.name + " and " + networkObject);

                    Debug.Log(networkObject.NetworkObjectId + " : networkObject.NetworkObjectId");
                    networkObjectId = networkObject.NetworkObjectId;
                    Debug.Log(networkObjectId + " is network object ID");
                    */

                    MyNetworkTransform myNetworkTransform = hit.collider.gameObject.GetComponent<MyNetworkTransform>();
                    int hitObjectRegisterID = myNetworkTransform.RegisterId;
                    ShootRaycastServerRpc(hitObjectRegisterID);
                    Debug.Log("check GRAVITYOBJECT ");

                }
                else if(hit.collider.CompareTag("moveable"))
                {
                    Debug.Log("check moveable test 1");
                    MyNetworkTransform myNetworkTransform = hit.collider.gameObject.GetComponent<MyNetworkTransform>();
                    int hitObjectRegisterID = myNetworkTransform.RegisterId;
                    ToIPosServerRpc(hitObjectRegisterID);  
                }
            }
        }
    }
    /*

    NetworkObject FindNetworkObjectById(ulong networkObjectId)
    { 
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SpawnManager != null)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject networkObject))
            {
                Debug.Log("network Object found");
                return networkObject;
            }
        }
        return null;
    }
    */
}