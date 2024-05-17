using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WallColor : MonoBehaviour
{
    // Start is called before the first frame update
    public Color incolor;
    public Color outcolor;
    private bool IsRed = false;
    private bool IsBlue = false;
    private bool IsGreen = false;
    public Color mix = Color.black;
    public GameObject door;
    public GameObject door2;
    
    public bool isClear;

    private MeshRenderer meshRenderer;


    public Color ClearWallColor;
    void Awake(){
        meshRenderer = GetComponent<MeshRenderer>();
    }
    void FixedUpdate(){
        isClear=false;
        
    }
    void Update(){
        
        if(isClear){
            if (door.transform.position.y > -2)
                {
                    Debug.Log("down");
                    door.transform.Translate(Vector3.down * Time.deltaTime *3);
                    if(door2 != null){
                        door2.transform.Translate(Vector3.down * Time.deltaTime *3);
                    }
                }
        }
        else{
            if (door.transform.position.y < 2.8)
                {
                    Debug.Log("up");
                    door.transform.Translate(Vector3.up * Time.deltaTime );
                    if(door2 != null){
                        door2.transform.Translate(Vector3.up * Time.deltaTime );
                    }
                }

        }
        ColorOFF();
        // mix = Color.white;
        // SetWallColor(Color.white);
        
    }

    void UpdaetWallColor(){
        mix = mixColor();
        SetWallColor(mix);
        if(mix == ClearWallColor){
            isClear = true;
        }
        // mix = outcolor;
        

    }

    public void ColorOn(Color color){
        if(color == Color.red){
            IsRed = true;
        }
        else if(color == Color.blue){
            IsBlue = true;
        }
        else if(color == Color.green){
            IsGreen = true;
        }
    }
    public void ColorOFF(){
        IsRed =false;
        IsBlue =false;
        IsGreen = false;
    }

    public void SetWallColor(Color color)
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = color;
        }
    }

    public Color mixColor(){
        float redValue = IsRed ? 1f : 0f;
        float blueValue = IsBlue ? 1f : 0f;
        float greenValue = IsGreen ? 1f : 0f;

        // 각 색상 채널을 누적하여 새로운 색상을 생성
        mix = new Color(redValue, greenValue, blueValue);

        return mix;
    }

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("trigger");
        // if (collision.gameObject.name == "LaserCollider25500")
        // {
        //     IsRed  = true;
        // }
        // else if (collision.gameObject.name == "LaserCollider00255")
        // {
        //     IsBlue  = true;
        // }
        // else if (collision.gameObject.name == "LaserCollider02550")
        // {
        //     IsGreen  = true;
        // }
    }


    void OnTriggerStay(Collider collision){
        Debug.Log("stay");
        if (collision.gameObject.name == "EndCollider25500")
        {
            IsRed  = true;
        }
        if (collision.gameObject.name == "EndCollider00255")
        {
            IsBlue  = true;
        }
        if (collision.gameObject.name == "EndCollider02550")
        {
            IsGreen  = true;
        }
        UpdaetWallColor();     
    }

    void OnTriggerExit(Collider collision){
        Debug.Log("exit");
        
        ColorOFF();
        mix = Color.white;
        SetWallColor(Color.white);  
        isClear = false;
    }

    public void SetinColor(Color color){
        incolor = color;
        if(incolor.r ==1 || incolor.b ==1 || incolor.g ==1){
            outcolor = incolor;
        }
    }
    public void SetoutColor(){
        outcolor.r = incolor.r != 0 ? 1 : 0;
        outcolor.g = incolor.g != 0 ? 1 : 0;
        outcolor.b = incolor.b != 0 ? 1 : 0;

    } 

}
