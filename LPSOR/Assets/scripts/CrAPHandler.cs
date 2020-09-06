using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class CrAPHandler : MonoBehaviour
{
    public GameObject UISpace;
    public GameObject SceneSpace;
    public int speciesCount = 2; // There are currently two species.

    #region Loading and Instantiating sprites

    private CharacterData characterData;

    public PetDatabase petDatabase;
    public PetSpriteGenerator petGen;

    public void LoadAllPets()
    {
        petGen = new PetSpriteGenerator(petDatabase);

        // Loops through the species count to generate each default species.
        for(int SpeciesInt = 0; SpeciesInt < speciesCount; SpeciesInt++) 
        {
            int SubSpecies = 0; // Subspecies i 0 by default.
            int[] Palette = { 0, 0, 0 }; // sets the default palette
            int[] PartTypes = { 0, 0, 0, 0, 0, 0 }; // Sets part types to 0
            // Generates the sprite
            GameObject Pet = petGen.GenerateCrAPSprite(Palette, SpeciesInt, SubSpecies, PartTypes, SceneSpace.transform.Find("CrAP"));
            LoadedCrAPs.Add(Pet); // Adds the model to the list
        }
        SelectedPet = 0;
        FocusOnPet();
    }

    public void ClearAllPets()
    {
        foreach(GameObject Child in SceneSpace.transform.Find("CrAP").GetComponentsInChildren<GameObject>())
        {
            if (Child.name != "CrAP") Destroy(Child);
        }
        LoadedCrAPs = new List<GameObject>(); // Resets the LoadedCrAPs table to a blank list
        characterData = new CharacterData(0); // wipes the characterData slot
    }


    private int SelectedPet;
    GameObject CurrentPet;
    private List<GameObject> LoadedCrAPs;
    public void FocusOnPet()
    {

        // Sets the next pet. If the current pet is the last pet, then the next pet is equal to 0
        int NextPet = SelectedPet + 1 >= LoadedCrAPs.Count ? 0 : SelectedPet + 1;
        // Sets the previous pet. If current pet is 0, then the previous pet is the last pet to be presented.
        int PreviousPet = SelectedPet - 1 < 0 ? LoadedCrAPs.Count - 1 : SelectedPet - 1;

        // Sets everyone to invisible
        foreach(GameObject Pet in LoadedCrAPs)
        {
            Pet.SetActive(false);
        }

        // set to active
        LoadedCrAPs[PreviousPet].SetActive(true);
        LoadedCrAPs[NextPet].SetActive(true);
        LoadedCrAPs[SelectedPet].SetActive(true);

        // Set position
        LoadedCrAPs[PreviousPet].transform.position = new Vector3();
        LoadedCrAPs[NextPet].transform.position = new Vector3();
        LoadedCrAPs[SelectedPet].transform.position = new Vector3();

        // Set size
        LoadedCrAPs[PreviousPet].transform.localScale = new Vector3();
        LoadedCrAPs[NextPet].transform.localScale = new Vector3();
        LoadedCrAPs[SelectedPet].transform.localScale = new Vector3();

    }

    public void SelectPet()
    {
        CurrentPet = LoadedCrAPs[SelectedPet];
        characterData = new CharacterData(SelectedPet); // creates a new characterData class

        for(int i = 0; i < LoadedCrAPs.Count; i++)
        {
            if (i != SelectedPet) // Destroys all pets except the selected one
            {
                Destroy(LoadedCrAPs[i]);
            }
        }
        LoadedCrAPs = new List<GameObject>(); // Resets the LoadedCrAPs table to a blank list
    }

    
    public void ChangePetPart(int Part, int PartType)
    {
        Destroy(CurrentPet); // Destroys the old pet
        characterData.parts[Part] = PartType; // Sets a new part type

        // Generates a new sprite
        CurrentPet = petGen.GenerateCrAPSprite(characterData.palette, characterData.species, characterData.speciesSubtype, characterData.parts, SceneSpace.transform.Find("CrAP")); 
    }

    void SetPalette(int i, int v)
    {
        characterData.palette[i] = v;
        Color[] colorPalette = petGen.SetPalette(characterData.species, characterData.palette);
        petGen.ColorPetAccordingToPalette(CurrentPet, colorPalette);
    }

    public void CoatColorSelect(int index) // Sets the coat color according to the palette index
    {
        SetPalette(0,index);
    }

    public void EyecolorSelect(int index) // Sets the eyecolor according to the palette index
    {
        SetPalette(1,index);
    }

    public void PatchcolorSelect(int index) // Sets the patch color according to the palette index
    {
        SetPalette(2, index);
    }
    #endregion

    #region Setting Data values
    public void SetName()
    {

    }

    public void CreatePet()
    {

    }

    #endregion

    #region UI Functions
    static public void ClearUISpace(GameObject Parent) // Destroys every object in the UI space
    {
        string ParentName = Parent.name;
        foreach (GameObject UI in Parent.GetComponentsInChildren<GameObject>())
        {
            if(UI.name != ParentName)
            {
                Destroy(UI);
            }
        }
    }

    GameObject SpeciesSelectPrefab;
    public void LoadSpeciesSelect()
    {
        ClearUISpace(UISpace);
    }

    GameObject SpeciesSubSelectPrefab;
    public void LoadSpeciesSubSelect()
    {
        ClearUISpace(UISpace);
    }

    GameObject PetCustomPrefab;
    public void PetCustomSelect()
    {
        ClearUISpace(UISpace);
    }

    #endregion

}
