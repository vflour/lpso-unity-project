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
            system.GetHandler<PetSelectHandler>().IncrementSlotGroup(increment);
        }
        public void SetPetSelect(int index)
        {
            system.GetHandler<PetSelectHandler>().SetSlotGroup(index);
        }
        // set the total slot count
        public void SetCharacterCount(int count)
        {
            PetSelectScreen screen = GetScreen("PetSelect").GetComponent<PetSelectScreen>();
            screen.characterCount = count;
        }
        // get the slot count 
        public void SetSlot(int slot)
        {
            PetSelectScreen screen = GetScreen("PetSelect").GetComponent<PetSelectScreen>();
            screen.CurrentSlot = slot; 
        }
        public void SetSlotGroup(Slot[] group)
        {
            PetSelectScreen screen = GetScreen("PetSelect").GetComponent<PetSelectScreen>();
            screen.Slots = group; 
        }
        // Getting data from server
        public CharacterData GetSave(int saveIndex)
        {
            CharacterData[] saves = (CharacterData[])system.Request("characterData");
            return saves[saveIndex];
        }

    }
}