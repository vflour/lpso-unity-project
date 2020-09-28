using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetSpriteDatabase", menuName = "PetSprites/PetSpriteDatabase", order = 1)]
public class PetDatabase : ScriptableObject
{
    public GameObject CustomizablePartPrefab;
    

    [Header("Kitty")]
    public SpeciesSubtype[] KittyCrAP;

    [Header("Dog")]
    public SpeciesSubtype[] DogCrAP;
    public PaletteStorage DogPalettes;

    [Header("Mouse")]
    public SpeciesSubtype[] MouseCrAP;

    [Header("Collect A Pet")]
    public SpeciesSubtype[] PetsCoAP;

    // gets the species array based on index
    public SpeciesSubtype[] GetSpeciesArray(int Index)
    {
        switch (Index)
        {
            case 0:
                return KittyCrAP;
            default:
                return DogCrAP;
        }
    }
    public PaletteStorage GetPaletteArray(int Index)
    {
        switch (Index)
        {
            case 0:
                return DogPalettes;
            default:
                return DogPalettes;
        }
    }
}
