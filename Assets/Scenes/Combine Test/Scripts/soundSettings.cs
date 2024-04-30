using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class soundSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    [SerializeField] GameObject Obj;

    public void setVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        Debug.Log(volume);
    }

    public void setBrightness(float Brightness)
    {
        CameraController.Brightness = Brightness;
    }

    public void setMouseSpeed(float MSpeed)
    {
        CameraController.MouseSpeed = MSpeed;
    }
}
