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
        PaletteColor[] Palette = GetPalette(Species, PaletteData); 

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
                    GameObject SpritePart = GameObject.Instantiate(sprite.Sprite,BoneParent); // Instantiates the sprite
                    
                }
            } 
        }
        SetPalette(CharacterObject,Palette);
        return CharacterObject;
    }

    public PaletteColor[] GetPalette(int species, int[] paletteData)
    {
        //paletteData corresponds to the palette index, aka pD[0] = coat index, pD[1] eyes index, pD[2] = patch index
        PaletteStorage paletteStorage = petDatabase.GetPaletteArray(species);
        PaletteColor[] colorArray = new PaletteColor[3];
        
        colorArray[0] = paletteStorage.coatPalettes[paletteData[0]];
        colorArray[1] = paletteStorage.patchPalettes[paletteData[1]];
        colorArray[2] = paletteStorage.eyePalettes[paletteData[2]];
        
        return colorArray;
    }
    public void SetPalette(GameObject characterObject, PaletteColor[] palette)
    {
        foreach(VectorPaletteModifier paletteMod in characterObject.GetComponentsInChildren<VectorPaletteModifier>()) // Looks for the palette modifier
        {
            paletteMod.Palette = palette;
        }
    }
 

    public static Transform FindParentByName(Transform P,string Name)
    {
        Transform Object = P;
        foreach(Transform Child in P.GetComponentsInChildren<Transform>())
        {
            if (Child.name == Name) Object = Child;
        }
        return Object;
    }
}
