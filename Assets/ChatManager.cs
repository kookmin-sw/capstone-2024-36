using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스
using UnityEngine.UI; // ScrollRect를 사용하기 위한 네임스페이스
using System.Collections; // 코루틴 사용을 위한 네임스페이스

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;  // 싱글톤 인스턴스
    public TextMeshProUGUI chatText;     // 채팅창에 텍스트를 표시할 TextMeshProUGUI
    public ScrollRect scrollRect;        // 스크롤뷰의 ScrollRect 컴포넌트

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);  // 중복 인스턴스가 생성되지 않도록 처리
        }
    }

    // 채팅 메시지를 채팅창에 추가하는 메소드
    public void AddMessage(string message)
    {
        // 채팅창의 텍스트에 새 메시지를 추가
        chatText.text += message + "\n";
        // 스크롤 위치 조정을 위한 코루틴 호출
        StartCoroutine(AdjustScrollPosition());
    }

    // 스크롤 위치를 맨 아래로 조정하는 코루틴
    private IEnumerator AdjustScrollPosition()
    {
        yield return new WaitForSeconds(0.1f);  // 짧은 딜레이 후 실행
        scrollRect.verticalNormalizedPosition = 0f;  // 스크롤을 맨 아래로 조정
    }
}