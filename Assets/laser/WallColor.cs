using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class WallColor : MonoBehaviour
{
    // Start is called before the first frame update
    public Color incolor;
    public Color outcolor;
    private bool IsRed = false;
    private bool IsBlue = false;
    private bool IsGreen = false;
    private Color mix = Color.black;


    public Color ClearWallColor;
    
    void Update(){
        
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

    public void SetWallColor(Color color)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }

    public Color mixColor(){
        float redValue = IsRed ? 1f : 0f;
        float blueValue = IsBlue ? 1f : 0f;
        float greenValue = IsGreen ? 1f : 0f;

        // 각 색상 채널을 누적하여 새로운 색상을 생성
        Color mix = new Color(redValue, greenValue, blueValue);

        return mix;
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
