using System.Collections;
using System.Collections.Generic;
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


 #region IHandler properties + methods       
        private GameSystem _system;
        public GameSystem system{get;   set;}
        public void Activate()
        {
            RequestCharacterData();

        }

        public void SetCharacterData(CharacterData[] characterData)
        {
            this.characterData = characterData;
            SetSlots();
            LoadSlotGroup(1);          
        }
#endregion

#region Private fields
        private List<Slot[]> slots = new List<Slot[]>(); // all organized slots
        private CharacterData[] characterData; // all characters that the player has
        private List<GameObject> loadedSlots = new List<GameObject>();
        private List<GameObject> loadedPlatforms = new List<GameObject>();

        private CharacterHandler characterHandler; // gets the character Handler ONLY FOR THIS PURPOSE

        private int currentSlotGroup;
        private int bronze; // bronze tickets + silver tickets
        private int silver;
#endregion
        private void RequestCharacterData() // requests the characterdata from the system and sets it
        {
            system.Emit("requestCharacters",null);
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
            int totalGroups = (int)Mathf.Ceil(allSlots.Count/6);
            int rest = (int)allSlots.Count%6;

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
            Slot slot = slots[currentSlotGroup][i];

            system.Emit("setCurrentSlot",i);
            DisplaySlot(i);
        }

        private void DisplaySlot(int i)
        {
            RemoveDisplay();

            currentSlotDisplay = GameObject.Instantiate(loadedSlots[i]);
            currentPlatDisplay = GameObject.Instantiate(displayPlatformPrefabs[i]);

            currentPlatDisplay.transform.SetParent(displaySpace);
            currentSlotDisplay.transform.SetParent(displaySpace);

            currentPlatDisplay.transform.localPosition = new Vector3(0,0,0);
            currentSlotDisplay.transform.localPosition = new Vector3(0,2,0);          
        }

        private void RemoveDisplay()
        {
            if(currentSlotDisplay)
            {
                GameObject.Destroy(currentSlotDisplay);
                GameObject.Destroy(currentPlatDisplay);
            }
        }

        // Loads the current slot group
        private void LoadSlotGroup(int groupIndex)
        {
            RemoveSlotGroup();
            currentSlotGroup = groupIndex;
            for(int i = 0; i < slots[groupIndex].Length; i++)
            {
                Slot slot = slots[groupIndex][i];
                switch(slot.type)
                {
                    // instantiates the sprite based on the type
                    case SlotType.Pet:
                        GameObject sprite = characterHandler.AddCharacter(characterData[slot.saveIndex]).gameObject;
                        loadedSlots.Add(sprite);
                        break;
                    case SlotType.BronzeTicket:
                        sprite = GameObject.Instantiate(ticketPrefabs[0]);
                        loadedSlots.Add(sprite);
                        break;
                    case SlotType.SilverTicket:
                        sprite = GameObject.Instantiate(ticketPrefabs[1]);
                        loadedSlots.Add(sprite);
                        break;
                }

                // instantiates the platform
                GameObject platform = GameObject.Instantiate(platformPrefabs[(int)slot.platformType]);
                loadedPlatforms.Add(platform);

                // tells system to set the new slot group
                system.Emit("setSlotGroup",slots[groupIndex]);
                // parents the sprite
                SetSlotPosition(i);
            }
        }

        // wipes the slot gameobjects completely
        private void RemoveSlotGroup()
        {
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
        

        // sets the position by parenting them to a location
        private void SetSlotPosition(int index)
        {
            GameObject slot = loadedSlots[index];
            GameObject platform = loadedPlatforms[index];
            
            platform.transform.SetParent(slotSpaces[index]);
            slot.transform.SetParent(slotSpaces[index]);

            platform.transform.localPosition = new Vector3(0,0,0);
            slot.transform.localPosition = new Vector3(0,2,0);
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


