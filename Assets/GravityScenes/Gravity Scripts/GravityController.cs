using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public GravityHandler gravityHandler;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //GravityControl 태그의 오브젝트를 찾아서
                if (hit.collider.gameObject.CompareTag("GravityControl"))
                {   
                    //중력존을 형성하는 스크립트 찾기
                    if (gravityHandler != null)
                    {
                            gravityHandler.ToggleGravity(hit.collider.gameObject);
                    }
                }
            }
        }
    }
}

