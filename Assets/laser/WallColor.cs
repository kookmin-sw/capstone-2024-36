using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallColor : MonoBehaviour
{
    // Start is called before the first frame update
    public Color incolor;
    public Color outcolor;

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
