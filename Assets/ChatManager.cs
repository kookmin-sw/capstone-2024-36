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
    private Vector2 originalSize;
    private Queue<string> messageQueue = new Queue<string>();
    private bool isMessageRunning = false;

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
        messageQueue.Enqueue(message + "\n");
        if (!isMessageRunning)
        {
            StartCoroutine(ShowMessages());
        }
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

                if (! insideTag)
                {
                    yield return new WaitForSeconds(0.03f);  // 일반 문자 처리 속도
                    scrollRect.verticalNormalizedPosition = 0f;// 스크롤 위치를 맨 아래로 설정
                }

                if (letter == '>')
                {
                    insideTag = false;  // 태그 종료
                }
            }

        }
        isMessageRunning = false;
    }
}
