using UnityEngine;

public class ExpandScrollView : MonoBehaviour
{
    private RectTransform scrollViewRect; // 스크롤 뷰의 RectTransform
    private Vector2 originalSize; // 원래 크기
    private Vector2 originalPosition; // 원래 위치
    private bool isExpanded = false; // 확장 상태 여부

    void Start()
    {
        scrollViewRect = GetComponent<RectTransform>();
        originalSize = scrollViewRect.sizeDelta; // 시작 시 원래 크기 저장
        originalPosition = scrollViewRect.anchoredPosition; // 시작 시 원래 위치 저장
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
            // 축소할 때
            scrollViewRect.sizeDelta = originalSize; // 원래 크기로 설정
            scrollViewRect.anchoredPosition = originalPosition; // 원래 위치로 설정
        }
        else
        {
            // 확장할 때
            scrollViewRect.sizeDelta = new Vector2(originalSize.x, originalSize.y * 2); // 높이를 2배로 설정
            scrollViewRect.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + originalSize.y / 2); // 위치를 위로 조정
        }
        isExpanded = !isExpanded;
    }
}