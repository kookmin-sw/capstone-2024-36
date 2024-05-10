using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserManger : MonoBehaviour
{
    public List<GameObject> lasers;
    public GameObject wall23;
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
            // if(currentLaserIndex ==0){
            //     wall23.GetComponent<WallColor>().ColorOFF();
            // }
            laserPointer.Shootlaser();
        }
        else if (laserPointer != null && currentLaser.activeSelf == false){
            laserPointer.beam.Offcollider();
        }

        currentLaserIndex++; // 다음 레이저로 인덱스 이동
    }
    else
    {
        currentLaserIndex = 0; // 리스트의 범위를 벗어나면 다시 0으로 설정하여 처음부터 다시 시작
    }
    }
}
