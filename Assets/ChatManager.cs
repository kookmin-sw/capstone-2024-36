using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;  // 싱글톤 인스턴스
    public TextMeshProUGUI chatText;     // 채팅창에 텍스트를 표시할 TextMeshProUGUI

    void Awake()
    {
        /*
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
        */
    }

    // 채팅 메시지를 채팅창에 추가하는 메소드
    public void AddMessage(string message)
    {
        // 채팅창의 텍스트에 새 메시지를 추가
        chatText.text += message + "\n";
    }
}