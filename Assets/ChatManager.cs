using UnityEngine;
using TMPro; // TextMeshPro�� ����ϱ� ���� ���ӽ����̽�

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;  // �̱��� �ν��Ͻ�
    public TextMeshProUGUI chatText;     // ä��â�� �ؽ�Ʈ�� ǥ���� TextMeshProUGUI

    void Awake()
    {
        /*
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
        */
    }

    // ä�� �޽����� ä��â�� �߰��ϴ� �޼ҵ�
    public void AddMessage(string message)
    {
        // ä��â�� �ؽ�Ʈ�� �� �޽����� �߰�
        chatText.text += message + "\n";
    }
}