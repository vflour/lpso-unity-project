using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Game.CrAP
{

    public enum CrAPSection{Species,SubSpecies,Modification}
    public class CrAPHandler : MonoBehaviour, IHandler
    {   
[Header("Species count")]
        public int speciesCount = 2;
       
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
        public PetDatabase petDatabase;
        
        // loaded instances of the character
        private CharacterData characterData;
        private Character currentPet;
        private List<Character> loadedPets = new List<Character>();
        private Dictionary<Character, GameObject> pedestals= new Dictionary<Character, GameObject>();

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
            characterHandler = system.GetHandler<CharacterHandler>();
            petDatabase = characterHandler.petDatabase;
            crapSection = CrAPSection.Species;
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
                AddPet(blankPalette,species,0,blankParts,0);
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
                AddPet(blankPalette,species,subSpecies,blankParts, 1);
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
        
        public GameObject petPlatform;
        public GameObject subSpeciesPlatform;
        
        public Character AddPet(int [] Palette, int SpeciesInt, int SubSpecies, int[] PartTypes, byte pedestalType)
        {
            // create character class
            CharacterData character = new CharacterData();
            character.palette = Palette;
            character.species = SpeciesInt;
            character.speciesSubtype =  SubSpecies;
            character.parts = PartTypes;

            // Generates the sprite and platform
            GameObject pedestal = pedestalType == 0 ? Instantiate(petPlatform,transform) : Instantiate(subSpeciesPlatform,transform) ;
            
            Character pet = characterHandler.AddCharacter($"Species{SpeciesInt}Sub{SubSpecies}",character);
            pet.transform.parent = pedestal.transform;
            pet.transform.localScale = new Vector3(1, 1, 1);
            pet.transform.localPosition = Vector3.zero;
            pedestals.Add(pet, pedestal);
            loadedPets.Add(pet); // Adds the model to the list
            return pet;
        }

        // destroys all instances of characters
        public void ClearAll()
        {
            foreach(Character character in loadedPets)
                if (characterHandler.HasCharacter(character.name))
                {
                    character.Remove();
                    Destroy(pedestals[character]);
                }

            if (currentPet && characterHandler.HasCharacter(currentPet.name))
            {
                currentPet.Remove();
                Destroy(pedestals[currentPet]);
            }
            loadedPets = new List<Character>(); // Resets the LoadedCrAPs table to a blank list
            
        }

        // Focus on the new selectedpet
        public void FocusOnPet()
        {
            int nextPet = (int)Mathf.Repeat(selectedPet + 1, loadedPets.Count);
            int previousPet = (int)Mathf.Repeat(selectedPet - 1, loadedPets.Count);

            foreach(Character pet in loadedPets)
            {
                pet.gameObject.SetActive(false);
            }
            currentPet = loadedPets[selectedPet];
            // set to active
            loadedPets[previousPet].gameObject.SetActive(true);
            loadedPets[nextPet].gameObject.SetActive(true);
            loadedPets[selectedPet].gameObject.SetActive(true);
        }

        // Shift Pedestals into place
        public IEnumerator ShiftPedestals(int increment)
        {
            // Sets the next pet. If the current pet is the last pet, then the next pet is equal to 0
            int nextPet = (int)Mathf.Repeat(selectedPet + 1, loadedPets.Count);
            // Sets the previous pet. If current pet is 0, then the previous pet is the last pet to be presented.
            int previousPet = (int)Mathf.Repeat(selectedPet - 1, loadedPets.Count);
            // Pet that loads out of the scene. Usually 2 steps ahead in the increment direction
            int vanishingPet = (int)Mathf.Repeat(selectedPet + Mathf.Sign(increment)*2 , loadedPets.Count);
            loadedPets[vanishingPet].gameObject.SetActive(true); // temp vanishing pet visible
            
            // Animate the pedestals
            string direction = increment > 0 ? "Left" : "Right";
            MovePedestals(loadedPets[vanishingPet], direction, 1);
            MovePedestals(loadedPets[previousPet], direction, -Mathf.Sign(increment));
            MovePedestals(loadedPets[selectedPet], direction, -Mathf.Sign(increment));
            MovePedestals(loadedPets[nextPet], direction, Mathf.Sign(increment));
            
            yield return new WaitForSeconds(0.5f);
            loadedPets[vanishingPet].gameObject.SetActive(false); // phase out the vanishing pet
        }

        public void MovePedestals(Character character, string direction, float reverse)
        {
            Animator animator = pedestals[character].GetComponent<Animator>();
            animator.SetFloat("ShiftDirection",reverse);
            animator.SetTrigger(direction);
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
            StartCoroutine(ShiftPedestals(i));
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
            currentPet = loadedPets[selectedPet];

            for(int i = 0; i < loadedPets.Count; i++)
            {
                if (i != selectedPet) // Destroys all pets except the selected one
                {
                    Destroy(loadedPets[i].gameObject);
                }
            }
            loadedPets = new List<Character>(); // Resets the LoadedCrAPs table to a blank list
        }
#endregion

#region Modification section        
        public void SetPart(int Part, int PartType)
        {
            ClearAll(); // Destroys the old pet
            characterData.parts[Part] = PartType; // Sets a new part type

            // Generates a new sprite
            currentPet = AddPet(characterData.palette,characterData.species,characterData.speciesSubtype,characterData.parts, 0); 
        }

        public void SetPalette(int i, int v)
        {
            characterData.palette[i] = v;
            PaletteColor[] colorPalette = characterHandler.GetPalette(characterData.species, characterData.palette);
            characterHandler.SetPalette(currentPet.name, colorPalette);
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
        public int Species
        {
            get => characterData.species;
            set => characterData.species = value;
        }
        public int SubSpecies
        {
            get => characterData.speciesSubtype;
            set => characterData.speciesSubtype = value;
        }

        public int Gender
        {
            get => characterData.gender;
            set => characterData.gender = value;
        }
        public string Name
        {
            get => characterData.name;
            set => characterData.name = value;
        }
        public void SaveCharacterData()
        {
            characterData.ticket = system.gameData.currentTicket;
            system.gameData.sessionData = characterData;
            system.ServerDataRequest("newCharacter", JsonUtility.ToJson(characterData), data => {
                string charId = (string) data;
                system.gameData.sessionData._id = charId;
                characterData._id = charId;
                system.gameData.sessionData.userName = system.gameData.playerData.userName;
            });
        }
#endregion

    }
}

