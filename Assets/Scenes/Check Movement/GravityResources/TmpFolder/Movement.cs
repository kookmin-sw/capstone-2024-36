using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject spaceCraft;
    
    public GameObject mainCamera;

    public float xPos = 0f;
    public float yPos = 0f;
    public float zPos = 0f;

    public float xRot = 0f;
    public float yRot = 0f;
    public float zRot = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            yPos += 0.01f;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            xPos += 0.01f;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            yPos -= 0.01f;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            zPos -= 0.01f;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            xPos -= 0.01f;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            zPos += 0.01f;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            zRot += 10f;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            yRot -= 10f;
            // mainCamera의 현재 오일러 각을 가져옵니다.
            Vector3 currentEulerAngles = mainCamera.transform.eulerAngles;

            // 현재 각도에 새로운 회전값을 더합니다.
            currentEulerAngles += new Vector3(xRot, yRot, zRot);

            // 새로운 회전을 적용합니다.
            mainCamera.transform.rotation = Quaternion.Euler(currentEulerAngles);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            xRot += 10f;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            xRot -= 10f;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            zRot -= 10f;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            yRot += 10f;
        }

        xPos = Mathf.Clamp(xPos, -0.02f, 0.02f);
        yPos = Mathf.Clamp(yPos, -0.02f, 0.02f);
        zPos = Mathf.Clamp(zPos, -0.02f, 0.02f);

        


        spaceCraft.transform.position += new Vector3(xPos, yPos, zPos);
        spaceCraft.transform.rotation = Quaternion.Euler(xRot, yRot, zRot);


    }
}
