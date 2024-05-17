using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public class NetworkLaserPointerShoot : NetworkBehaviour
{
    public Color LaserColor = Color.red;
    public Material material;

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
        lasermanger = GameObject.Find("LaserManger").GetComponent<LaserManger>();
    }

    void Update()
    {
        if(thisscene){if(beam == null){
            //beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, LaserColor);
        }
        else{
            if (isLaserActive.Value && beam != null)
            {
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
