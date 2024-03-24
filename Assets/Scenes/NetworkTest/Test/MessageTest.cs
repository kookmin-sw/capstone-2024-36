using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("before");
            gameObject.SendMessage("PrintFloat", 5.0f, SendMessageOptions.DontRequireReceiver);
            Debug.Log("after");
        }
    }
}
