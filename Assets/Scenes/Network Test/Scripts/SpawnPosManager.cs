using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: 해당 위치에 이미 캐릭터가 있으면? 
public class SpawnPosManager : SingletoneComponent<SpawnPosManager>
{
    public bool isGuestSpawnPosFound;
    public bool isHostSpawnPosFound;

    [HideInInspector] public Transform HostSpawnPos;
    [HideInInspector] public Transform GuestSpawnPos;

    private void Awake()
    {
        RegisterInstance(this);

        setupReference();

        SceneManager.activeSceneChanged += sceneManager_activeSceneChanged;
    }

    private void sceneManager_activeSceneChanged(Scene oldScene, Scene newScene)
    {
        setupReference();
    }

    private void setupReference()
    {
        isHostSpawnPosFound = false;
        isGuestSpawnPosFound = false;

        GameObject manSpawnPosGo = GameObject.FindGameObjectWithTag("Man Spawn Pos");
        if (manSpawnPosGo != null)
        {
            HostSpawnPos = manSpawnPosGo.transform;
            isHostSpawnPosFound = true;
        }
        else
        {
            // Debug.LogError("GameObject with Man Spawn Pos not found. ");
        }

        GameObject womanSpawnPosGo = GameObject.FindGameObjectWithTag("Woman Spawn Pos");
        if (womanSpawnPosGo != null)
        {
            GuestSpawnPos = womanSpawnPosGo.transform;
            isGuestSpawnPosFound = true;
        }
        else
        {
            // Debug.LogError("GameObject with Woman Spawn Pos not found. ");
        }
    }
}
