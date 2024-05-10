using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadManager : MonoBehaviour
{
    // DontDestroyOnLoad ���¿� �ִ� ��� ��Ʈ ���� ������Ʈ�� ��ȯ�մϴ�.
    private static GameObject[] GetDontDestroyOnLoadObjects()
    {
        GameObject temp = new GameObject("TemporaryFinder");
        DontDestroyOnLoad(temp);
        Scene dontDestroyOnLoadScene = temp.scene;
        GameObject[] rootObjects = dontDestroyOnLoadScene.GetRootGameObjects();
        DestroyImmediate(temp);

        return rootObjects;
    }

    // DontDestroyOnLoad ������ ChatManager �ν��Ͻ� ã��
    public static ChatManager FindChatManagerInDontDestroyOnLoad()
    {
        GameObject[] rootObjects = GetDontDestroyOnLoadObjects();
        foreach (GameObject obj in rootObjects)
        {
            ChatManager chatManager = obj.GetComponentInChildren<ChatManager>(true);
            if (chatManager != null)
            {
                return chatManager;
            }
        }
        return null; // ChatManager�� �߰ߵ��� ���� ���
    }
}
