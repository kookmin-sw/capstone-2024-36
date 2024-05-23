using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class TestPlayUI : MonoBehaviour
{
    public void OnClickkDisconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public void OnClickSceneButton(string sceneName)
    {
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
        NetworkSceneManager.Instance.LoadScene(sceneIndex);
    }
}
