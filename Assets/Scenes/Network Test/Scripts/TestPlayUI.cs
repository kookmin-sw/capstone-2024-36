using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TestPlayUI : MonoBehaviour
{
    public void OnClickkDisconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public void OnClickSceneButton(int buildIndex)
    {
        NetworkSceneManager.Instance.LoadScene(buildIndex);
    }
}
