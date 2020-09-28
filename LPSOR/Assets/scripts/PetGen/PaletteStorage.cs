using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetPalettes", menuName = "PetSprites/PetPalettes", order = 1)]
public class PaletteStorage : ScriptableObject
{
    [Header("Coat Palettes")]
    public ColorPaletteField[] coatPalettes;

    [Header("Eyecolor Palettes")]
    public ColorPaletteField[] eyePalettes;

    [Header("Patch Palettes")]
    public ColorPaletteField[] patchPalettes;

    [System.Serializable]
    public struct ColorPaletteField
    {
        public float[] Colors;
    }
}
