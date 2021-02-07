using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

namespace Game.CrAP
{

    public enum CrAPSection{Species,SubSpecies,Modification}
    public class CrAPHandler : MonoBehaviour, IHandler
    {   
[Header("Species count")]
        public int speciesCount = 1;

        // Create a pet section property
        private CrAPSection _crapSection;
        public CrAPSection crapSection 
        {
            get{return _crapSection;}
            set
            {
                _crapSection = value;
                switch(_crapSection)
                {
                case CrAPSection.Species:
                        SpeciesSection();
                        break;
                    case CrAPSection.SubSpecies:
                        SubSpeciesSection();
                        break;
                    case CrAPSection.Modification:
                        CharacterModSection();
                        break;
                }
            }
        } 

#region Private fields
        private CharacterHandler characterHandler;
        private PetDatabase petDatabase;

        // loaded instances of the character
        private CharacterData characterData;
        private Character CurrentPet;
        private List<Character> LoadedCrAPs = new List<Character>();

        private int selectedPet;
        private int maxPets;
#endregion

#region Handler fields
            private GameSystem _system; // basic reference to system
            public GameSystem system
            {
                get{return _system;}
                set{_system = value;}
            }
        // Initialization
        public void Activate()
        {
            characterHandler = GameObject.Find("CharacterHandler").GetComponent<CharacterHandler>();
            petDatabase = characterHandler.petDatabase;
            crapSection = CrAPSection.Species;
            system.Emit("setPetDatabase", petDatabase);
        }
        
        private bool _display;
        public bool Display
        {
            get { return _display;}
            set { _display = value; }
        }
#endregion

#region CrAP sections

        private int[] blankPalette =  { 0, 0, 0 };
        private int[] blankParts = { 0, 0, 0, 0, 0, 0 };

        private void SpeciesSection()
        {
            characterData= new CharacterData();
            maxPets = speciesCount;
            ClearAll();
            // generate all species in list and put them on display
            for(int species = 0; species < speciesCount; species++) 
            {
                AddPet(blankPalette,species,0,blankParts);
            }
            selectedPet = 0;
            FocusOnPet();
        }

        private void SubSpeciesSection()
        {
            // get subspecies count + current species
            int species = characterData.species;
            int subSpeciesCount = petDatabase.GetSpeciesArray(species).Length;
            maxPets = subSpeciesCount;
            
            // generate all subspecies in list
            ClearAll();
            for(int subSpecies = 0; subSpecies < subSpeciesCount; subSpecies++) 
            {
                AddPet(blankPalette,species,subSpecies,blankParts);
            }
            selectedPet = 0;
            FocusOnPet();
        }

        private void CharacterModSection()
        {
            // set the display + instantiate screen
            SelectPet();
        }
#endregion

#region Loading and Instantiating sprites

        public Character AddPet(int [] Palette, int SpeciesInt, int SubSpecies, int[] PartTypes)
        {
            // create character class
            CharacterData character = new CharacterData();
            character.palette = Palette;
            character.species = SpeciesInt;
            character.speciesSubtype =  SubSpecies;
            character.parts = PartTypes;

            // Generates the sprite
            Character Pet = characterHandler.AddCharacter(character);
            LoadedCrAPs.Add(Pet); // Adds the model to the list
            return Pet;
        }

        // destroys all instances of characters
        public void ClearAll()
        {
            foreach(Character obj in LoadedCrAPs)
                GameObject.Destroy(obj.gameObject);
            
            if(CurrentPet) GameObject.Destroy(CurrentPet.gameObject);

            LoadedCrAPs = new List<Character>(); // Resets the LoadedCrAPs table to a blank list
            characterData = new CharacterData(); // wipes the characterData slot
        }

        // Focus on the new selectedpet
        public void FocusOnPet()
        {
            
            // Sets the next pet. If the current pet is the last pet, then the next pet is equal to 0
            int NextPet = selectedPet + 1 >= LoadedCrAPs.Count ? 0 : selectedPet + 1;
            // Sets the previous pet. If current pet is 0, then the previous pet is the last pet to be presented.
            int PreviousPet = selectedPet - 1 < 0 ? LoadedCrAPs.Count - 1 : selectedPet - 1;

            // Sets everyone to invisible
            foreach(Character Pet in LoadedCrAPs)
            {
                Pet.gameObject.SetActive(false);
            }

            // set to active
            LoadedCrAPs[PreviousPet].gameObject.SetActive(true);
            LoadedCrAPs[NextPet].gameObject.SetActive(true);
            LoadedCrAPs[selectedPet].gameObject.SetActive(true);

            // Set position
            LoadedCrAPs[PreviousPet].transform.position = new Vector3();
            LoadedCrAPs[NextPet].transform.position = new Vector3();
            LoadedCrAPs[selectedPet].transform.position = new Vector3();

            // Set size
            LoadedCrAPs[PreviousPet].transform.localScale = new Vector3(1,1,1);
            LoadedCrAPs[NextPet].transform.localScale = new Vector3(1,1,1);
            LoadedCrAPs[selectedPet].transform.localScale = new Vector3(2,2,2);
            
            CurrentPet = LoadedCrAPs[selectedPet];
        }
#endregion

#region Selecting Pets
        public void ChangeSelection(int i)
        {
            selectedPet = i;
            SetSelection();
            FocusOnPet();
        }
        public void IncrementSelection(int i)
        {
            selectedPet+=i;
            SetSelection();
            FocusOnPet();
        }
        public void SetSelection()
        {
            if(selectedPet>=maxPets) selectedPet = 0;
            else if(selectedPet < 0) selectedPet = maxPets-1;
        }
        public void SelectSpecies()
        {
            characterData.species = selectedPet;
        }
        public void SelectSubSpecies()
        {
            characterData.speciesSubtype = selectedPet;
        }

        public void SelectPet() // Selects the pet and sends them to the Modifcation section
        {
            CurrentPet = LoadedCrAPs[selectedPet];

            for(int i = 0; i < LoadedCrAPs.Count; i++)
            {
                if (i != selectedPet) // Destroys all pets except the selected one
                {
                    Destroy(LoadedCrAPs[i].gameObject);
                }
            }
            LoadedCrAPs = new List<Character>(); // Resets the LoadedCrAPs table to a blank list
        }
#endregion

#region Modification section        
        public void SetPart(int Part, int PartType)
        {
            ClearAll(); // Destroys the old pet
            characterData.parts[Part] = PartType; // Sets a new part type

            // Generates a new sprite
            CurrentPet = AddPet(characterData.palette,characterData.species,characterData.speciesSubtype,characterData.parts); 
        }

        public void SetPalette(int i, int v)
        {
            characterData.palette[i] = v;
            Color[] colorPalette = characterHandler.GetPalette(characterData.species, characterData.palette);
            characterHandler.SetPalette(characterHandler.GetIndex(CurrentPet), colorPalette);
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
        public void SetSpecies(int species)
        {
            characterData.species = species;
        }
        public void SetSubSpecies(int subSpecies)
        {
            characterData.speciesSubtype = subSpecies;
        }
        public void SetGender(int gender)
        {
            characterData.gender = gender;
        }
        public void SetName(string name)
        {
            characterData.name = name;
        }
        public void SaveCharacterData()
        {
            characterData.ticket = system.Request<int>("currentTicket");
            system.Emit("setPetData",characterData);
        }
#endregion

    }
}

