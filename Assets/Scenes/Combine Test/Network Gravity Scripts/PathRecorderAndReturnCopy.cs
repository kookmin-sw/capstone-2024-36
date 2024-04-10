using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class pathRecorderAndReturnCopy : NetworkBehaviour
{
    public float recordInterval = 0.01f; // 위치 기록 단위
    private NetworkList<Vector3> positions = new NetworkList<Vector3>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private bool isRecord = false;
    private Quaternion originalRotation;

    IEnumerator RecordPositionRoutine()
    {
        Debug.Log("Recording position...");
        float elapsedTime = 0f; 
        originalRotation = transform.rotation;

        while (elapsedTime < 10.0f) 
        {
            positions.Add(transform.position); 
            
            if (positions.Count > 10000)
            {
                positions.RemoveAt(0); 
            }
            else if (IsClient)
            {
                // 클라이언트인 경우 서버로 위치 정보를 전송
                SendPositionToServer(transform.position);
            }
            elapsedTime += recordInterval; 
            yield return new WaitForSeconds(recordInterval);
        }

        Debug.Log("Recording finish");
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
        ReverseMovement();
    }

    private void ReverseMovement()
    {
        StartCoroutine(ReverseMovementRoutine());
    }

    private IEnumerator ReverseMovementRoutine()
    {
        //이동 경로를 거꾸로 재생
        Quaternion originalRotation = transform.rotation;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        for (int i = positions.Count - 1; i >= 0; i--)
        {
            float moveSpeed = 1.0f / recordInterval;
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
        transform.rotation = originalRotation;
        Debug.Log("Reverse movement finish");
    }

    private void SendPositionToServer(Vector3 position)
    {

        // 클라이언트에서 서버로 위치 정보를 RPC로 전송
        ReceivePositionFromClientServerRpc(position);
    }

    [ServerRpc]
    private void ReceivePositionFromClientServerRpc(Vector3 position)
    {
        // 서버에서 클라이언트로부터 받은 위치 정보를 positions에 추가
        positions.Add(position); 
        if (positions.Count > 10000)
        {
            positions.RemoveAt(0); 
        }
    }

}
