using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueEndingDoor : MonoBehaviour
{
    static bool hasKey = true;  // debug true

    public Transform doorTransform;
    public bool isPlayerInIt = false;

    public bool isOpenStarted = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        SaveFileManager.Instance.ShowCanvas("CEO's Room", "", ePortalType.TrueEnding);
        isPlayerInIt = true;
        if (hasKey)
        {
            SaveFileManager.Instance.SetHostColor(true);
            SaveFileManager.Instance.SetGuestColor(true);            
        }
        else
        {
            SaveFileManager.Instance.SetHostColor(false);
            SaveFileManager.Instance.SetGuestColor(false);
        }
            
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        SaveFileManager.Instance.HideCanvas();
        isPlayerInIt = false;
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E))
            return;
        if (!isPlayerInIt)
            return;
        if (isOpenStarted)
            return;

        isOpenStarted = true;
        SaveFileManager.Instance.HideCanvas();
        StartCoroutine(open());
    }

    public float zStart = -17.946f;
    public float zEnd = -16.09f;
    public float openSpeed = 1;
    IEnumerator open()
    {
        Debug.Log("open started");
        while (doorTransform.localPosition.z < zEnd)
        {
            doorTransform.localPosition += Vector3.forward * openSpeed * Time.deltaTime;
            yield return null;
        }
        Debug.Log("open end");
    }
}
