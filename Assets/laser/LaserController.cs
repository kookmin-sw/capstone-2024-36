using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public GameObject redLaser;
    public GameObject blueLaser;
    public GameObject greenLaser;
    private int currentLaserIndex = 0;

    private void Start()
    {
        redLaser.SetActive(true);
        blueLaser.SetActive(false);
        greenLaser.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleLaser();
        }
    }

    void ToggleLaser()
    {
        currentLaserIndex++;
        if (currentLaserIndex > 6) 
        {
            currentLaserIndex = 0;
        }
        switch (currentLaserIndex)
        {
            case 0:
                blueLaser.GetComponent<ShootLaser>().Destroybeam();
                greenLaser.GetComponent<ShootLaser>().Destroybeam();
                greenLaser.SetActive(false);
                blueLaser.SetActive(false);
                redLaser.SetActive(true);
                break;
            case 1:
                redLaser.GetComponent<ShootLaser>().Destroybeam();
                redLaser.SetActive(false);
                blueLaser.SetActive(true);
                break;
            case 2:
                blueLaser.GetComponent<ShootLaser>().Destroybeam();
                blueLaser.SetActive(false);
                greenLaser.SetActive(true);
                break;
            case 3:
                greenLaser.GetComponent<ShootLaser>().Destroybeam();
                greenLaser.SetActive(false);
                redLaser.SetActive(true);
                blueLaser.SetActive(true);
                break;
            case 4:
                blueLaser.GetComponent<ShootLaser>().Destroybeam();
                blueLaser.SetActive(false);
                greenLaser.SetActive(true);
                break;
            case 5:
                redLaser.GetComponent<ShootLaser>().Destroybeam();
                redLaser.SetActive(false);
                blueLaser.SetActive(true);
                break;
            case 6:
                redLaser.SetActive(true);
                break;
        }
    }
}

