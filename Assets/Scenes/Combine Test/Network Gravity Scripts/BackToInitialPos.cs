using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToInitialPos : MonoBehaviour
{
    public Vector3 initialPosition;
    [SerializeField] private Quaternion initialRotations;

    void Start()
    {
        //Start에 안두고 함수로 만든다음 가지고 있는 다른 컴포넌트에 존재하는 스크립트에 넣는게 어떤지 확인해보기
        //거기서 함수 호출
        initialPosition = transform.localPosition;
        initialRotations = Quaternion.Normalize(transform.localRotation);
    }
    public void ToInitialPos(GameObject clientObject)
    {
        Debug.Log(clientObject + " : check clientObj");
        Rigidbody rb = clientObject.GetComponent<Rigidbody>();

        clientObject.transform.position = initialPosition;
        clientObject.transform.rotation = initialRotations;
                            
        //물리 시뮬레이션 재시작 
        //isKinematic을 꺼놓은 상태라 한번 받은 힘의 영향을 계속 받아서 
        //원래 자리로 돌아가도 계속 움직이기 때문에
        rb.isKinematic = true;
        rb.isKinematic = false;
    }
}
