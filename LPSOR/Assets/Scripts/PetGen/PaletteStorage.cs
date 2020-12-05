using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetPalettes", menuName = "PetSprites/PetPalettes", order = 1)]
public class PaletteStorage : ScriptableObject
{
    [Header("Coat Palettes")]
    public Color[] coatPalettes;

    [Header("Patch Palettes")]
    public Color[] patchPalettes;
    
    [Header("Eyecolor Palettes")]
    public Color[] eyePalettes;
    
    public Color[] getTypePalette(int typeIndex)
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
