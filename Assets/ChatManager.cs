using UnityEngine;
using TMPro; // TextMeshPro�� ����ϱ� ���� ���ӽ����̽�
using UnityEngine.UI; // ScrollRect�� ����ϱ� ���� ���ӽ����̽�
using System.Collections; // �ڷ�ƾ ����� ���� ���ӽ����̽�

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;  // �̱��� �ν��Ͻ�
    public TextMeshProUGUI chatText;     // ä��â�� �ؽ�Ʈ�� ǥ���� TextMeshProUGUI
    public ScrollRect scrollRect;        // ��ũ�Ѻ��� ScrollRect ������Ʈ

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject);  // �ߺ� �ν��Ͻ��� �������� �ʵ��� ó��
        }
    }

    // ä�� �޽����� ä��â�� �߰��ϴ� �޼ҵ�
    public void AddMessage(string message)
    {
        // ä��â�� �ؽ�Ʈ�� �� �޽����� �߰�
        chatText.text += message + "\n";
        // ��ũ�� ��ġ ������ ���� �ڷ�ƾ ȣ��
        StartCoroutine(AdjustScrollPosition());
    }

    // ��ũ�� ��ġ�� �� �Ʒ��� �����ϴ� �ڷ�ƾ
    private IEnumerator AdjustScrollPosition()
    {
        yield return new WaitForSeconds(0.1f);  // ª�� ������ �� ����
        scrollRect.verticalNormalizedPosition = 0f;  // ��ũ���� �� �Ʒ��� ����
    }
}