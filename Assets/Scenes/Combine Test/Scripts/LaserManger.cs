using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserManger : MonoBehaviour
{
    public List<GameObject> lasers;
    private int currentLaserIndex = 0; // 현재 레이저 인덱스
    NetworkLaserPointerShoot laserPointer;
    // Start is called before the first frame update
    void Start()
    {
        lasers = new List<GameObject>();
        

    }

    // Update is called once per frame
    void Update()
    {
        if (currentLaserIndex < lasers.Count)
    {
        GameObject currentLaser = lasers[currentLaserIndex];
        laserPointer = currentLaser.GetComponent<NetworkLaserPointerShoot>();

        if (laserPointer != null && currentLaser.activeSelf)
        {
            laserPointer.Shootlaser();
        }

        currentLaserIndex++; // 다음 레이저로 인덱스 이동
    }
    else
    {
        currentLaserIndex = 0; // 리스트의 범위를 벗어나면 다시 0으로 설정하여 처음부터 다시 시작
    }
    }
}
