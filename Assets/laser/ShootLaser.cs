using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootLaser : MonoBehaviour
{
    public Color LaserColor = Color.red;
    public Material material;
    
    bool isLaserActive = true;
    LaserBeam beam;

    void Start(){
       beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, LaserColor);
    
    }
    // Update is called once per frame
    void Update()
    {
        // if(isLaserActive){
        
        // beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, LaserColor);
        // }
       if(beam != null && isLaserActive == true){
        beam.laser.positionCount = 0;
        beam.laserIndices.Clear();
        beam.CastRay(transform.position, transform.forward, beam.laser);}

    
    }

    public void Destroybeam()
    {
        Destroy(GameObject.Find("Laser_Beam" + (int)(LaserColor.r*255) + (int)(LaserColor.g*255) + (int)(LaserColor.b*255)));
    }

    public void Onofflaser(){
        if(isLaserActive){
            isLaserActive = false;
            Destroybeam();
        }
        else{
            isLaserActive = true;
             beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, LaserColor);
        }
    }
}
