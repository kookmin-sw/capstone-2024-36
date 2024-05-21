using System.Collections;
using UnityEngine;
using TMPro;

public class FlashingTextTrigger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI flashingText; // TextMeshPro 객체를 할당하기 위한 필드
    public float displayDuration = 10.0f; // 메시지 표시 시간
    public string message = "컨트롤 안내 메세지 : 화면 중앙에 x초간 깜빡거리며 나옴"; // 표시할 메시지
    public string triggerID; // 트리거를 구별할 ID
    private bool hasTriggered = false;

    private void Start()
    {
        if (flashingText != null)
            flashingText.gameObject.SetActive(false); // 게임 시작 시 텍스트 숨김

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
        if (other.CompareTag("PlayerForGravity") && !hasTriggered)
        {
            if (flashingText != null)
            {
                flashingText.text = message;
                StartCoroutine(DisplayFlashingText());
            }
        }
    }

    private IEnumerator DisplayFlashingText()
    {
        flashingText.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < displayDuration)
        {
            flashingText.color = new Color(flashingText.color.r, flashingText.color.g, flashingText.color.b, Mathf.PingPong(Time.time * 2, 1));
            elapsed += Time.deltaTime;
            yield return null;
        }

        flashingText.gameObject.SetActive(false);

        // 트리거 발생 여부 저장
        hasTriggered = true;
        PlayerPrefs.SetInt(triggerID, 1);
        PlayerPrefs.Save();
    }
}
