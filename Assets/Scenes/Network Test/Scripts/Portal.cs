using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string NextSceneName;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("portal collision enter");
        if (collision.gameObject.CompareTag("Player"))
        {
            NetworkPlayer player = collision.gameObject.GetComponentInParent<NetworkPlayer>();
            if (player == null)
                return;

            if (player.IsOwner)
            {
                MyNetworkTransform playerTransform = player.GetComponent<MyNetworkTransform>();

                int idx = SceneUtility.GetBuildIndexByScenePath(NextSceneName);
                NetworkSceneManager.Instance.MoveScene(idx, playerTransform);
            }
        }
    }
}
