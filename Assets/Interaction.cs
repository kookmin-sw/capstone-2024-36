using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] int scale_RID;
    [SerializeField] int rot_RID;

    private MyNetworkTransform Interact_scale;
    private MyNetworkTransform Interact_rot;
    private bool scaleCheck = false;
    private bool rotCheck = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Interact_scale = NetworkSceneManager.GetRegistered(scale_RID);
        Interact_rot = NetworkSceneManager.GetRegistered(rot_RID);
    }

    // Update is called once per frame
    void Update()
    {
        if (Interact_scale.transform.localScale.x > 3.0f && Interact_scale.RegisterId == scale_RID)
            scaleCheck = true;
        else
            scaleCheck = false;

        if (Interact_rot.transform.rotation.y > 35.0f && Interact_rot.transform.rotation.y < 55.0f && Interact_rot.RegisterId == rot_RID)
            rotCheck = true;
        else
            rotCheck = false;

        if (scaleCheck && rotCheck)

            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}
