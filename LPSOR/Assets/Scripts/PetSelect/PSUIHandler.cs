using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.PetSelect;

namespace Game.UI.PetSelect
{
    public class PSUIHandler : GameUI
    {
        // increment the slot group
        public void IncrementPetSelect(int increment)
        {
            system.Emit("incrementPS",increment);
        }
        public void SetPetSelect(int index)
        {
            system.Emit("setPS",index);
        }
        // set the total slot count
        public void SetCharacterCount(int count)
        {
            PetSelectScreen screen = firstScreen.GetComponent<PetSelectScreen>();
            screen.characterCount = count;
        }
        // get the slot count 
        public void SetSlot(int slot)
        {
            PetSelectScreen screen = firstScreen.GetComponent<PetSelectScreen>();
            screen.CurrentSlot = slot; 
        }
        public void SetSlotGroup(Slot[] group)
        {

            PetSelectScreen screen = firstScreen.GetComponent<PetSelectScreen>();
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