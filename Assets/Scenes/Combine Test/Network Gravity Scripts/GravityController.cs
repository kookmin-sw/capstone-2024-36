using UnityEngine;

public class GravityController : MonoBehaviour
{
    public GravityHandler gravityHandler; // 중력 핸들러(중력 줄였다 키우기)
    private BackToInitialPos backToInitialPosition; // 초기 위치 저장 클래스
    private Vector3 initialPosition; // 초기 위치 저장용 변수

    private void Update()
    {
        if (Input.GetKey(KeyCode.Y)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Debug.Log(ray);

            //"GravityControl" 레이어를 무시하는 레이어 마스크 생성
            int layerMask = 1 << LayerMask.NameToLayer("GravityArea");
            layerMask = ~layerMask;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                //중력존 태그를 가진 객체와 충돌했을 때 중력 처리
                if (hit.collider.CompareTag("GravityObject"))
                {   
                    Debug.Log("Toggle Gravity Objecy : " + hit.collider.gameObject.name);
                    if (gravityHandler != null)
                    {
                        Debug.Log("check Gravityhandler : " + gravityHandler );
                        gravityHandler.ToggleGravity(hit.collider.gameObject);
                    }
                }
                //이동 가능한 오브젝트 태그를 가진 객체와 충돌했을 때 위치 조정
                else if(hit.collider.CompareTag("moveable"))
                {
                    Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>();
                    Debug.Log("Back To Initial Position : " + hit.collider.gameObject.transform.position);
                    
                    //초기 위치 저장 및 위치 및 회전 조정
                    backToInitialPosition = hit.collider.gameObject.GetComponent<BackToInitialPos>();
                    //backToInitialPosition.initialPosition.x = 0f;
                    //hit.collider.gameObject.transform.position = backToInitialPosition.initialPosition;
                    //hit.collider.gameObject.transform.rotation = backToInitialPosition.initialRotations;  
                    
                    //물리 시뮬레이션 재시작 
                    //isKinematic을 꺼놓은 상태라 한번 받은 힘의 영향을 계속 받아서 
                    //원래 자리로 돌아가도 계속 움직이기 때문에
                    rb.isKinematic = true;
                    rb.isKinematic = false;

                    Debug.Log("Cube's position is " + hit.collider.gameObject.transform.position);
                }    
            }
        }
    }
}
