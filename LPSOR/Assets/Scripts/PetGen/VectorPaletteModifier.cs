using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using System.IO;

public class VectorPaletteModifier : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    
    public int ColorIndex = 3;
    private Color[] palette = new Color[3];
    public Color[] Palette
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
        
        sprite.material.SetFloat("_ColorIndex", ColorIndex);
        if (ColorIndex > 2) return;
        sprite.material.color = palette[ColorIndex];
    }

}
