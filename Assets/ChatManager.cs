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
    private bool isChatExpanded = false; // 채팅창 확장 상태를 추적하는 변수

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
        originalSize = chatRectTransform.sizeDelta; // 원래 채팅창 크기를 저장
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
            foreach (char letter in message)
            {
                chatText.text += letter;
                yield return new WaitForSeconds(0.03f);
            }
            yield return new WaitForEndOfFrame();
            scrollRect.verticalNormalizedPosition = 0f;
        }
        isMessageRunning = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ToggleChatWindowSize();
        }
    }

    private void ToggleChatWindowSize()
    {
        if (!isChatExpanded)
        {
            chatRectTransform.sizeDelta = new Vector2(originalSize.x, originalSize.y * 2);
            isChatExpanded = true;
        }
        else
        {
            chatRectTransform.sizeDelta = originalSize;
            isChatExpanded = false;
        }
    }
}
