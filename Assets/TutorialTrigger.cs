using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string message; // ǥ���� �޽���
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾ Ʈ���ſ� ������ �޽��� ǥ��
        if ((other.CompareTag("PlayerForGravity") || other.CompareTag("moveable")) && !hasTriggered)
        {
            // DontDestroyOnLoad ������ ChatManager �ν��Ͻ� ã��
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
