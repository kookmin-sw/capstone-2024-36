using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserBeam
{
    Vector3 pos, dir;
    GameObject laserObject;
    public LineRenderer laser;
    public List<Vector3> laserIndices = new List<Vector3>();
    Color laserColor;

    public LaserBeam(Vector3 pos, Vector3 dir, Material material, Color color)
    {
        this.laserColor = color;

        this.laserObject = new GameObject();
        string name = "Laser_Beam" + (int)(color.r *255 ) + (int)(color.g * 255) + (int)(color.b * 255);
        this.laserObject.name = "Laser_Beam" + (int)(color.r *255 ) + (int)(color.g * 255) + (int)(color.b * 255);
        this.laserObject.tag = "Laser";

        this.laserObject.AddComponent<BoxCollider>();
        this.laserObject.GetComponent<BoxCollider>().isTrigger = true;

        this.pos = pos;
        this.dir = dir;

        this.laser = this.laserObject.AddComponent<LineRenderer>();
        this.laser.startWidth = 0.1f;
        this.laser.endWidth = 0.1f;
        this.laser.material = material;
        this.laser.startColor = color;
        this.laser.endColor = color;

        CastRay(pos, dir,laser);
    }
    public LaserBeam(Vector3 pos, Vector3 dir, Material material, Color color, string name){
        this.laserColor = color;

        this.laserObject = new GameObject();
        this.laserObject.name = name;
        this.laserObject.tag = "Laser";

        this.pos = pos;
        this.dir = dir;

        this.laser = this.laserObject.AddComponent<LineRenderer>();
        this.laser.startWidth = 0.1f;
        this.laser.endWidth = 0.1f;
        this.laser.material = material;
        this.laser.startColor = color;
        this.laser.endColor = color;
        
        CastRay(pos, dir, laser, name);
    }
    public void CastRay(Vector3 pos, Vector3 dir, LineRenderer laser, string name)
    {
        laserIndices.Add(pos);
        this.laser = laser;

        Ray ray = new Ray(pos, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 30))
        {
            CheckHit(hit, dir);
        }
        else
        {
            laserIndices.Add(ray.GetPoint(30));
            UpdateLaser();
        }
        
    }

    public void CastRay(Vector3 pos, Vector3 dir, LineRenderer laser)
    {
        laserIndices.Add(pos);
        this.laser = laser;

        Ray ray = new Ray(pos, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 30))
        {
            CheckHit(hit, dir);
        }
        else
        {
            laserIndices.Add(ray.GetPoint(30));
            UpdateLaser();
        }
    }

    void UpdateLaser()
    {
        int count = 0;
        laser.positionCount = laserIndices.Count;

        foreach (Vector3 idx in laserIndices)
        {
            laser.SetPosition(count++, idx);
        }
    }

    void CheckHit(RaycastHit hitInfo, Vector3 direction)
    {
        if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Mirror"))
        {
            Vector3 hitPoint = hitInfo.point;
            Vector3 reflectDir = Vector3.Reflect(direction, hitInfo.normal);

            CastRay(hitPoint, reflectDir,laser);
        }
        else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            if( this.laser.name != "laserability"){
            Renderer hitRenderer = hitInfo.collider.GetComponent<Renderer>();
            WallColor wallc = hitInfo.collider.GetComponent<WallColor>();
            Color wallColor = wallc.incolor;

            Color mixedColor = (wallColor + laserColor) /2f; // 색상 혼합
            wallc.SetinColor(mixedColor);
            wallc.SetoutColor();
            if(wallc.incolor.r == 1 || wallc.incolor.g ==1 || wallc.incolor.b ==1){
                wallc.outcolor = wallc.incolor;
            }
            hitRenderer.material.color = wallc.outcolor;
            }
            laserIndices.Add(hitInfo.point);
            UpdateLaser();
        }
        else
        {
            laserIndices.Add(hitInfo.point);
            UpdateLaser();
        }
        if (hitInfo.collider.CompareTag("LaserPoint") && this.laser.name == "laserability")
        {
            hitInfo.collider.GetComponent<ShootLaser>().LaserColor = laserColor;
            hitInfo.collider.GetComponent<ShootLaser>().Onofflaser();
        }
    }
    Color MixColors(Color color1, Color color2)
{
    // 각 채널의 값이 255가 되도록 조정하여 최종 색상 계산
    float red = Mathf.Max(color1.r, color2.r);
    float green = 0;
    float blue = Mathf.Max(color1.b, color2.b);
    
    Color mixedColor = new Color(red, green, blue);
    
    return mixedColor;
}
}
