using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCredit : MonoBehaviour
{
    public RectTransform rectTransform;

    public float scrollSpeed;
    public float fastScrolSpeed;

    public string returnSceneName;

    private void Awake()
    {
        rectTransform.anchoredPosition = Vector3.zero;
    }

    private bool bLoadScene = false;

    // Update is called once per frame
    void Update()
    {
        if (rectTransform.anchoredPosition.y < rectTransform.rect.height)
        {
            float yScale = Time.deltaTime;
            if (Input.GetMouseButton(0))
            {
                yScale *= fastScrolSpeed;
            }
            else
            {
                yScale *= scrollSpeed;
            }

            rectTransform.anchoredPosition += Vector2.up * yScale;
        }
        else if (!bLoadScene)
        {
            bLoadScene = true;
            int idx = SceneUtility.GetBuildIndexByScenePath(returnSceneName);
            SceneManager.LoadScene(idx);
        }
    }
}
