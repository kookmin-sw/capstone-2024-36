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
    private Queue<string> messageQueue = new Queue<string>();
    private bool isMessageRunning = false;
    private Coroutine fadeCoroutine;
    private List<string> allMessages = new List<string>();

    void Start()
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            MakeTextOpaque();
        }
    }

    public void AddMessage(string message)
    {
        messageQueue.Enqueue(message + "\n");

        if (!isMessageRunning)
        {
            StartCoroutine(ShowMessages());
        }

        // 새로운 메시지가 추가될 때마다 이전 메시지들은 흐리게 처리
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutText());
    }

    IEnumerator ShowMessages()
    {
        isMessageRunning = true;
        chatText.text = ""; // 이전 메시지를 모두 지웁니다
        allMessages.Clear(); // 모든 메시지 리스트 초기화

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
                allMessages.Add(letter.ToString());

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
        yield return new WaitForSeconds(10f); // 15초 대기

        float duration = 3f; // 5초 동안 점진적으로 투명하게
        float elapsed = 0f;

        TMP_TextInfo textInfo = chatText.textInfo;
        Color32[] newVertexColors;
        Color32 c0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                c0 = newVertexColors[vertexIndex]; // 기존 색상 유지
                byte alphaByte = (byte)(alpha * 255); // 알파 값을 바이트로 변환
                c0 = new Color32(c0.r, c0.g, c0.b, alphaByte);
                newVertexColors[vertexIndex + 0] = c0;
                newVertexColors[vertexIndex + 1] = c0;
                newVertexColors[vertexIndex + 2] = c0;
                newVertexColors[vertexIndex + 3] = c0;
            }

            chatText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            yield return null;
        }
    }

    private void MakeTextOpaque()
    {
        // 모든 코루틴 중단
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // 텍스트를 다시 불투명하게 설정
        TMP_TextInfo textInfo = chatText.textInfo;
        Color32[] newVertexColors;
        Color32 c0;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            c0 = newVertexColors[vertexIndex]; // 기존 색상 유지
            c0 = new Color32(c0.r, c0.g, c0.b, 255); // 알파 값을 255로 설정하여 불투명하게
            newVertexColors[vertexIndex + 0] = c0;
            newVertexColors[vertexIndex + 1] = c0;
            newVertexColors[vertexIndex + 2] = c0;
            newVertexColors[vertexIndex + 3] = c0;
        }

        chatText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        // 페이드 아웃 코루틴 다시 시작
        fadeCoroutine = StartCoroutine(FadeOutText());
    }
}