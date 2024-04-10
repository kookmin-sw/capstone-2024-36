using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class TriggerEnterText : MonoBehaviour
{
    public TextMeshProUGUI existingText; // ������ TextMeshProUGUI ������Ʈ�� �Ҵ�
    public string customMessage = "�⺻ �޽���"; // �� Ʈ���Ÿ��� ������ �޽���
    public ScrollRect scrollRect; // �ν����Ϳ��� ��ũ�Ѻ��� ScrollRect ������Ʈ�� �Ҵ�

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.gameObject.GetComponent<TPSCharacterController>() != null)
        {
            hasTriggered = true;
            existingText.text += "\n" + customMessage; // �޽��� �߰�

            // ��ũ�� ��ġ�� �����ϱ� ���� Canvas�� ������ ������Ʈ
            Canvas.ForceUpdateCanvases();
            // ��ũ���� �� �Ʒ��� ����
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
