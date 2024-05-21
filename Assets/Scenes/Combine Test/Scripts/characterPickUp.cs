using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterPickUp : MonoBehaviour
{
    public GameObject object1; // 캐릭터가 잡을 수 있는 물체
    private bool isHoldingObject1 = false;

    public bool IsHoldingObject1()
    {
        return isHoldingObject1;
    }

    void Update()
    {
        // E 키로 물체를 잡거나 놓습니다.
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isHoldingObject1)
            {
                DropObject();
            }
            else
            {
                PickUpObject();
            }
        }
    }

    void PickUpObject()
    {
        if (Vector3.Distance(transform.position, object1.transform.position) < 208.0f) // 일정 거리 이내
        {
            // Prefab의 인스턴스를 생성합니다.
            GameObject objectInstance = Instantiate(object1, transform.position, Quaternion.identity);

            // 캐릭터의 자식으로 설정합니다.
            objectInstance.transform.parent = transform;

            // 인스턴스의 로컬 위치를 초기화합니다.
            objectInstance.transform.localPosition = Vector3.zero;

            // 캐릭터가 물체를 들었음을 표시합니다.
            isHoldingObject1 = true;
            Debug.Log('A');
        }
        else
        {
            Debug.Log('B');
        }
    }

    void DropObject()
    {
        // Prefab의 인스턴스를 생성합니다.
        GameObject objectInstance = Instantiate(object1, transform.position, Quaternion.identity);

        // 캐릭터의 자식으로 설정합니다.
        objectInstance.transform.parent = transform;
        isHoldingObject1 = false;
        Debug.Log('C');
    }
}
