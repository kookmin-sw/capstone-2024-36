using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadManager : MonoBehaviour
{
    // DontDestroyOnLoad 상태에 있는 모든 루트 게임 오브젝트를 반환합니다.
    private static GameObject[] GetDontDestroyOnLoadObjects()
    {
        GameObject temp = new GameObject("TemporaryFinder");
        DontDestroyOnLoad(temp);
        Scene dontDestroyOnLoadScene = temp.scene;
        GameObject[] rootObjects = dontDestroyOnLoadScene.GetRootGameObjects();
        DestroyImmediate(temp);

        return rootObjects;
    }

    // DontDestroyOnLoad 상태의 ChatManager 인스턴스 찾기
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
        return null; // ChatManager가 발견되지 않은 경우
    }
}
