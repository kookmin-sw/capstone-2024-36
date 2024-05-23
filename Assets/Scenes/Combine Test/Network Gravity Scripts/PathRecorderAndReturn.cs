/*
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class PathRecorderAndReturn : NetworkBehaviour
{
    public float recordInterval = 0.01f;
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
            if (IsClient)
                AddPositionToServerRpc(transform.position);
                 
            if (positions.Count > 10000)
            {
                positions.RemoveAt(0); 
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

    [ServerRpc(RequireOwnership = false)]
    void AddPositionToServerRpc(Vector3 position)
    {
        positions.Add(position); // 서버의 위치 리스트에 위치 추가
    }

    [ClientRpc]
    void ReceivePositionsFromServer(List<Vector3> receivedPositions)
    {
        foreach(Vector3 position in receivedPositions) 
        {
            positions.Add(position); 
        }
        ReverseMovement();
    }

    [ServerRpc(RequireOwnership = false)]
    void SendPositionsToClientsServerRpc(List<Vector3> positions)
    {
        ReceivePositionsFromServer(positions); // 클라이언트에게 위치 목록을 보냄
    }
}
*/