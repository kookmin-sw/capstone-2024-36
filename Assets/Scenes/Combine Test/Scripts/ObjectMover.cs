using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public characterPickUp characterController;
    public GameObject object2;
    private bool isMovingObject2 = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // E 키를 누르면 Object2를 이동시키는 코루틴을 시작합니다.
            StartCoroutine(MoveObject2Coroutine());
        }
    }

    IEnumerator MoveObject2Coroutine()
    {
        if (!isMovingObject2)
        {
            isMovingObject2 = true;
            Debug.Log("E");

            // 3초 동안 Object2를 위로 이동시킵니다.
            float elapsedTime = 0f;
            while (elapsedTime < 3f)
            {
                object2.transform.Translate(Vector3.left * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            isMovingObject2 = false;
        }
    }
}
