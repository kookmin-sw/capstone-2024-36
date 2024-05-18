using System.Collections;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string message; // 표시할 메시지
    public float delayTime = 0f; // 메시지 지연 시간 (초), 인스펙터에서 설정 가능
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 트리거에 들어올 때
        if ((other.CompareTag("PlayerForGravity") || other.CompareTag("moveable")) && !hasTriggered)
        {
            // DontDestroyOnLoad 상태의 ChatManager 인스턴스를 찾음
            ChatManager chatManager = DontDestroyOnLoadManager.FindChatManagerInDontDestroyOnLoad();
            if (chatManager != null)
            {
                StartCoroutine(DelayedMessage(chatManager));
                hasTriggered = true;
            }
            else
            {
                Debug.LogError("ChatManager not found in DontDestroyOnLoad scene.");
            }
        }
    }

    private IEnumerator DelayedMessage(ChatManager chatManager)
    {
        // 설정된 시간만큼 대기
        yield return new WaitForSeconds(delayTime);
        // 메시지를 추가
        chatManager.AddMessage(message);
    }
}
