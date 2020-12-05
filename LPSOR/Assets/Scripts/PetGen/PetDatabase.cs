using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetSpriteDatabase", menuName = "PetSprites/PetSpriteDatabase", order = 1)]
public class PetDatabase : ScriptableObject
{
    public enum SpeciesNames
    {
        Kitty,
        Dog,
        Mouse
    };
    
    public GameObject CustomizablePartPrefab;
    public int SpeciesCount = 2;
    
    [Header("Kitty")]
    public SpeciesSubtype[] KittyCrAP;
    public PaletteStorage KittyPalettes;
    
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
                return KittyPalettes;
            default:
                return DogPalettes;
        }
    }
}
