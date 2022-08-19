using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class VectorPaletteModifier : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    
    public int ColorIndex = 3;
    private PaletteColor[] palette = new PaletteColor[3];
    public PaletteColor[] Palette
    {
        set
        {
            palette = value;
            InitColorSwapTex(GetComponent<SpriteRenderer>());
        }
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitColorSwapTex(spriteRenderer);
    }
    public void InitColorSwapTex(SpriteRenderer sprite)
    {
        if (palette[ColorIndex] == null) return;
        sprite.material.SetFloat("_ColorIndex", ColorIndex);
        sprite.material.SetFloat("_Saturation", palette[ColorIndex].saturationMultiplier);
        if (ColorIndex > 2) return;
        sprite.material.color = palette[ColorIndex].color;  
        
    }

}
