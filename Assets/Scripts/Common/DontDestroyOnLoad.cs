using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public bool yes = true;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
