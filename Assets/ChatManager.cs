using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;  // �� ���� �߰��ϼ���

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;  // �̱��� �ν��Ͻ�
    public TextMeshProUGUI chatText;     // ä��â�� �ؽ�Ʈ�� ǥ���� TextMeshProUGUI
    public ScrollRect scrollRect;        // ä��â ��ũ�ѹ� ��Ʈ���� ���� ScrollRect

    void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject);  // �ߺ� �ν��Ͻ� ����
        }
    }

    // ä�� �޽����� ä��â�� �߰��ϴ� �޼ҵ�
    public void AddMessage(string message)
    {
        chatText.text += message + "\n";
        StartCoroutine(AdjustScrollPosition());  // ��ũ�� ��ġ ���� �ڷ�ƾ ȣ��
    }

    // ��ũ�� ��ġ�� ���ϴ����� �����ϴ� �ڷ�ƾ
    IEnumerator AdjustScrollPosition()
    {
        yield return new WaitForEndOfFrame();  // ������ ���� ��ٸ�
        scrollRect.verticalNormalizedPosition = 0f;  // ��ũ���� ���ϴ����� �̵�
    }
}
