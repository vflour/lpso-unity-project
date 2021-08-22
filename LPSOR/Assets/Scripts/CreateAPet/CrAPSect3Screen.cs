using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.CrAP;
using UnityEngine.UI;

namespace Game.UI.CRaP
{
    public class CrAPSect3Screen : GameScreen
    {
#region Generic Section Handling
        // Gets and queues the pet database
        private PetDatabase _petDB;
        private CrAPHandler crapHandler;
        
        public PetDatabase PetDB
        {
            get
            {
                if (_petDB == null) 
                    _petDB = crapHandler.petDatabase;
                return _petDB;
            }
        }
        // Create! Button
        public Button saveButton;
        public void SavePet()
        {
            ToggleInput(false);
            crapHandler.Name = Name;
            crapHandler.SaveCharacterData();
            gameUI.system.Emit("loadScene",3);
        }

        // Back button
        public void PreviousSection()
        {
            CrAPUI uiHandler = gameUI as CrAPUI;
            uiHandler.PreviousSection(); 
        }
#endregion
#region Section 3 Modifiers
        private byte _currentModifier = 0; // modifier isnt a strict binary
[Header("General Section 3 References")] 
        public GameObject[] modifierUIObjects;
        public Button continueButton;
        public byte CurrentModifier
        {
            get { return _currentModifier; }
            set
            {
                _currentModifier = value;
                foreach(GameObject ui in modifierUIObjects)
                    ui.SetActive(false);
                modifierUIObjects[_currentModifier].SetActive(true);
                SetPaletteButton();
            }
        }
        // Initialization
        public void Start()
        {
            crapHandler = gameUI.system.GetHandler<CrAPHandler>();
            GenerateParts();
            GeneratePalettes();
        }

        public void AddButtons()
        {
            buttons.Add(saveButton);
        }

        // get current ticket
        public int CurrentTicket
        {
            get { return gameUI.system.gameData.currentTicket; }
        }
#endregion
#region Part Modification
        public PartModifierButton[] partModButtons = new PartModifierButton[6];
        private int currentPartModifier = 0;

        public void SetPart(int partType, int partValue)
        {
            if (CurrentModifier != 0) CurrentModifier = 0;
            currentPartModifier = partType;
            
            int[] partModArray = new int[] {currentPartModifier, partValue};
            crapHandler.SetPart(currentPartModifier,partValue);
            for (int pbIndex = 0; pbIndex < partModButtons.Length; pbIndex++)
                partModButtons[pbIndex].Selected = (currentPartModifier == pbIndex);
        }
        private void GenerateParts()
        {
            for (int partModIndex = 0; partModIndex < 6; partModIndex++)
            {
                // gets all the parts for the specific part type
                
                CustomizablePart[] partsList = PetDB.GetSpeciesArray(crapHandler.Species)[crapHandler.SubSpecies].GetCustomizablePartArray(partModIndex);
                
                // sets which parts are accessible based on ticket
                // my condolences to the server developer
                int accessiblePartCount = 0;
                foreach(CustomizablePart partValue in partsList)
                    if (partValue.Ticket <= CurrentTicket)
                        accessiblePartCount++;

                PartModifierButton partModButton = partModButtons[partModIndex];
                partModButton.maxPartValue = accessiblePartCount;
                partModButton.partType = partModIndex;
                partModButton.sect3Screen = this;
                buttons.Add(partModButton.GetComponent<Button>());
            }
            partModButtons[0].PartValue = 0;
        }
        public void IncrementPartModifier()
        {
            PartModifierButton partButton = partModButtons[currentPartModifier];
            partButton.IncrementIndex();
        }
        public void DecrementPartModifier()
        {
            PartModifierButton partButton = partModButtons[currentPartModifier];
            partButton.DecrementIndex();
        }
#endregion
#region Palette Modification
[Header("Palette Modification")]
        public GameObject[] paletteModContainers;
        public ArrowList[] paletteModArrowLists;
        public GameObject paletteColorSprite;
        public GameObject paletteMirrorSprite;
        
[Header("Palette Mod Button")]
        public GameObject paletteModButton;
        public Sprite[] paletteButtonSprites;
        
