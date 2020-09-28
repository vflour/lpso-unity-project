using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.PetSelect;

namespace Game.UI.PetSelect
{
    public class PSUIHandler : GameUI
    {
        int currentPSScreen = -1;
        
        public override void FirstScreen()
        {
            currentPSScreen = InstantiateScreen(1);
            InstantiateBackground(1);
        }

        // slot button action, either CRaP or go to map
        public void PlayAsPet(int saveIndex)
        {
            system.Emit("playAsCharacter",saveIndex);
        }
        public void CreatePet(int ticket)
        {
            system.Emit("createCharacter",ticket);
        }

        // increment the slot group
        public void IncrementPetSelect(int increment)
        {
            system.Emit("incrementPS",increment);
        }
        public void SetPetSelect(int index)
        {
            system.Emit("setPS",index);
        }

        // get the slot count 

        public void SetSlot(int slot)
        {
            PetSelectScreen screen = GetScreen(currentPSScreen).GetComponent<PetSelectScreen>();
            screen.CurrentSlot = slot; 
        }
        public void SetSlotGroup(Slot[] group)
        {

            PetSelectScreen screen = GetScreen(currentPSScreen).GetComponent<PetSelectScreen>();
            screen.Slots = group; 
        }

        // Getting data from server

        public CharacterData GetSave(int saveIndex)
        {
            CharacterData[] saves = (CharacterData[])system.Request("characterSaves");
            return saves[saveIndex];
        }

    }
}