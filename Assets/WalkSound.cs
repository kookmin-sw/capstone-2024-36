using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSound : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] public AudioClip Walk_Sound;
    [SerializeField] public AudioClip Run_Sound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void WalkStep()
    {
        AudioSource.PlayClipAtPoint(Walk_Sound, Camera.main.transform.position);
    }

    void RunStep()
    {
        AudioSource.PlayClipAtPoint(Run_Sound, Camera.main.transform.position);
    }
}