        private List<GameObject>[] activePaletteButtons = new List<GameObject>[3];
        
        // Modifies the palette value
        public void SetPaletteSection(int paletteIndex, int paletteValue)
        {
            crapHandler.SetPalette(paletteIndex, paletteValue);

            // Set the palette mirror sprite color
            PaletteColor[] paletteArray = PetDB.GetPaletteArray(crapHandler.Species).getTypePalette(paletteIndex);
            Color paletteColor = paletteArray[paletteValue].color;
            paletteMirrorSprite.GetComponent<Image>().color = paletteColor;
            
            // Set the active palette button
            foreach (GameObject button in activePaletteButtons[paletteIndex])
            {
                // Set selected based on whether or not its value is equal to the current palette value
                PaletteModifierButton buttonObject = button.GetComponent<PaletteModifierButton>();
                buttonObject.Selected = (buttonObject.Value == paletteValue);
            }
        }
        // Open palette modifier
        public void PaletteButton()
        {
            for (int pbIndex = 0; pbIndex < partModButtons.Length; pbIndex++)
                partModButtons[pbIndex].Selected = false;
            CurrentModifier = 1;
        }

        public void SetPaletteButton()
        {
            if (CurrentModifier==1)
            {
                paletteModButton.GetComponent<Image>().sprite = paletteButtonSprites[0];
                paletteModButton.transform.localScale = new Vector3(1.25f,1.25f,1.25f);
            }
            else
            {
                paletteModButton.GetComponent<Image>().sprite = paletteButtonSprites[1];
                paletteModButton.transform.localScale = new Vector3(1,1,1);
            }
        }
        // Generate color palettes
        private void GeneratePalettes()
        {
            for(int containerIndex = 0; containerIndex < 3; containerIndex++)
            {
                // get palette from database
                PaletteStorage paletteStorage = PetDB.GetPaletteArray(crapHandler.Species);
                PaletteColor[] paletteArray = paletteStorage.getTypePalette(containerIndex);
                
                // create a new paletteobject list
                List<GameObject> PaletteObjects = new List<GameObject>();
                // adds palette buttons
                for(int hsvIndex = 0; hsvIndex<paletteArray.Length; hsvIndex++)
                {
                    GameObject paletteButton = GameObject.Instantiate(paletteColorSprite,paletteModContainers[containerIndex].transform);
                    PaletteModifierButton paletteModComponent = paletteButton.GetComponent<PaletteModifierButton>();
                    paletteModComponent.Index = containerIndex;
                    paletteModComponent.Value = hsvIndex;
                    paletteModComponent.sect3Screen = this;
                    
                    // sets the sprite color using the color field
                    Color paletteColor = paletteArray[hsvIndex].color;
                    paletteButton.GetComponent<Image>().color = paletteColor;

                    PaletteObjects.Add(paletteButton);
                }
                
                // Initialize Arrowlist + Active Palette buttons
                paletteModArrowLists[containerIndex].Initialize(PaletteObjects.ToArray());
                activePaletteButtons[containerIndex] = PaletteObjects;
                
                SetPaletteSection(containerIndex, 0);
            }
        }
        #endregion
#region Misc Modification
        private int currentGender = 0;
[Header("Gender Modification")]
        public Sprite[] genderButtonSprites;
        public GameObject genderButton;
        
        public void GenderButton()
        {
            currentGender = (int)Mathf.Repeat(currentGender+1, 2);
            crapHandler.Gender = currentGender;
            genderButton.GetComponent<Image>().sprite = genderButtonSprites[currentGender];
        } 
[Header("Name Modification")] 
        public InputField nameField;
        private string Name
        {
            get { return nameField.text; }
        }
        // Method setting the interactable property when the Name is at least 2 characters
        public void ContinueButtonInteraction()
        {
            continueButton.interactable = (Name.Length > 1);
        }
        #endregion
    }
}