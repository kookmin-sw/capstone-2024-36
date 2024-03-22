using UnityEngine;

public class ObjectPositionReset : MonoBehaviour
{
    private Vector3 savedPosition; 
    private bool isPositionSaved = false; 

    private void Update()
    {
        // (물체를 잡은 상태에서)로 구현할 예정
        // R 키를 누르면 현재 위치를 저장하고 저장된 위치로 오브젝트를 이동합니다.
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isPositionSaved)
            {
                SavePosition(); // 위치를 저장
            }
            else
            {
                ResetToSavedPosition(); // 저장된 위치로 되돌아감
            }
        }
    }

    private void SavePosition()
    {
        savedPosition = transform.position; 
        isPositionSaved = true; 
    }

    private void ResetToSavedPosition()
    {
        transform.position = savedPosition; 
        isPositionSaved = false; 
    }
}