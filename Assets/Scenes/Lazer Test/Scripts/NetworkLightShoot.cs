    using System.Collections;
    using System.Collections.Generic;
    using Unity.Netcode;
    using UnityEngine;

    public class NetworkLightShoot : NetworkBehaviour
    {
        public Color LaserColor = Color.red;
        public Material material;

        private Camera m_camera;

        private RaycastHit hit;

        bool isLaserActive = false;

        private PlayerControl actions;

        LaserBeam beam;

        void Awake(){
            if (actions == null)
        {
            actions = new PlayerControl();

            actions.PlayerAction.Lightability.performed += _ => Shoot();

            actions.Enable();
        }
        }
        void Start(){
            isLaserActive = true;
        }

        void Update(){
            
        }

        void Shoot(){
            if(IsLocalPlayer){
                Debug.Log("f button");
                Vector3 playerposition = FindInChildren(transform,"Base HumanRArmDigit23").position;
                
                m_camera = Camera.main;
                // Vector3 mousedirection = m_camera.transform.forward;
                Vector3 cameraPosition = m_camera.transform.position;
                Vector3 camerafoward = m_camera.transform.forward;

                Ray laserray = new(cameraPosition, camerafoward);
                // Ray laserray = m_camera.ScreenPointToRay(Input.mousePosition);
                Vector3 directionToCameraCenter = (laserray.GetPoint(30f) - playerposition).normalized;
                if(Physics.Raycast(laserray, out hit)){
                    // mousedirection = (hit.point - playerposition).normalized;
                    directionToCameraCenter = (hit.point - playerposition).normalized;
                    Debug.Log(playerposition);
                }
                ShootLaserServerRpc(playerposition,directionToCameraCenter);
                ColorChange();
            }
        }

        // find child object using recrusive
        Transform FindInChildren(Transform parent, string name)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == name)
                {
                    return child; // 찾은 경우 해당 자식을 반환합니다.
                }
                else
                {
                    // 재귀적으로 하위 자식을 검색합니다.
                    Transform foundChild = FindInChildren(child, name);
                    if (foundChild != null)
                    {
                        return foundChild; // 하위 자식에서 찾은 경우 해당 자식을 반환합니다.
                    }
                }
            }
            return null; // 모든 자식을 검색한 후에도 찾지 못한 경우 null을 반환합니다.
        }

        
        [ServerRpc]
        void ShootLaserServerRpc(Vector3 position, Vector3 direction){
            ShootLaserClientRPC(position, direction);
        }

        [ClientRpc]
        void ShootLaserClientRPC(Vector3 position, Vector3 direction){
            // if(beam != null && isLaserActive == true){
            //     beam.laser.positionCount = 0;
            //     beam.laserIndices.Clear();
            //     beam.CastRay(position, direction, beam.laser);}
            LaserBeam beam = new LaserBeam(position,direction, material,LaserColor ,"laserability");
            Invoke(nameof(DestroyLaserAbility), 0.3f);
        }

        void DestroyLaserAbility(){
            GameObject laserAbility = GameObject.Find("laserability");
            if (laserAbility != null)
            {
                Destroy(laserAbility);
            }
        }

        void ColorChange(){
            if(LaserColor == Color.red){
                LaserColor = Color.blue;
            }
            else if(LaserColor == Color.blue){
                LaserColor = Color.green;
            }
            else if(LaserColor == Color.green){
                LaserColor = Color.red;
            }

        }

        private void SetExist(bool bExist){
        if(bExist){
        }
        else{

        }
        }


    }
