using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string message; // 표시할 메시지
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 트리거에 들어오면 메시지 표시
        if ((other.CompareTag("PlayerForGravity") || other.CompareTag("moveable")) && !hasTriggered)
        {
            // DontDestroyOnLoad 상태의 ChatManager 인스턴스 찾기
            ChatManager chatManager = DontDestroyOnLoadManager.FindChatManagerInDontDestroyOnLoad();
            if (chatManager != null)
            {
                chatManager.AddMessage(message);
                hasTriggered = true;
            }
            else
            {
                Debug.LogError("ChatManager not found in DontDestroyOnLoad scene.");
            }
        }
    }
}
