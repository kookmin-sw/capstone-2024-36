using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour
{
    Vector3 mousePosition;
    public Camera getCamera;
    private RaycastHit hit;
    private GameObject NPCDialog;
    private Text NPCText; 

    void Start()
    {
        NPCDialog = GameObject.Find("Interact");
        NPCText = GameObject.Find("Text").GetComponent<Text>();
        NPCDialog.SetActive(false);
    }

    void Awake()
    {
        getCamera = Camera.main;
        NPCDialog = GameObject.Find("Interact");
        NPCText = GameObject.Find("Text").GetComponent<Text>();
        NPCDialog.SetActive(false); 
    }

    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown()
    {

        //마우스의 위치 가져오기
        mousePosition = Input.mousePosition - GetMousePos();
        Debug.Log("Mouse Down");

    }

    private void OnMouseEnter()
    {
        Debug.Log("enter");
        transform.GetChild(0).gameObject.SetActive(true);
        //Debug.Log(transform.GetComponent<MyNetworkTransform>().RegisterId);
    }

    private void OnMouseExit()
    {
        Debug.Log("exit");
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnMouseDrag()
    {
        Debug.Log("Moveable Drag");
        //마우스의 위치를 월드좌표로 변환시켜서 물체에 대입
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        if (transform.position.y <= 0)
        {
            //물체가 y좌표 0 이하로 가는 것을 방지
            transform.position = new Vector3(transform.position.x, transform.localScale.y / 2, transform.position.z);
            transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);

        }
        if (transform.position.y > 30)
        {
            //물체가 y좌표 30 이상으로 가는 것을 방지
            transform.position = new Vector3(transform.position.x, 30-transform.localScale.y / 2, transform.position.z);
            transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);

        }
        NetworkMovingTest.m_movedPosition = transform.position;
    }

    private void OnMouseUp()
    {
        Ray ray = getCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "moveable")
            {
                hit.rigidbody.velocity = Vector3.zero;
                hit.rigidbody.angularVelocity = Vector3.zero;
                hit.rigidbody.useGravity = true;
            }
        }
    }
    void Update()
    {
        //Ray ray = getCamera.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out hit))
        //{
        //    if (hit.collider.tag == "moveable")
        //    {
        //        hit.rigidbody.velocity = Vector3.zero;
        //        hit.rigidbody.angularVelocity = Vector3.zero;
        //        hit.rigidbody.useGravity = true;
        //    }
        //}
    }
    // Update is called once per frame
    //void Update()
    //{
    //    Ray ray = getCamera.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray, out hit))
    //    { 
    //        if(hit.collider.tag == "moveable")
    //        {
    //            //물체의 중력이 중첩되는 부분 수정
    //            //hit.rigidbody.velocity = Vector3.zero;
    //            //hit.rigidbody.angularVelocity = Vector3.zero;
    //            //Q로 물체의 크기 키우기
    //            if (Input.GetKeyDown(KeyCode.Q) && hit.transform.localScale[0] < 10.0f && hit.transform.localScale[1] < 10.0f && hit.transform.localScale[2] < 10.0f)
    //            {
    //                hit.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
    //            }
    //            else if (Input.GetKey(KeyCode.Q) && hit.transform.localScale[0] < 10.0f && hit.transform.localScale[1] < 10.0f && hit.transform.localScale[2] < 10.0f)
    //            {
    //                hit.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
    //            }
    //            //E로 물체의 크기 줄이기
    //            if (Input.GetKeyDown(KeyCode.E) && hit.transform.localScale[0] > 1.0f && hit.transform.localScale[1] > 1.0f && hit.transform.localScale[2] > 1.0f)
    //            {
    //                hit.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
    //            }
    //            else if (Input.GetKey(KeyCode.E) && hit.transform.localScale[0] > 1.0f && hit.transform.localScale[1] > 1.0f && hit.transform.localScale[2] > 1.0f)
    //            {
    //                hit.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
    //            }
    //            //R로 물체 회전
    //            if (Input.GetKey(KeyCode.R))
    //            {
    //                hit.transform.eulerAngles += new Vector3(0f, 0.2f, 0f);
    //                Debug.Log("R");
    //            }
    //        }
    //    }
    //    //Debug.Log(hit.transform.localScale);
    //    //hit.Rigidbody2D.velocity = new Vector2(0, 0);
    //}
}
