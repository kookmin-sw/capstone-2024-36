using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReciprocatingMotion : MonoBehaviour
{
    Vector3 pos; //������ġ
    [SerializeField] float delta = 8.0f; // ��(��)�� �̵������� (x)�ִ밪
    [SerializeField] float Xspeed = 3.0f; // �̵��ӵ�
    [SerializeField] float Yspeed = 0.0f; // �̵��ӵ�

    void Start()
    {
        pos = transform.position;
    }


    void Update()
    {
        Vector3 v = pos;
        v.x += delta * Mathf.Sin(Time.time * Xspeed);
        v.y += delta * Mathf.Sin(Time.time * Yspeed);
        transform.position = v;
    }
}
