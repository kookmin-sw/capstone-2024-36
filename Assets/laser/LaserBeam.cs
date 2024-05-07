using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserBeam
{
    Vector3 pos, dir;
    public GameObject laserObject;
    public LineRenderer laser;
    public List<Vector3> laserIndices = new List<Vector3>();
    private List<BoxCollider> colliders = new List<BoxCollider>();
    public Color laserColor;

    public LaserBeam(Vector3 pos, Vector3 dir, Material material, Color color)
    {
        this.laserColor = color;

        this.laserObject = new GameObject();
        string name = "Laser_Beam" + (int)(color.r *255 ) + (int)(color.g * 255) + (int)(color.b * 255);
        this.laserObject.name = "Laser_Beam" + (int)(color.r *255 ) + (int)(color.g * 255) + (int)(color.b * 255);
        this.laserObject.tag = "Laser";
        this.laserObject.layer = LayerMask.NameToLayer("Laser");

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
        this.laserObject.layer = LayerMask.NameToLayer("Laser");
        this.laserObject.AddComponent<MeshCollider>();
        this.laserObject.AddComponent<MeshFilter>();
        this.laserObject.AddComponent<Rigidbody>();


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
        int layerMask = 1 << LayerMask.NameToLayer("Filter"); // 필터 레이어를 무시하기 위한 레이어 마스크
        layerMask = ~layerMask; // 필터 레이어를 제외한 다른 모든 레이어를 활성화합니다.
        if (Physics.Raycast(ray, out hit, 500, layerMask))
        {
            CheckHit(hit, dir);
        }
        else
        {
            laserIndices.Add(ray.GetPoint(500));
            UpdateLaser();
        }
        
    }

    public void CastRay(Vector3 pos, Vector3 dir, LineRenderer laser)
    {
        laserIndices.Add(pos);
        this.laser = laser;

        Ray ray = new Ray(pos, dir);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Filter"); // 필터 레이어를 무시하기 위한 레이어 마스크
        layerMask = ~layerMask; // 필터 레이어를 제외한 다른 모든 레이어를 활성화합니다.
        Offcollider();
        if (Physics.Raycast(ray, out hit, 500, layerMask))
        {
            CheckHit(hit, dir);
        }
        else
        {
            laserIndices.Add(ray.GetPoint(500));
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

    public void UpdateCollider(){
        for (int i = 0; i < laser.positionCount - 1; i++)
        {
            Vector3 startPos = laser.GetPosition(i);
            Vector3 endPos = laser.GetPosition(i + 1);
            Debug.Log(startPos +"end " +  endPos);
            if(i >= colliders.Count){
                CreateBoxCollider(startPos,endPos);
                Debug.Log("create");
            }
            else{
                if(colliders[i].gameObject.activeSelf == false){
                    colliders[i].gameObject.SetActive(true);
                }
                UpdateBoxCollider(colliders[i],startPos,endPos);
            }

        }
        for (int i = laser.positionCount - 1; i < colliders.Count; i++)
        {
           colliders[i].gameObject.SetActive(false);
        }
    }

    public void Offcollider(){
        for (int i = 0; i < colliders.Count; i++){
            colliders[i].gameObject.SetActive(false);
        }

    }

    void CreateBoxCollider(Vector3 startPos, Vector3 endPos)
    {
        Vector3 midPoint = (startPos + endPos) / 2f;
        Vector3 colliderSize = new Vector3(Vector3.Distance(startPos, endPos), 0.1f, 0.1f);

        GameObject colliderObject = new GameObject("LaserCollider");
        colliderObject.transform.position = midPoint;

        // 레이저의 방향을 향하도록 콜라이더의 회전 설정
        colliderObject.transform.rotation = Quaternion.LookRotation(endPos - startPos);
        colliderObject.transform.Rotate(0f, 90f, 0f);


        BoxCollider collider = colliderObject.AddComponent<BoxCollider>();
        collider.size = colliderSize;
        collider.isTrigger = true;

        colliders.Add(collider);

    }

    void UpdateBoxCollider(BoxCollider collider, Vector3 startPos, Vector3 endPos)
    {
        Vector3 midPoint = (startPos + endPos) / 2f;
        Vector3 colliderSize = new Vector3(Vector3.Distance(startPos, endPos), 0.1f, 0.1f);
        Quaternion rotation = Quaternion.LookRotation(endPos - startPos);
        

        collider.transform.position = midPoint;
        collider.transform.rotation = rotation;
        collider.transform.Rotate(0f, 90f, 0f);
        collider.size = colliderSize;
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
        else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("ClearCheck")){
            hitInfo.collider.GetComponent<LightDoorUpdate>().ClearSuccess(this.laserColor);
            laserIndices.Add(hitInfo.point);
            UpdateLaser();
        }
         else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Laser")){
            Debug.Log("laser");
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
            Debug.Log("laserpointer by laserability hit");
            hitInfo.collider.GetComponent<NetworkLaserPointerShoot>().LaserColor = laserColor;
            hitInfo.collider.GetComponent<NetworkLaserPointerShoot>().Onofflaser();
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



    public void CheckCollision()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(pos, dir, out hitInfo, 500))
        {
            // 충돌이 발생한 경우 해당 레이저의 충돌 처리 함수를 호출합니다.
            if (hitInfo.collider.gameObject.layer == this.laserObject.layer)
            {
                Debug.Log("collsion laser laser ");
                StopAtCollision(hitInfo.point);
            }
        }
    }

    // 충돌 지점에서 레이저를 멈추는 함수
    private void StopAtCollision(Vector3 collisionPoint)
    {
        laser.SetPosition(laser.positionCount - 1, collisionPoint);
        
        // 충돌 지점에서 더 이상 전진하지 않도록 레이저의 위치를 고정합니다.
        laserIndices.Clear(); // 이전까지의 레이저 포인트들을 제거하여 전진을 막습니다.
        laserIndices.Add(collisionPoint); // 충돌 지점을 추가하여 레이저를 고정합니다.

        // 레이저를 업데이트하여 변경된 위치를 반영합니다.
        UpdateLaser();
    }

    public void GenerateMeshCollider()
    {
        MeshCollider collider = this.laserObject.GetComponent<MeshCollider>();

        if (collider == null)
        {
            collider = this.laserObject.AddComponent<MeshCollider>();
        }


        Mesh mesh = new();
        this.laser.BakeMesh(mesh, true);
        // collider.sharedMesh = mesh;
        // this.laser.GetComponent<MeshFilter>().mesh = mesh;

        // this.laser.generateLightingData = true;
        // collider.convex = true;
        // Rigidbody rb = this.laserObject.GetComponent<Rigidbody>();
        // rb.useGravity = false;
        // collider.isTrigger = true;

        // if you need collisions on both sides of the line, simply duplicate & flip facing the other direction!
        // This can be optimized to improve performance ;)
        // int[] meshIndices = mesh.GetIndices(0);
        // int[] newIndices = new int[meshIndices.Length * 2];
        // int j = meshIndices.Length - 1;
        // for (int i = 0; i < meshIndices.Length; i++)
        // {
        //     newIndices[i] = meshIndices[i];
        //     newIndices[meshIndices.Length + i] = meshIndices[j];
        // }
        // mesh.SetIndices(newIndices, MeshTopology.Triangles, 0);

        collider.sharedMesh = mesh;
    }

}
