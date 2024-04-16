using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class CoopPortal : MonoBehaviour
{
    public static CoopPortal current;

    [Header("Status")]
    public bool IsLocalPlayerIn = false;
    public bool IsRemotePlayerIn = false;

    [Header("Setting")]
    public bool IsSimpleDoor;
    public string StageName;
    public string PrefsName;
    public string NextSceneName;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        NetworkPlayer player = collision.gameObject.GetComponentInParent<NetworkPlayer>();
        Debug.Log(collision.gameObject.name);
        if (player == null)
        {
            Debug.LogError("player is NULL!");
            return;
        }

        if (player.IsLocalPlayer)
        {
            current = this;

            if (NetworkManager.Singleton.IsServer)
            {
                SaveFileManager.Instance.SetHostColor(true);
                SaveFileManager.Instance.SetGuestColor(IsRemotePlayerIn);
            }
            else
            {
                SaveFileManager.Instance.SetHostColor(IsRemotePlayerIn);
                SaveFileManager.Instance.SetGuestColor(true);
            }

            SaveFileManager.Instance.ShowCanvas(StageName, PrefsName, IsSimpleDoor);
            IsLocalPlayerIn = true;
        }
        else
        {
            IsRemotePlayerIn = true;

            if (this != current)
            {
                return;
            }

            if (NetworkManager.Singleton.IsServer)
            {
                SaveFileManager.Instance.SetGuestColor(true);
            }
            else
            {
                SaveFileManager.Instance.SetHostColor(true);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        NetworkPlayer player = collision.gameObject.GetComponentInParent<NetworkPlayer>();
        Debug.Log(collision.gameObject.name);
        if (player == null)
        {
            Debug.LogError("player is NULL!");
            return;
        }

        if (player.IsLocalPlayer)
        {
            current = null;

            SaveFileManager.Instance.HideCanvas();
            IsLocalPlayerIn = false;
        }
        else
        {
            if (this != current)
                return;

            if (NetworkManager.Singleton.IsServer)
            {
                SaveFileManager.Instance.SetGuestColor(false);
            }
            else
            {
                SaveFileManager.Instance.SetHostColor(false);
            }

            IsRemotePlayerIn = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (current != this)
                return;

            if (NetworkManager.Singleton.IsHost)
            {
                int playerCount = NetworkManager.Singleton.ConnectedClients.Count;

                bool moveScene = false;

                if (IsSimpleDoor)
                {
                    if (playerCount == 1 && IsLocalPlayerIn)
                    {
                        moveScene = true;
                    }

                    if (playerCount == 2 && (IsLocalPlayerIn && IsRemotePlayerIn))
                    {
                        moveScene = true;
                    }
                }
                else
                {
                    // TODO: 각 유저의 세이브 기록 확인
                    if (playerCount == 1 && IsLocalPlayerIn)
                    {
                        moveScene = true;
                    }

                    if (playerCount == 2 && (IsLocalPlayerIn && IsRemotePlayerIn))
                    {
                        moveScene = true;
                    }
                }

                if (moveScene)
                {
                    int sceneIndex = SceneUtility.GetBuildIndexByScenePath(NextSceneName);
                    NetworkSceneManager.Instance.MoveSceneWithEveryPlayersRPC(sceneIndex);
                }
            }
        }
    }
}
