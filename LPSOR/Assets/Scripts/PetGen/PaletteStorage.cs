using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetPalettes", menuName = "PetSprites/PetPalettes", order = 1)]
public class PaletteStorage : ScriptableObject
{
    [Header("Coat Palettes")]
    public PaletteColor[] coatPalettes;

    [Header("Patch Palettes")]
    public PaletteColor[] patchPalettes;
    
    [Header("Eyecolor Palettes")]
    public PaletteColor[] eyePalettes;
    
    public PaletteColor[] getTypePalette(int typeIndex)
    {
        switch (typeIndex)
        {
            case 0:
                return coatPalettes;
            case 1:
                return patchPalettes;
            default:
                return eyePalettes;
        }
    }
}

[System.Serializable]
public class PaletteColor
{
    public Color color;
    public float saturationMultiplier = 1;
}
