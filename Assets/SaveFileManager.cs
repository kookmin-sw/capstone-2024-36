using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class StageClearInfo
{
    public void Load()
    {
        IsStage1Cleared = true;
        if (PlayerPrefs.GetInt("isStage1Cleared", 0) == 0)
        {
            IsStage1Cleared = false;
        }

        IsStage2Cleared = true;
        if (PlayerPrefs.GetInt("isStage2Cleared", 0) == 0)
        {
            IsStage2Cleared = false;
        }

        IsStage3Cleared = true;
        if (PlayerPrefs.GetInt("isStage3Cleared", 0) == 0)
        {
            IsStage3Cleared = false;
        }
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

    public void ShowCanvas(string stageName, string prefsName, bool isSimpleDoor)
    {
        StageNameText.text = stageName;
        
        int isCleared = PlayerPrefs.GetInt(prefsName, 0);
        if (isCleared == 0)
        {
            IsClearedText.text = "NO";
        }
        else
        {
            IsClearedText.text = "YES";
        }

        if (isSimpleDoor)
        {
            IsClearedUIText.enabled = false;
            IsClearedText.enabled = false;
        }
        else
        {
            IsClearedUIText.enabled = true;
            IsClearedText.enabled = true;
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
        else    // 내가 게스트인 경우
        {
            Player1Sprite.rectTransform.anchoredPosition = new Vector2(-80.0f, 0.0f);
            Player2Sprite.enabled = true;
            InfoText.text = "Waiting Host... ";
        }
    }
}
