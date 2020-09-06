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
    public float IsColorable = 1;

    public float[] CoatPalette{
        set {
            coatPalette = value;
            InitColorSwapTex(GetComponent<SpriteRenderer>());
        }
    }
    public float[] PatchPalette{
        set {
            patchPalette = value;
            InitColorSwapTex(GetComponent<SpriteRenderer>());
        }
    }
    public float[] EyesPalette{
        set {
            eyesPalette = value;
            InitColorSwapTex(GetComponent<SpriteRenderer>());
        }
    }

    private void Start()
    {   
        //InitColorSwapTex(GetComponent<SpriteRenderer>());
    }
    public void InitColorSwapTex(SpriteRenderer sprite)
    {
        Color[] HSVarray = new Color[3];
        sprite.material.SetFloat("_IsColorable", IsColorable);
        if (IsColorable == 0) return;
        
    
        HSVarray[0] = new Color(coatPalette[0],coatPalette[1],coatPalette[2],1);
        HSVarray[1] = new Color(patchPalette[0],patchPalette[1],patchPalette[2],1);
        HSVarray[2] = new Color(eyesPalette[0],eyesPalette[1],eyesPalette[2],1);

        sprite.material.SetColorArray("_ColorArray", HSVarray);
        
    }

}
