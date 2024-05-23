using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterSwitch : MonoBehaviour
{
    [SerializeField] GameObject FilterSelect_1;
    [SerializeField] GameObject FilterSelect_2;
    [SerializeField] GameObject FilterSelect_3;
    [SerializeField] GameObject FilterSelect_4;
    [SerializeField] int NumSwitch = 3;
    [SerializeField] private KeyCode InteractKey;


    public static GameObject[] FilterList;

    // Start is called before the first frame update
    void Start()
    {
        FilterList = new GameObject[4];
        FilterList[0] = FilterSelect_1;
        FilterList[1] = FilterSelect_2;
        FilterList[2] = FilterSelect_3;
        FilterList[3] = FilterSelect_4;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(InteractKey))
        {
            FilterList[NumSwitch].SetActive(true);
            NumSwitch++;
            if (NumSwitch > 3)
                NumSwitch -= 4;
            FilterList[NumSwitch].SetActive(false);
        }
    }
}
