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

    public GameObject GenerateCrAPSprite(int[] PaletteData, int Species, int speciesSubtype, int[] PartTypes, Transform Parent)
    {
        // Find the species array
        SpeciesSubtype[] SpeciesArray = petDatabase.GetSpeciesArray(Species);
        SpeciesSubtype Subtype = SpeciesArray[speciesSubtype];
        
        // Instantiate the subtype's body
        GameObject CharacterObject = GameObject.Instantiate(Subtype.BodyPrefab, Parent);

        // sets the actual color palette based on palette data
        Color[] Palette = SetPalette(Species, PaletteData); 

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
        
        ColorPetAccordingToPalette(CharacterObject, Palette);
        return CharacterObject;
    }

    public Color[] SetPalette(int species, int[] paletteData)
    {
        //paletteData corresponds to the palette index, aka pD[0] = coat index, pD[1] eyes index, pD[2] = patch index
        PaletteStorage paletteStorage = petDatabase.GetPaletteArray(species);
        Color[] colorArray = new Color[9];
        
        Color[] coatArray = paletteStorage.coatPalettes[paletteData[0]].Colors; // Selecting the coat Palette based on what index PaletteData[0] is
        Color[] eyesArray = paletteStorage.eyePalettes[paletteData[1]].Colors; // selecting the eyes palette based on paletteData[1]
        Color[] patchArray = paletteStorage.patchPalettes[paletteData[2]].Colors; // selecting the patch palette based on paletteData[2]

        // bluh im assigning them manually cuz coatarray[4] is a weird outliar
        colorArray[0] = coatArray[0]; // coat color
        colorArray[1] = coatArray[1];
        colorArray[2] = coatArray[2];
        colorArray[5] = coatArray[3]; // eyelash color

        colorArray[3] = eyesArray[0]; // eye color
        colorArray[4] = eyesArray[1];

        colorArray[6] = patchArray[0]; // patch color
        colorArray[7] = patchArray[1];
        colorArray[8] = patchArray[2];

        return colorArray;
    }

    public void ColorPetAccordingToPalette(GameObject Pet,Color[] Palette)
    {
        foreach(VectorPaletteModifier PaletteMod in Pet.GetComponentsInChildren<VectorPaletteModifier>()) // Looks for the palette modifier
        {
            for(int i = 0; i < 9; i++) // Basically mods colors 1 to 9, not touching 0 or 10
            {
                ///Color[] NewPalette = PaletteMod.Palette;
                ///NewPalette[i + 1] = Palette[i];
               /// PaletteMod.Palette = NewPalette; // honestly just making sure the get function triggers 
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
