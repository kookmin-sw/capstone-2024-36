using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackground : MonoBehaviour
{
    public GameObject[] prisoners;

    public float minX = -23;
    public float maxX = 23;

    public int minWalkSpeed = 2;
    public int maxWalkSpeed = 4;

    public int spawnSecMin = 3;
    public int spawnSecMax = 7;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float leftSecToSpawn = 0;

    // Update is called once per frame
    void Update()
    {
        if (leftSecToSpawn > 0)
        {
            leftSecToSpawn -= Time.deltaTime;
        }
        else
        {
            leftSecToSpawn = Random.Range(spawnSecMin, spawnSecMax);

            int rand_idx = Random.Range(0, prisoners.Length - 1);
            GameObject prisonerGo = Instantiate(prisoners[rand_idx]);
            MenuPrisoner menuPrisoner = prisonerGo.GetComponent<MenuPrisoner>();
            menuPrisoner.transform.parent = transform;

            int random_z = Random.Range(154, 171);

            int random_walk_speed = Random.Range(minWalkSpeed, maxWalkSpeed);
            menuPrisoner.walkSpeed = random_walk_speed;
            Debug.Log(random_walk_speed);

            menuPrisoner.minX = minX;
            menuPrisoner.maxX = maxX;

            int i_bRand = Random.Range(0, 2);
            if (i_bRand == 0)
            {
                menuPrisoner.isLeft = true;
                menuPrisoner.transform.localPosition = new Vector3(maxX - 0.05f, 0, random_z);
                menuPrisoner.transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            else
            {
                menuPrisoner.isLeft = false;
                menuPrisoner.transform.localPosition = new Vector3(minX + 0.05f, 0, random_z);
                menuPrisoner.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
    }
}
