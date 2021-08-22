using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI.PetSelect;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.PetSelect
{   
    public class PetSelectHandler : MonoBehaviour, IHandler
    {

[Header("Slot locations")]
        public Transform[] slotSpaces;
        public Transform  displaySpace;

[Header("Prefabs")]        
        public GameObject[] platformPrefabs;
        public GameObject[] displayPlatformPrefabs;
        public GameObject[] ticketPrefabs;
        
[Header("References")]      
        public CharacterHandler characterHandler; // gets the character Handler ONLY FOR THIS PURPOSE

 #region IHandler properties + methods       
        private GameSystem _system;
        public GameSystem system{get;   set;}
        public void Activate()
        {
            
        }
        private bool _display;
        public bool Display
        {
            get { return _display;}
            set
            {
                _display = value;
                if (_display)
                {
                    RequestCharacterData();
                    SetSlotsVisible(_display);
                }
            }
        }
 #endregion

#region Private fields
        private List<Slot[]> slots = new List<Slot[]>(); // all organized slots
        private CharacterData[] _characterData; // all characters that the player has

        public CharacterData[] characterData
        {
            get { return _characterData;}
            set{
                _characterData = value;
                SetSlots();
                LoadSlotGroup(0);
            }
        }
        private List<GameObject> loadedSlots = new List<GameObject>();
        private List<GameObject> loadedPlatforms = new List<GameObject>();

        private int currentSlotGroup;
        private int bronze; // bronze tickets + silver tickets
        private int silver;
#endregion
        private void RequestCharacterData() // requests the characterdata from the system and sets it
        {
            system.ServerDataRequest("getAllCharacters", data =>
            {
                characterData = JsonConvert.DeserializeObject<List<CharacterData>>(data.ToString()).ToArray();
                system.Store("characterData", characterData);
                PSUIHandler uiHandler = (PSUIHandler)system.GetHandler<GameUI>();
                uiHandler.SetCharacterCount(characterData.Length);
            });
        }
        private void SetSlots()
        {
            PlayerData playerData = (PlayerData) system.Request("playerData");
            bronze = playerData.bronze;
            silver = playerData.silver;

            List<Slot> allSlots = new List<Slot>();

            // sets the first two slots as tickets
            if(bronze!=0) allSlots.Add(new Slot(SlotType.BronzeTicket,0,PlatformType.Default));
            if(silver!=0) allSlots.Add(new Slot(SlotType.SilverTicket,0,PlatformType.Premium));

            // adds the remaining character slots
            for(int i = 0; i < characterData.Length; i++)
            {
                int petPlatform = 0;
                allSlots.Add(new Slot(SlotType.Pet,i,(PlatformType)petPlatform)); // adds a pet slot, + the index to its save
            }

            // sort the slots into miniature groups
            
            int totalGroups = (int)Mathf.Ceil(allSlots.Count/6.00f);
            int rest = allSlots.Count%6;

            int slotIndex = 0;
            for(int group = 0; group<totalGroups; group++)
            {
                // sets the amount of slots that should be allocated per group
                int groupCount = 6;
                // if there's a rest that's not 0, then set the group count to rest on final group
                if(rest != 0 && group == totalGroups-1) groupCount = rest;
                Slot[] slotGroup = new Slot[groupCount];
                for(int i = 0; i < groupCount; i++)
                {
                    slotGroup[i] = allSlots[slotIndex];
                    slotIndex++;
                } 
                slots.Add(slotGroup);
            }
        }


        private GameObject currentSlotDisplay;
        private GameObject currentPlatDisplay;
        public void SelectSlot(int i)
        {
            if (i >= slots[currentSlotGroup].Length) return;
            Slot slot = slots[currentSlotGroup][i];
            
            system.GetHandler<GameUI>().GetScreen("PetSelect").GetComponent<PetSelectScreen>().CurrentSlot = i;
            DisplaySlot(i);
        }

        private void DisplaySlot(int i)
        {
            RemoveDisplay();
            
            currentSlotDisplay = LoadSlotModel("_display",slots[currentSlotGroup][i]);
            currentPlatDisplay = GameObject.Instantiate(displayPlatformPrefabs[(int)slots[currentSlotGroup][i].platformType],displaySpace);
            
            SetSlotPosition(currentSlotDisplay, displaySpace, 0.1f);
            SetSlotPosition(currentPlatDisplay, displaySpace, 0);

            currentSlotDisplay.transform.localScale = new Vector3(0.5f,0.5f,0.5f); 
        }

        private void RemoveDisplay()
        {
            if(currentSlotDisplay)
            {
                if (characterHandler.HasCharacter(currentSlotDisplay.name))
                    characterHandler.RemoveCharacter(currentSlotDisplay.name);
                else
                    GameObject.Destroy(currentSlotDisplay);
                GameObject.Destroy(currentPlatDisplay);
            }
        }

        // Loads the current slot group
        private void LoadSlotGroup(int groupIndex)
        {
            // tells system to set the new slot group
            system.GetHandler<PSUIHandler>().SetSlotGroup(slots[groupIndex]);
            
            RemoveSlotGroup();
            currentSlotGroup = groupIndex;
            for(int i = 0; i < 6; i++)
            {
                // default platform
                int platformType = 0;
                
                if (i < slots[groupIndex].Length) // IF the slot has data
                {
                    Slot slot = slots[groupIndex][i];
                    platformType = (int) slot.platformType;
                    GameObject slotModel = LoadSlotModel("_slot",slot);
                    loadedSlots.Add(slotModel);
                    // parents the slot
                    float slotOffset = slot.type == SlotType.Pet ? 0.1f : 0.2f;
                    SetSlotPosition(loadedSlots[i], slotSpaces[i], slotOffset);
                }
                // instantiates and parents the platform
                GameObject platform = GameObject.Instantiate(platformPrefabs[platformType]);
                PSPlatform platformScript = platform.GetComponent<PSPlatform>();
                platformScript.petSelectHandler = this;
                platformScript.slot = i;
                loadedPlatforms.Add(platform);
                SetSlotPosition(loadedPlatforms[i], slotSpaces[i], 0);
            }

            SelectSlot(0);
        }

        private GameObject LoadSlotModel(string name, Slot slot)
        {
            GameObject sprite;
            switch(slot.type)
            {
                // instantiates the sprite based on the type
                case SlotType.BronzeTicket:
                    sprite = GameObject.Instantiate(ticketPrefabs[0]);
                    break;
                case SlotType.SilverTicket:
                    sprite = GameObject.Instantiate(ticketPrefabs[1]);
                    break;
                default:
                    sprite = characterHandler.AddCharacter(characterData[slot.saveIndex]._id+name,characterData[slot.saveIndex]).gameObject;
                    break;
            }

            return sprite;
        }

        // wipes the slot gameobjects completely
        private void RemoveSlotGroup()
        {
            if (loadedSlots.Count == 0) return;
            for(int i = 0; i<slots[currentSlotGroup].Length; i++)
            {
                Slot slot = slots[currentSlotGroup][i];
                
                GameObject obj = loadedSlots[i];
                GameObject platform = loadedPlatforms[i];              

                // removes the character if they are one
                if(slot.type == SlotType.Pet) obj.GetComponent<Character>().Remove();
                else GameObject.Destroy(obj);

                // removes platform
                GameObject.Destroy(platform);
            }

            // resets lists to blank
            loadedSlots = new List<GameObject>();
            loadedPlatforms = new List<GameObject>();
        }
        

        // sets the position of a slot or platform by parenting them to a location
        private void SetSlotPosition(GameObject slotObject, Transform slotSpace, float yOffset)
        {
            slotObject.transform.SetParent(slotSpace);
            slotObject.transform.localPosition = new Vector3(0,yOffset,0);
        }
        
        //sets the slots+platform's visibility
        private void SetSlotsVisible(bool visible)
        {
            foreach(Transform slot in slotSpaces)
                slot.gameObject.SetActive(visible);
            displaySpace.gameObject.SetActive(visible);
            
        }
        
        // modifies the slot group
        public void IncrementSlotGroup(int i)
        {
           LoadSlotGroup(currentSlotGroup+i);   
        }
        public void SetSlotGroup(int group)
        {
            LoadSlotGroup(group);  
        }
    }
    

    // quick reference to the slots. mostly meant to organize which types there are
    public enum SlotType{BronzeTicket,SilverTicket,Pet}
    public enum PlatformType{Default,Premium}
    public class Slot
    {
        public Slot(SlotType type,int saveIndex, PlatformType platform)
        {
            this.type = type;
            this.saveIndex = saveIndex;
            this.platformType = platform;
        }
        public SlotType type; // slot type of the slot
        public int saveIndex; // points to a specific save value if it's a character
        public PlatformType platformType;
    }
}


