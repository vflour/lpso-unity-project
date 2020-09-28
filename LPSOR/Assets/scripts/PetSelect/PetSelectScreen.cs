using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.PetSelect;

namespace Game.UI.PetSelect
{
    public class PetSelectScreen : GameScreen
    {
        private int currentSlot = 0;
        private Slot[] slots;
        public Slot[] Slots
        {
            get{return slots;}
            set
            {
                slots = value;
                Initialize();
            }
        }

        public int CurrentSlot
        {
            get{return currentSlot;}
            set
            {
                currentSlot = value;
                Initialize();
            }
        }

        public void GreenButton()
        {
            PSUIHandler uiHandler = (PSUIHandler)gameUI;
            Slot slot = slots[currentSlot];
            switch(slot.type)
            {
                case SlotType.Pet:
                    uiHandler.PlayAsPet(slot.saveIndex);
                    break;
                default:
                    uiHandler.CreatePet((int)slot.type);
                    break;  
            }
        }

        public Text nameField;
        public Text typeField;
        public void Initialize()
        {
            PSUIHandler uiHandler = (PSUIHandler)gameUI;
            Slot slot = slots[currentSlot];
            string name;
            string type;

            switch(slot.type)
            {
                case SlotType.Pet:
                    name = uiHandler.GetSave(slot.saveIndex).name;
                    type = slot.platformType.ToString();
                    break;
                default:
                    name = slot.type.ToString();
                    type = "";
                    break;  
            }

            nameField.text = name;
            typeField.text = type;

        }

    }
}

