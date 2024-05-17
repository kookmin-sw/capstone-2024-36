using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DoorUpdate : MonoBehaviour
{
    public bool isClear;
    bool allClear;
    public List<GameObject> doorlist;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        isClear= true;
        CheckDoorList();
        if (isClear)
            {
                if (gameObject.transform.position.y > -2)
                {
                    gameObject.transform.Translate(Vector3.down * Time.deltaTime *3);
                }
            }
            else
            {
                if (gameObject.transform.position.y < 2)
                {
                    gameObject.transform.Translate(Vector3.up * Time.deltaTime);
                }
            }
        
    }

    void CheckDoorList()
    {
        allClear=true;
            foreach(GameObject door in doorlist)
        {
            if(!door.GetComponent<LightDoorUpdate>().isDoorActive) // GameObject의 isclear이 false인 경우
            {
                allClear = false; // 모든 GameObject의 isclear을 false로 설정
                break; // 더 이상 확인할 필요 없으므로 반복문 종료
            }
        }

        // 모든 GameObject의 isclear이 true일 때만 isclear을 true로 설정
        isClear = allClear;

    }
}
