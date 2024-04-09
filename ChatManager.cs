using UnityEngine;

public class ExpandScrollView : MonoBehaviour
{
    private RectTransform scrollViewRect; // ��ũ�� ���� RectTransform
    private Vector2 originalSize; // ���� ũ��
    private Vector2 originalPosition; // ���� ��ġ
    private bool isExpanded = false; // Ȯ�� ���� ����

    void Start()
    {
        scrollViewRect = GetComponent<RectTransform>();
        originalSize = scrollViewRect.sizeDelta; // ���� �� ���� ũ�� ����
        originalPosition = scrollViewRect.anchoredPosition; // ���� �� ���� ��ġ ����
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ToggleScrollViewSize();
        }
    }

    void ToggleScrollViewSize()
    {
        if (isExpanded)
        {
            // ����� ��
            scrollViewRect.sizeDelta = originalSize; // ���� ũ��� ����
            scrollViewRect.anchoredPosition = originalPosition; // ���� ��ġ�� ����
        }
        else
        {
            // Ȯ���� ��
            scrollViewRect.sizeDelta = new Vector2(originalSize.x, originalSize.y * 2); // ���̸� 2��� ����
            scrollViewRect.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + originalSize.y / 2); // ��ġ�� ���� ����
        }
        isExpanded = !isExpanded;
    }
}