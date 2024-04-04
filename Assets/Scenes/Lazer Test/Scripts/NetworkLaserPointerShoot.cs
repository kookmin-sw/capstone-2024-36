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

    void Start(){
        if(beam == null){
        beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, LaserColor);
        Debug.Log("빔생성");
        }
    }

    void Update()
    {
        beam.laser.positionCount = 0;
        beam.laserIndices.Clear();
        
        if (isLaserActive.Value && beam != null)
        {
            
            beam.CastRay(gameObject.transform.position, gameObject.transform.up, beam.laser);
        }
    
    }

    public void Onofflaser()
    {
        
        isLaserActive.Value = !isLaserActive.Value; // 레이저 활성화 상태를 토글
        Debug.Log(isLaserActive.Value);
    
    }
}
