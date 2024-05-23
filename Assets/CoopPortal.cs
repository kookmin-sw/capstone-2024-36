using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public enum ePortalType
{
    Simple,
    Stage,
    Clear,
    Ending
}


public class CoopPortal : MonoBehaviour
{
    public static CoopPortal current;

    [Header("Status")]
    public bool IsLocalPlayerIn = false;
    public bool IsRemotePlayerIn = false;

    [Header("Setting")]
    public ePortalType portalType;
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

            SaveFileManager.Instance.ShowCanvas(StageName, PrefsName, portalType);
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

                // 이동 여부 결정
                if (portalType == ePortalType.Simple || portalType == ePortalType.Clear)
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
                else if (portalType == ePortalType.Stage)
                {
                    StageClearInfo info = SaveFileManager.Instance.GetStageClearInfo();
                    if (PrefsName.Contains('1'))
                    {
                        moveScene = true;
                    }
                    else if (PrefsName.Contains('2') && info.IsStage1Cleared)
                    {
                        moveScene = true;
                    }
                    else if (PrefsName.Contains('3') && info.IsStage2Cleared)
                    {
                        moveScene = true;
                    }
                }
                else if (portalType == ePortalType.Ending)
                {
                    StageClearInfo info = SaveFileManager.Instance.GetStageClearInfo();
                    if (info.IsStage1Cleared && info.IsStage2Cleared && info.IsStage3Cleared)
                    {
                        moveScene = true;
                    }
                }

                if (moveScene)
                {
                    if (portalType == ePortalType.Clear)
                    {
                        StageClearInfo info = SaveFileManager.Instance.GetStageClearInfo();
                        if (PrefsName.Contains('1'))
                        {
                            info.IsStage1Cleared = true;
                        }
                        else if (PrefsName.Contains('2'))
                        {
                            info.IsStage2Cleared = true;
                        }
                        else if (PrefsName.Contains('3'))
                        {
                            info.IsStage3Cleared = true;
                        }

                        info.Save();
                    }

                    if (portalType != ePortalType.Ending)
                    {
                        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(NextSceneName);
                        NetworkSceneManager.Instance.MoveSceneWithEveryPlayersRPC(sceneIndex);
                    }
                    else
                    {
                        GameObject.Destroy(ChatManager.Instance.transform.parent.gameObject);

                        NetworkSceneManager.bSceneLoadOnDisconnect = false;
                        NetworkManager.Singleton.Shutdown();
                        SaveFileManager.Instance.HideCanvas();
                        SceneManager.LoadScene(NextSceneName);
                    }
                }
            }
        }
    }
}
