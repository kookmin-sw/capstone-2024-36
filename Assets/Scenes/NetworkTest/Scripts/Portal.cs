using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public int nextSceneIndex;

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

                NetworkSceneManager.Instance.MoveScene(nextSceneIndex, playerTransform);
            }
        }
    }
}
