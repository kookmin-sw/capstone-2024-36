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
        if (other.CompareTag("Player") && !hasTriggered) // �÷��̾ Ʈ���ſ� ������
        {
            chatManager.AddMessage(message); // �޽��� ǥ��
            hasTriggered = true;
}
    }
}