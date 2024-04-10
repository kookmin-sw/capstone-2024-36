using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public RectTransform chatScrollViewRectTransform; // Scroll View의 RectTransform
    private Vector2 originalSize; // 최초 크기 저장
    private bool isExpanded = false; // 확장 상태

    void Start()
    {
        originalSize = chatScrollViewRectTransform.sizeDelta; // 시작 시 크기 저장
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
            // 축소
            chatScrollViewRectTransform.sizeDelta = originalSize;
        }
        else
        {
            // 확장
            chatScrollViewRectTransform.sizeDelta = new Vector2(originalSize.x, originalSize.y * 2); // 높이를 두 배로
        }
        isExpanded = !isExpanded; // 상태 토글
    }
}