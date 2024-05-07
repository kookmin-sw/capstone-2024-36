using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsKinematicOff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 3초 후에 SetKinematic 함수 실행
        StartCoroutine(SetKinematicAfterDelay(1f));
    }

    IEnumerator SetKinematicAfterDelay(float delay)
    {
        // delay 시간만큼 대기
        yield return new WaitForSeconds(delay);

        // isKinematic 속성을 false로 변경
        GetComponent<Rigidbody>().isKinematic = false;
    }
}

