using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string message; // 표시할 메시지
    public ChatManager chatManager; // 채팅 매니저 참조
    private bool hasTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered) // 플레이어가 트리거에 들어오면
        {
            chatManager.AddMessage(message); // 메시지 표시
            hasTriggered = true;
}
    }
}