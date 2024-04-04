using UnityEngine;
using Unity.Netcode;

public class NetworkLaserPointerShoot : NetworkBehaviour
{
    public Color LaserColor = Color.red;
    public Material material;

    // 레이저의 활성화 상태를 나타내는 Network Variable
    public NetworkVariable<bool> isLaserActive = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    LaserBeam beam;

    void Update()
    {
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

            Debug.Log(IsClient);
            Debug.Log(IsServer);
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

        // 변경된 값을 모든 클라이언트들에게 전달합니다.
        ToggleLaserActivationClientRpc(isLaserActive.Value);
    }

    // 클라이언트에서 호출되는 RPC 함수로 변경된 레이저 활성화 상태를 받아 처리합니다.
    [ClientRpc]
    private void ToggleLaserActivationClientRpc(bool isActive)
    {
        // 변경된 레이저 활성화 상태를 클라이언트에서 처리합니다.
        isLaserActive.Value = isActive;
        Debug.Log("toggle client recieve");
        // 레이저가 없는 경우에만 생성합니다.
        if (beam == null && isActive)
        {
            beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, LaserColor);
            Debug.Log("빔 생성");
        }

        Debug.Log(isActive);
    }
}
