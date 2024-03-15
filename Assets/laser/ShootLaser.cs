using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootLaser : MonoBehaviour
{
    public Color LaserColor = Color.red;
    public Material material;
    
    LaserBeam beam;
    // Update is called once per frame
    void Update()
    {
        Destroybeam();
        beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, LaserColor);
    }

    public void Destroybeam()
    {
        Destroy(GameObject.Find("Laser_Beam" + (int)(LaserColor.r*255) + (int)(LaserColor.g*255) + (int)(LaserColor.b*255)));
    }
}
