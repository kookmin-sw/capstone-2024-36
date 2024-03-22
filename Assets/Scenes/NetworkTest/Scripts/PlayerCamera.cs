using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private static PlayerCamera s_instance;

    public static PlayerCamera Instance() { return s_instance; }

    private void Awake()
    {
        if (s_instance == null)
            s_instance = this;
        else
            Destroy(gameObject);
    }
}
