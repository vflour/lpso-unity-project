using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using System.IO;

public class VectorPaletteModifier : MonoBehaviour
{
    private float[] coatPalette = new float[3];
    private float[] patchPalette = new float[3];
    private float[] eyesPalette = new float[3];
    public float ColorIndex = 3;

    public float[,] Palette
    {
        set {
            coatPalette = new float[]{value[0,2],value[0,1],value[0,2]};
            patchPalette = new float[]{value[1,2],value[1,1],value[1,2]};
            eyesPalette = new float[]{value[2,2],value[2,1],value[2,2]};

            InitColorSwapTex(GetComponent<SpriteRenderer>());
        }
    }

    private void Start()
    {   
        InitColorSwapTex(GetComponent<SpriteRenderer>());
    }
    public void InitColorSwapTex(SpriteRenderer sprite)
    {
        Color[] HSVarray = new Color[3];
        sprite.material.SetFloat("_ColorIndex", ColorIndex);
        if (ColorIndex >= 2) return;
        
    
        HSVarray[0] = new Color(coatPalette[0],coatPalette[1],coatPalette[2],1);
        HSVarray[1] = new Color(patchPalette[0],patchPalette[1],patchPalette[2],1);
        HSVarray[2] = new Color(eyesPalette[0],eyesPalette[1],eyesPalette[2],1);

        sprite.material.SetColorArray("_ColorArray", HSVarray);
        
    }

}
