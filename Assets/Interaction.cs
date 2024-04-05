using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    private GameObject Interact;
    
    // Start is called before the first frame update
    void Start()
    {
        Interact = GameObject.Find("Network Moving Test(Clone)");
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale[0] < 4.0f && Interact.GetComponent<MyNetworkTransform>().RegisterId == 2)
            Interact.transform.localScale = new Vector3(4.0f, 4.0f, 4.0f) -transform.localScale;
        Debug.Log(Interact.transform.localScale);
        Debug.Log(Interact.GetComponent<MyNetworkTransform>().RegisterId);
    }
}
