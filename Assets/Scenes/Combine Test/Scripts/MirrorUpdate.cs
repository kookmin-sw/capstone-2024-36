using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorUpdate : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // x와 z 축의 회전 값을 0으로 설정
        currentRotation.x = 0f;
        currentRotation.z = 0f;

        // 변경된 회전 값을 다시 적용
        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
