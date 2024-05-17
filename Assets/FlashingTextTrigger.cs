using System.Collections;
using UnityEngine;
using TMPro;

public class FlashingTextTrigger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI flashingText; // TextMeshPro 객체를 할당하기 위한 필드
    public float displayDuration = 10.0f; // 메시지 표시 시간
    public string message = "컨트롤 안내 메세지 : 화면 중앙에 x초간 깜빡거리며 나옴"; // 표시할 메시지
    public float onTime = 0.7f; // "켜짐" 상태의 시간 비율
    public float offTime = 0.3f; // "꺼짐" 상태의 시간 비율
    public float fadeDuration = 0.2f; // 페이드 인/아웃 시간

    private void Start()
    {
        if (flashingText != null)
            flashingText.gameObject.SetActive(false); // 게임 시작 시 텍스트 숨김
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerForGravity"))
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
            // 페이드 인
            yield return StartCoroutine(FadeTextToAlpha(1f));

            // "켜짐" 상태 유지
            yield return new WaitForSeconds(onTime);

            // 페이드 아웃
            yield return StartCoroutine(FadeTextToAlpha(0f));

            // "꺼짐" 상태 유지
            yield return new WaitForSeconds(offTime);

            elapsed += onTime + offTime + 2 * fadeDuration;
        }

        flashingText.gameObject.SetActive(false);
    }

    private IEnumerator FadeTextToAlpha(float targetAlpha)
    {
        float startAlpha = flashingText.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            flashingText.color = new Color(flashingText.color.r, flashingText.color.g, flashingText.color.b, newAlpha);
            yield return null;
        }

        flashingText.color = new Color(flashingText.color.r, flashingText.color.g, flashingText.color.b, targetAlpha);
    }
}
