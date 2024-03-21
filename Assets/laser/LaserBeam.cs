using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserBeam
{
    Vector3 pos, dir;
    GameObject laserObject;
    LineRenderer laser;
    List<Vector3> laserIndices = new List<Vector3>();
    Color laserColor;
    private Color previousLaserColor;
    private bool collidedWithOtherLaser = false;

    public LaserBeam(Vector3 pos, Vector3 dir, Material material, Color color)
    {
        previousLaserColor = color;
        this.laserColor = color;

        this.laserObject = new GameObject();
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

        CastRay(pos, dir);
    }

    void CastRay(Vector3 pos, Vector3 dir)
    {
        laserIndices.Add(pos);

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

            CastRay(hitPoint, reflectDir);
        }
        else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Renderer hitRenderer = hitInfo.collider.GetComponent<Renderer>();
            Color wallColor = hitRenderer.material.color;
            Debug.Log(wallColor+"벽색깔");
            Debug.Log(laserColor+"레이저색깔");

             Color mixedColor = (wallColor + laserColor) /2f; // 색상 혼합

            hitRenderer.material.color = mixedColor;
            laserIndices.Add(hitInfo.point);
            UpdateLaser();
        }
        else
        {
            laserIndices.Add(hitInfo.point);
            UpdateLaser();
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
