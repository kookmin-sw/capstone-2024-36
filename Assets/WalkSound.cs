using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSound : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] public AudioClip Walk_Sound;
    [SerializeField] public AudioClip Run_Sound;

    void WalkStep()
    {
        AudioSource.PlayClipAtPoint(Walk_Sound, Camera.main.transform.position);
    }

    void RunStep()
    {
        AudioSource.PlayClipAtPoint(Run_Sound, Camera.main.transform.position);
    }
}
