using UnityEngine;
using Unity.Netcode;

public class NetworkLaserPointerShoot : NetworkBehaviour
{
    public Color LaserColor = Color.red;
    public Material material;

    // 레이저의 활성화 상태를 나타내는 Network Variable
    public NetworkVariable<bool> isLaserActive = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    LaserBeam beam;

    void Update()
    {
        if(beam == null){
            //beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, LaserColor);
            Debug.Log("beam없음");
        }
        else{
             if (isLaserActive.Value && beam != null)
            {
                beam.laser.positionCount = 0;
                beam.laserIndices.Clear();  
                beam.CastRay(gameObject.transform.position, gameObject.transform.up, beam.laser);
            }
            else if (!isLaserActive.Value && beam != null)
            {
                beam.laser.positionCount = 0;
                beam.laserIndices.Clear();
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
            RequestToggleLaserActivationServerRpc();
            Debug.Log("toggle send");
        
    }

    // 서버에서 호출되는 RPC 함수로 레이저 활성화 상태를 토글합니다.
    [ServerRpc]
    private void RequestToggleLaserActivationServerRpc(ServerRpcParams rpcParams = default)
    {
        // 클라이언트에서 온 요청을 처리하여 네트워크 변수를 변경합니다.
        isLaserActive.Value = !isLaserActive.Value;
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
                Debug.Log("빔생성");
            }
        }
    }

    
}
