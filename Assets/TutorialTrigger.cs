using System.Collections;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string message; // 표시할 메시지
    public float messageDelay = 0f; // 메시지 지연 시간
    public string triggerID; // 트리거를 구별할 ID
    private bool hasTriggered = false;

    private void Start()
    {
        // 트리거 ID가 없으면 게임 오브젝트 이름으로 설정
        if (string.IsNullOrEmpty(triggerID))
        {
            triggerID = gameObject.name;
        }

        // 트리거가 이미 발생했는지 확인
        hasTriggered = PlayerPrefs.GetInt(triggerID, 0) == 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("PlayerForGravity") || other.CompareTag("moveable")) && !hasTriggered)
        {
            StartCoroutine(DisplayMessage());
        }
    }

    private IEnumerator DisplayMessage()
    {
        yield return new WaitForSeconds(messageDelay);

        // DontDestroyOnLoad 상태의 ChatManager 인스턴스 찾기
        ChatManager chatManager = DontDestroyOnLoadManager.FindChatManagerInDontDestroyOnLoad();
        if (chatManager != null)
        {
            chatManager.AddMessage(message);
            hasTriggered = true;
            PlayerPrefs.SetInt(triggerID, 1); // 트리거 발생 여부 저장
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogError("ChatManager not found in DontDestroyOnLoad scene.");
        }
    }
}
