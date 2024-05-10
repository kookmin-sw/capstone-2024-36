using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;  // 이 줄을 추가하세요

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;  // 싱글톤 인스턴스
    public TextMeshProUGUI chatText;     // 채팅창에 텍스트를 표시할 TextMeshProUGUI
    public ScrollRect scrollRect;        // 채팅창 스크롤바 컨트롤을 위한 ScrollRect

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);  // 중복 인스턴스 제거
        }
    }

    // 채팅 메시지를 채팅창에 추가하는 메소드
    public void AddMessage(string message)
    {
        chatText.text += message + "\n";
        StartCoroutine(AdjustScrollPosition());  // 스크롤 위치 조정 코루틴 호출
    }

    // 스크롤 위치를 최하단으로 조정하는 코루틴
    IEnumerator AdjustScrollPosition()
    {
        yield return new WaitForEndOfFrame();  // 프레임 끝을 기다림
        scrollRect.verticalNormalizedPosition = 0f;  // 스크롤을 최하단으로 이동
    }
}
