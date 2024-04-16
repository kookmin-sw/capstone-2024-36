using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    private MyNetworkTransform Interact;
    
    // Start is called before the first frame update
    void Start()
    {
        Interact = NetworkSceneManager.GetRegistered(100);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("RID = " + Interact.RegisterId);
        if (Interact.transform.localScale.x > 3.0f && Interact.RegisterId == 100)
            gameObject.SetActive(false);
    }
}
