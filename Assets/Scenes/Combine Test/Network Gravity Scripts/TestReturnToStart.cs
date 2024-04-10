/*
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class TestReturnToStart : NetworkBehaviour
{
    public float recordInterval = 0.01f; // 위치 기록 단위
    private List<Vector3> positions = new List<Vector3>(); // 위치 기록 리스트
    private Rigidbody rb;
    private bool isRecord = false;
    private Quaternion originalRotation;



    private void Start()
    {

    }

    
    void Update()
    {
        bool isMoved = rb.velocity.magnitude > 0.1f;
        
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("check T");
            StartCoroutine(RecordPositionRoutine());
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("check Y");
            StartCoroutine(ReverseMovementRoutine());
        }
        
    }


    IEnumerator RecordPositionRoutine()
    {
        Debug.Log("check RecordPositionRoutine");
        float elapsedTime = 0f; // 경과 시간을 저장할 변수
        originalRotation = transform.rotation;

        while (elapsedTime < 10.0f) 
        {
            positions.Add(transform.position); // 현재 위치 기록
            if (positions.Count > 10000)
            {
                positions.RemoveAt(0); // 가장 오래된 위치 제거
            }

            elapsedTime += recordInterval; // 경과 시간 증가
            yield return new WaitForSeconds(recordInterval);
        }

        Debug.Log("recording finish");
    }

    [ServerRpc]
    public void StartReverseMovementServerRpc()
    {
        Debug.Log("check StartReverseMovementServerRpc");
        StartCoroutine(ReverseMovementRoutine());
    }

    IEnumerator ReverseMovementRoutine()
    {
        rb = this.transform.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        Debug.Log("check reverseMovementRoutine");
        transform.rotation = originalRotation;

        for (int i = positions.Count - 1; i >= 0; i--)
        {
            transform.position = positions[i];
            // 현재 위치에서 목표 위치까지 보간하여 부드럽게 이동
            //transform.position = Vector3.Lerp(transform.position, positions[i], (float)(positions.Count - i) / (positions.Count * 1)); // 보간 계수를 조정하여 이동 속도를 느리게 함
            yield return new WaitForSeconds(recordInterval);
        }
        rb.isKinematic = false;
        positions.Clear();
        Debug.Log("check finish return");
    }

    IEnumerator ReverseMovementRoutine()
    {
        rb = this.transform.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        Debug.Log("check reverseMovementRoutine");
        transform.rotation = originalRotation;

        float moveSpeed = 1.0f / recordInterval; // 이동 속도를 조절합니다. 필요에 따라 조정할 수 있습니다.

        for (int i = positions.Count - 1; i >= 0; i--)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = positions[i];
            float t = 0.0f;

            while (t < 1.0f)
            {
                t += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPosition, endPosition, t);
                yield return null;
            }
        }

        rb.isKinematic = false;
        positions.Clear();
        Debug.Log("check finish return");
    }


    public void StartRecord()
    {
        if (!isRecord)
        {
            isRecord = true;
            StartCoroutine(RecordPositionRoutine());
        }
        else
            StartReverseMovement();
    }

    public void StartReverseMovement()
    {
        isRecord = false;
        StartReverseMovementServerRpc();
    }
}
*/