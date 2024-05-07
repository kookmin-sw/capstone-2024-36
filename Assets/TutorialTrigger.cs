using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string message; // ǥ���� �޽���
    public ChatManager chatManager; // ä�� �Ŵ��� ����
    private bool hasTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerForGravity") && !hasTriggered) // �÷��̾ Ʈ���ſ� ������
        {
            chatManager.AddMessage(message); // �޽��� ǥ��
            hasTriggered = true;
        }
        else if (other.CompareTag("moveable") && !hasTriggered)
        {
            chatManager.AddMessage(message); // �޽��� ǥ��
            hasTriggered = true;
        }
    }
}