using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;
    public TextMeshProUGUI chatText;
    public ScrollRect scrollRect;
    public RectTransform chatRectTransform;
    private Queue<string> messageQueue = new Queue<string>();
    private bool isMessageRunning = false;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMessage(string message)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        chatText.color = new Color(chatText.color.r, chatText.color.g, chatText.color.b, 1f); // 텍스트를 다시 선명하게
        messageQueue.Enqueue(message + "\n");

        if (!isMessageRunning)
        {
            StartCoroutine(ShowMessages());
        }

        fadeCoroutine = StartCoroutine(FadeOutText());
    }

    IEnumerator ShowMessages()
    {
        isMessageRunning = true;
        while (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();
            bool insideTag = false;

            for (int i = 0; i < message.Length; i++)
            {
                char letter = message[i];

                if (letter == '<')
                {
                    insideTag = true;  // 태그 시작
                }

                chatText.text += letter;

                if (!insideTag)
                {
                    yield return new WaitForSeconds(0.03f);  // 일반 문자 처리 속도
                    scrollRect.verticalNormalizedPosition = 0f; // 스크롤 위치를 맨 아래로 설정
                }

                if (letter == '>')
                {
                    insideTag = false;  // 태그 종료
                }
            }
        }
        isMessageRunning = false;
    }

    IEnumerator FadeOutText()
    {
        yield return new WaitForSeconds(20f); // 20초 대기

        float duration = 5f; // 5초 동안 점진적으로 투명하게
        float elapsed = 0f;

        Color originalColor = chatText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            chatText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
}
