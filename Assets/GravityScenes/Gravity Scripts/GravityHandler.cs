using UnityEngine;

public class GravityHandler : MonoBehaviour
{
    private bool isGravityEnabled = true;

    //중력 조절 능력 횟수 제한
    private int clickCount = 0;
    private const int maxClickCount = 1;
    private GameObject _gravityArea;

    public void ToggleGravity(GameObject gravityArea)
    {
        if (clickCount >= maxClickCount)
        {
            return; 
        }

        _gravityArea = gravityArea;

        clickCount++; 

        //중력 상태에 따라 키고 끌 수 있게 바꿀 생각중
        //현재 그냥 중력을 끄게만 되어있음
        isGravityEnabled = !isGravityEnabled;
        _gravityArea.SetActive(isGravityEnabled);
        Invoke("EnableGravity", 2f); 
        Debug.Log("check toggled"  );
    }

    private void EnableGravity()
    {
        isGravityEnabled = true;
        _gravityArea.SetActive(isGravityEnabled);
        clickCount = 0; 
    }
}
