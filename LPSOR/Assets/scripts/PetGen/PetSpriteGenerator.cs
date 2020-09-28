using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetSpriteGenerator
{
    public PetDatabase petDatabase;

    public PetSpriteGenerator(PetDatabase petDatabase)
    {
        this.petDatabase = petDatabase;
    }

    public GameObject GenerateCrAPSprite(int[] PaletteData, int Species, int speciesSubtype, int[] PartTypes)
    {
        // Find the species array
        SpeciesSubtype[] SpeciesArray = petDatabase.GetSpeciesArray(Species);
        SpeciesSubtype Subtype = SpeciesArray[speciesSubtype];
        
        // Instantiate the subtype's body
        GameObject CharacterObject = GameObject.Instantiate(Subtype.BodyPrefab);

        // sets the actual color palette based on palette data
        float[,] Palette = GetPalette(Species, PaletteData); 

        // Cycle through every Part in PartTypes
        for (int Index = 0; Index < PartTypes.Length; Index++)
        {
            CustomizablePart[] Part = Subtype.GetCustomizablePartArray(Index);
            if (Part.Length > 0)
            {
                CustomizablePart PartType = Part[PartTypes[Index]]; // Identifying the PartType

                // Cycle through each sprite in the parttype
                for(int PIndex = 0; PIndex < PartType.SpritesByOrder.Length; PIndex++)
                {
                    CustomSprite sprite = PartType.SpritesByOrder[PIndex];

                    Transform Angle = CharacterObject.transform.Find(sprite.Angle); // Finds the corresponding angle
                    Transform BoneParent = FindParentByName(Angle, sprite.PartName);// Finds the bone
                    GameObject SpritePart = GameObject.Instantiate(petDatabase.CustomizablePartPrefab,BoneParent); // Instantiates the sprite

                    SpriteRenderer spriteRenderer = SpritePart.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = sprite.Sprite; // Sets the graphical sprite
                    spriteRenderer.sortingOrder = sprite.SortOrder;// Sets the sort order
                }
            } 
        }
        SetPalette(CharacterObject,Palette);
        return CharacterObject;
    }

    public float[,] GetPalette(int species, int[] paletteData)
    {
        //paletteData corresponds to the palette index, aka pD[0] = coat index, pD[1] eyes index, pD[2] = patch index
        PaletteStorage paletteStorage = petDatabase.GetPaletteArray(species);
        float[,] colorArray = new float[3,3];
        
        float[] coatArray = paletteStorage.coatPalettes[paletteData[0]].Colors; // Selecting the coat Palette based on what index PaletteData[0] is
        float[] eyesArray = paletteStorage.eyePalettes[paletteData[1]].Colors; // selecting the eyes palette based on paletteData[1]
        float[] patchArray = paletteStorage.patchPalettes[paletteData[2]].Colors; // selecting the patch palette based on paletteData[2]

        // set each array to an hsv multiplier
        colorArray[0,0] = coatArray[0]; // coat color
        colorArray[0,1] = coatArray[1];
        colorArray[0,2] = coatArray[2];

        colorArray[1,0] = eyesArray[0]; // eye color
        colorArray[1,1] = eyesArray[1];
        colorArray[1,2] = eyesArray[2];

        colorArray[2,0] = patchArray[0]; // patch color
        colorArray[2,1] = patchArray[1];
        colorArray[2,2] = patchArray[2];

        return colorArray;
    }
    public void SetPalette(GameObject characterObject, float[,] palette)
    {
        foreach(VectorPaletteModifier PaletteMod in characterObject.GetComponentsInChildren<VectorPaletteModifier>()) // Looks for the palette modifier
        {
            for(int i = 0; i < 9; i++) // Basically mods colors 1 to 9, not touching 0 or 10
            {
                PaletteMod.Palette = palette;
            }
        }
    }
 

    public Transform FindParentByName(Transform P,string Name)
    {
        Transform Object = P;
        foreach(Transform Child in P.GetComponentsInChildren<Transform>())
        {
            if (Child.name == Name) Object = Child;
        }
        return Object;
    }
}
