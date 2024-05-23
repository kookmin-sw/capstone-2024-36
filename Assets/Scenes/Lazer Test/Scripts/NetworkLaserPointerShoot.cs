using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public class NetworkLaserPointerShoot : NetworkBehaviour
{
    public Color LaserColor = Color.red;
    public Material material;
    public GameObject particleSystemPrefab;

    public LaserManger lasermanger;

    public List<GameObject> colliders;

    public bool thisscene = true;

    // 레이저의 활성화 상태를 나타내는 Network Variable
    public NetworkVariable<bool> isLaserActive = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public LaserBeam beam;

    private float lastUpdateTime; // 마지막으로 업데이트된 시간
    public float updateInterval = 0.5f; // 업데이트 간격 설정 (초 단위)

    void Start(){
    }

    void Update()
    {
        if (lasermanger == null)
            lasermanger = GameObject.Find("LaserManger").GetComponent<LaserManger>();


        if (isLaserActive.Value)
        {
            if(beam == null){
                beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, LaserColor);
                lasermanger.lasers.Add(gameObject);
                Debug.Log("빔생성");
            }
            beam.laserObject.SetActive(true);
            beam.EndPoint.SetActive(true);
            beam.laserObject.GetComponent<LineRenderer>().startColor = LaserColor;
            beam.laserObject.GetComponent<LineRenderer>().endColor = LaserColor;
            beam.EndPoint.name = "EndCollider" + (int)( LaserColor.r *255 ) + (int)( LaserColor.g * 255) + (int)( LaserColor.b * 255);
            beam.laserColor = LaserColor;
            // beam.laser.positionCount = 0;
            // beam.laserIndices.Clear();  
            // beam.CastRay(gameObject.transform.position, gameObject.transform.up, beam.laser);
        }
        else if (!isLaserActive.Value && beam != null)
        {
            beam.laserObject.SetActive(false);
            beam.Offcollider();
            beam.EndPoint.SetActive(false);
            // beam.laser.positionCount = 0;
            // beam.laserIndices.Clear();
        }
        
            
        
        
        
       
    }

    // public void Onofflaser()
    // {
        
    //     isLaserActive.Value = !isLaserActive.Value; // 레이저 활성화 상태를 토글
    //     Debug.Log(isLaserActive.Value);
    //     if(beam == null){
    //     beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, LaserColor);
    //     Debug.Log("빔생성");
    //     }
    //     if (!isLaserActive.Value){
    //         beam.laser.positionCount = 0;
    //         beam.laserIndices.Clear();  
    //     }
    
    // }
        public void Onofflaser()
    {
            // 클라이언트에서 서버로 RPC를 보냅니다.
            if(thisscene){
                if(NetworkManager.Singleton.IsServer){
                    isLaserActive.Value = !isLaserActive.Value;
                    Debug.Log("toggle success");
                    }
                RequestToggleLaserActivationServerRpc();
                Debug.Log("toggle send");
                Debug.Log(isLaserActive.Value);

            // 레이저가 꺼질 때에만 파티클 시스템 생성
            if (!isLaserActive.Value)
            {
                // 마지막으로 꺼진 레이저의 시작점부터 끝점까지의 레이저 줄기에 따라 파티클 시스템 실행
                if (beam != null)
                {
                    GenerateParticlesAlongLaser(beam.laser.GetPosition(0), beam.laser.GetPosition(beam.laser.positionCount - 1));
                }
            }
        }
            
        
    }

    private void GenerateParticlesAlongLaser(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        int numParticles = Mathf.FloorToInt(distance * 20); // 거리에 따른 파티클 수

        direction.Normalize(); // 방향 벡터 정규화

        for (int i = 0; i < numParticles; i++)
        {
            // 파티클의 위치 계산
            Vector3 particlePosition = start + direction * (i + 0.5f); // 레이저의 중간 지점에 파티클 생성

            // 파티클 시스템 생성
            GameObject particles = Instantiate(particleSystemPrefab, particlePosition, Quaternion.identity);
            particles.GetComponent<ParticleSystem>().Play();
            Destroy(particles, 3f);
        }
    }

    // 서버에서 호출되는 RPC 함수로 레이저 활성화 상태를 토글합니다.
    [Rpc(SendTo.Server)]
    private void RequestToggleLaserActivationServerRpc()
    {
        // 클라이언트에서 온 요청을 처리하여 네트워크 변수를 변경합니다.
        // if(NetworkManager.Singleton.IsServer){
        // isLaserActive.Value = !isLaserActive.Value;
        Debug.Log("toggle changed");
        RequestLaserActiveClientRpc(isLaserActive.Value);
        
    }

    [ClientRpc]
    private void RequestLaserActiveClientRpc(bool isActive)
    {
        if (isActive)
        {
            if (beam == null)
            {
                beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, LaserColor);
                lasermanger.lasers.Add(gameObject);
                Debug.Log("빔생성");
            }
        }
    }



    public void SetExist(bool bExist){
        if(bExist){
            if(beam == null && isLaserActive.Value){
                beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, LaserColor);
                lasermanger.lasers.Add(gameObject);
                Debug.Log("빔생성");
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(true);
            }
             isLaserActive.Value = false;
             thisscene = true;
        }
        else{
            if(beam != null){
            beam = null;
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(false);
            }
            isLaserActive.Value = false;
            thisscene = false;
        }
    }


    public void Shootlaser()
    {
        beam.laser.positionCount = 0;
        beam.laserIndices.Clear();
        beam.CastRay(gameObject.transform.position, gameObject.transform.up, beam.laser);
        beam.UpdateCollider();
        // beam.Offcollider();
        // beam.GenerateMeshCollider();
    }

}
