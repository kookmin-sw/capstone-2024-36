using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Diagnostics;
using Unity.VisualScripting;

public class LightDoorUpdate : MonoBehaviour
{

    // need to gameobject alloacte
    public GameObject door;
    public GameObject door1;

    public Color ClearColor;
    public Color receiveColor;

    public bool isDoorActive;
    // public NetworkVariable<bool> isDoor1Active = new NetworkVariable<bool>(
    //     false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // Start is called before the first frame update

    // Update is called once per frame

    void Start()
    {
        //door = GameObject.Find("Door_2 Variant");
    }
    void Update()
    {
        if (door != null)
        {
            if (isDoorActive)
            {
                if (door.transform.position.y > -2)
                {
                    door.transform.Translate(Vector3.down * Time.deltaTime *3);
                    door1.transform.Translate(Vector3.down * Time.deltaTime * 3);
                }
            }
            else
            {
                if (door.transform.position.y < 2)
                {
                    door.transform.Translate(Vector3.up * Time.deltaTime);
                    door1.transform.Translate(Vector3.up * Time.deltaTime);
                }
            }
            //if laser doesn't continously box , need to close door again 
            
        }
        else
        {
            //door = GameObject.Find("Door_2 Variant");
        }
    }

    public void ClearSuccess(Color color){
        receiveColor = color;
        UnityEngine.Debug.Log(receiveColor + "clearcolor" + ClearColor);
        if(receiveColor == ClearColor){
            isDoorActive = true;
            UnityEngine.Debug.Log("clear" + isDoorActive);
        }




        // if(NetworkManager.Singleton.IsServer){
        //     if(isDoor1Active.Value == false){
        //     isDoor1Active.Value = !isDoor1Active.Value;
        //     }
        //     Debug.Log("door toggle success");
        //     }
    }


    

    void OnTriggerExit(Collider other)
    {
        isDoorActive = false;
        UnityEngine.Debug.Log("exit");
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
