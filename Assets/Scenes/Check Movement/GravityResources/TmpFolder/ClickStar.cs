using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickStars : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed = 5f;

    private Vector3 firstStarPosition;
    private Vector3 secondStarPosition;
    private bool firstStarClicked = false;
    private bool isMoving = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("stars"))
            {
                // 첫 번째 "stars" 오브젝트가 클릭된 경우
                if (!firstStarClicked)
                {
                    firstStarPosition = hit.transform.position;
                    firstStarClicked = true;
                    isMoving = true; // 첫 번째 위치로 이동 시작
                }
                // 두 번째 "stars" 오브젝트가 클릭된 경우
                else
                {
                    secondStarPosition = hit.transform.position;
                    isMoving = true; // 두 오브젝트의 중간 위치로 이동 시작
                }
            }
        }

        if (isMoving)
        {
            Vector3 targetPosition;
            if (firstStarClicked && secondStarPosition != Vector3.zero)
            {
                // 두 오브젝트의 중간 위치 계산
                targetPosition = (firstStarPosition + secondStarPosition) / 2;
            }
            else
            {
                // 첫 번째 오브젝트 위치
                targetPosition = firstStarPosition;
            }

            MovePlayerTowards(targetPosition);
        }
    }

    void MovePlayerTowards(Vector3 targetPos)
    {
        Vector3 directionToMove = (targetPos - player.transform.position).normalized;
        player.transform.Translate(directionToMove * moveSpeed * Time.deltaTime, Space.World);

        // 목표 위치에 가까워졌는지 확인
        if (Vector3.Distance(player.transform.position, targetPos) < 0.1f)
        {
            isMoving = false;
            // 두 번째 오브젝트에 도달했다면 초기화
            if (secondStarPosition != Vector3.zero)
            {
                firstStarClicked = false;
                secondStarPosition = Vector3.zero;
            }
        }
    }
}
