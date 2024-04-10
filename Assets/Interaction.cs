using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public RectTransform chatScrollViewRectTransform; // Scroll View�� RectTransform
    private Vector2 originalSize; // ���� ũ�� ����
    private bool isExpanded = false; // Ȯ�� ����

    void Start()
    {
        originalSize = chatScrollViewRectTransform.sizeDelta; // ���� �� ũ�� ����
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ToggleChatWindow();
        }
    }

    void ToggleChatWindow()
    {
        if (isExpanded)
        {
            // ���
            chatScrollViewRectTransform.sizeDelta = originalSize;
        }
        else
        {
            // Ȯ��
            chatScrollViewRectTransform.sizeDelta = new Vector2(originalSize.x, originalSize.y * 2); // ���̸� �� ���
        }
        isExpanded = !isExpanded; // ���� ���
    }
}