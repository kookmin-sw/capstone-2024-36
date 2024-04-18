using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LightDoorUpdate : NetworkBehaviour
{

    // need to gameobject alloacte
    public GameObject door;

    public NetworkVariable<bool> isDoor1Active = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // Start is called before the first frame update

    // Update is called once per frame

    void Start()
    {
        door = GameObject.Find("Door_2 Variant");
    }
    void Update()
    {
        if(door!=null){
            if(isDoor1Active.Value){
            if(door.transform.position.y >-2){
                door.transform.Translate(Vector3.down * Time.deltaTime);

            }
        }
        else{
            if(door.transform.position.y<2){
                door.transform.Translate(Vector3.up * Time.deltaTime);
            }
        }
        //if laser doesn't continously box , need to close door again 
        if(isDoor1Active.Value == true){
            isDoor1Active.Value = !isDoor1Active.Value;
            }

        }
        else{
            door = GameObject.Find("Door_2 Variant");
        }
    }

    public void ClearSuccess(){
        if(NetworkManager.Singleton.IsServer){
            if(isDoor1Active.Value == false){
            isDoor1Active.Value = !isDoor1Active.Value;
            }
            Debug.Log("door toggle success");
            }
    }

    public void SetExist(bool bExist)
    {
        if (bExist)
        {
            
        }
        else
        {

        }
    }
}
