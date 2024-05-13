using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using System.IO;
using System;

[System.Serializable]
public class StageClearInfo
{
    public void Load()
    {
        string contents = "";
        try
        {
            contents = File.ReadAllText("./saved.save");

        }
        catch (FileNotFoundException)
        {
            StageClearInfo info = new StageClearInfo();
            info.IsStage1Cleared = false;
            info.IsStage2Cleared = false;
            info.IsStage3Cleared = false;

            contents = JsonUtility.ToJson(info);

            try { 
                File.WriteAllText("./saved.save", contents);
            } catch(Exception e)
            {
                Debug.LogError(e);
            }
        }
        finally
        {
            StageClearInfo info = new StageClearInfo();

            try
            {
                info = JsonUtility.FromJson<StageClearInfo>(contents);
            }
            catch
            {
                // file corrupted
                IsStage1Cleared = false;
                IsStage2Cleared = false;
                IsStage3Cleared = false;

                string content = JsonUtility.ToJson(this);
                File.WriteAllText("./saved.save", content);
            }
            finally
            {
                IsStage1Cleared = info.IsStage1Cleared;
                IsStage2Cleared = info.IsStage2Cleared;
                IsStage3Cleared = info.IsStage3Cleared;
            }
            
        }
    }

    public void Save()
    {
        string content = JsonUtility.ToJson(this);
        File.WriteAllText("./saved.save", content);
    }

    public bool IsStage1Cleared;
    public bool IsStage2Cleared;
    public bool IsStage3Cleared;
}

public class SaveFileManager : SingletoneComponent<SaveFileManager>
{
    [Header("Reference")]
    public Canvas UICanvas;
    public TMP_Text StageNameText;
    public TMP_Text IsClearedUIText;
    public TMP_Text IsClearedText;
    public Image Player1Sprite;
    public Image Player2Sprite;
    public TMP_Text InfoText;

    private StageClearInfo m_stageClearInfo = new StageClearInfo();
    public StageClearInfo GetStageClearInfo()
    {
        return m_stageClearInfo;
    }

    public void ShowCanvas(string stageName, string prefsName, ePortalType portalType)
    {
        StageNameText.text = stageName;

        IsClearedText.text = "No";
        if (prefsName.Contains("1") && m_stageClearInfo.IsStage1Cleared)
        {
            IsClearedText.text = "Yes";
        }
        else if (prefsName.Contains("2") && m_stageClearInfo.IsStage2Cleared)
        {
            IsClearedText.text = "Yes";
        }
        else if (prefsName.Contains("3") && m_stageClearInfo.IsStage3Cleared)
        {
            IsClearedText.text = "Yes";
        }


        if (portalType == ePortalType.Stage)
        {
            IsClearedUIText.enabled = true;
            IsClearedText.enabled = true;
        }
        else if (portalType == ePortalType.Simple)
        {
            IsClearedUIText.enabled = false;
            IsClearedText.enabled = false;
        }
        else if (portalType == ePortalType.Stage)
        {
            IsClearedUIText.enabled = false;
            IsClearedText.enabled = false;
        }

        UICanvas.gameObject.SetActive(true);
    }

    public void HideCanvas()
    {
        UICanvas.gameObject.SetActive(false);
    }

    private void Awake()
    {
        RegisterInstance(this);
        m_stageClearInfo.Load();

        HideCanvas();
    }

    public void SetHostColor(bool isGreen)
    {
        if (isGreen)
        {
            Player1Sprite.color = Color.green;
        }
        else
        {
            Player1Sprite.color = Color.red;
        }
    }

    public void SetGuestColor(bool isGreen)
    {
        if (isGreen)
        {
            Player2Sprite.color = Color.green;
        }
        else
        {
            Player2Sprite.color = Color.red;
        }
    }

    public void SetColor(bool hostGreen, bool guestGreen)
    {
        SetHostColor(hostGreen);
        SetGuestColor(guestGreen);
    }

    private void Update()
    {
        if (!UICanvas.enabled)
            return;

        if (NetworkManager.Singleton.IsServer)
        {
            int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
            if (playerCount == 1)
            {
                Player1Sprite.rectTransform.anchoredPosition = Vector2.zero;
                Player2Sprite.enabled = false;
                InfoText.text = "Press E To Enter";
            }
            else if (playerCount == 2)
            {
                Player1Sprite.rectTransform.anchoredPosition = new Vector2(-80.0f, 0.0f);
                Player2Sprite.enabled = true;

                if (Player1Sprite.color == Color.green && Player2Sprite.color == Color.green)
                {
                    InfoText.text = "Press E To Enter";
                }
                else
                {
                    InfoText.text = "Waiting Other Player";
                }
            }
        }
        else   
        {
            Player1Sprite.rectTransform.anchoredPosition = new Vector2(-80.0f, 0.0f);
            Player2Sprite.enabled = true;
            InfoText.text = "Waiting Host... ";
        }
    }
}
