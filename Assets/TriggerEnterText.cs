using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class TriggerEnterText : MonoBehaviour
{
    public TextMeshProUGUI existingText; // 기존의 TextMeshProUGUI 컴포넌트를 할당
    public string customMessage = "기본 메시지"; // 각 트리거마다 설정할 메시지
    public ScrollRect scrollRect; // 인스펙터에서 스크롤뷰의 ScrollRect 컴포넌트를 할당

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.gameObject.GetComponent<TPSCharacterController>() != null)
        {
            hasTriggered = true;
            existingText.text += "\n" + customMessage; // 메시지 추가

            // 스크롤 위치를 조정하기 전에 Canvas를 강제로 업데이트
            Canvas.ForceUpdateCanvases();
            // 스크롤을 맨 아래로 조정
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
