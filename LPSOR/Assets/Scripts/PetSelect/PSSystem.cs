using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI.PetSelect;
using Newtonsoft.Json;
using Game.UI;
using Newtonsoft.Json.Linq;

namespace Game.PetSelect
{
    public class PSSystem : GameSystem
    {
        
        private PetSelectHandler petSelect;
        private CharacterHandler characterHandler;
        
        protected override void Awake()
        {
            base.Awake();

            // adds pet select and character handler to the list of handlers
            petSelect = GameObject.Find("PetSelectHandler").GetComponent<PetSelectHandler>();
            characterHandler = GameObject.Find("CharacterHandler").GetComponent<CharacterHandler>();

            handlers.Add(petSelect as IHandler);
            handlers.Add(characterHandler as IHandler);
            
            stringRequests.Add("playerData",gameData.playerData);
        }

        protected override void DeclareRecievers()
        {
            base.DeclareRecievers();
            Recieve("requestCharacters", (obj) => RequestCharacters());
            Recieve("recieveCharacters", (obj) =>
            {
                if (!processNetworkSignal(obj)) return;
                RecieveCharacters((JObject) obj);
            } );
            
            Recieve("playAsCharacter",(obj)=>   PlayAsCharacter((int)obj));
            Recieve("createCharacter",(obj)=>   CreateCharacter((int)obj));
            
            Recieve("incrementPS",(obj)=>       petSelect.IncrementSlotGroup((int)obj));
            Recieve("setPS",(obj)=>             petSelect.SetSlotGroup((int)obj));
            Recieve("setCurrentSlot",(obj)=>    SetSlot((int)obj));

            Recieve("setSlotGroup",(obj)=>      SetSlotGroup((Slot[])obj));
            
        }

        public void RequestCharacters()
        {
            this.networkClient.RequestCharacterData();
        }
        public void RecieveCharacters(JObject data)
        {
            CharacterData[] characterData = ((JArray) data["characterData"]).ToObject<CharacterData[]>();
            petSelect.SetCharacterData(characterData);
            
            PSUIHandler uiHandler = (PSUIHandler)gameUI;
            uiHandler.SetCharacterCount(characterData.Length);
        }
        
        public void PlayAsCharacter(int saveIndex)
        {
            gameData.currentCharacter = saveIndex;
            LoadScene(3); // load map
        }

        public void CreateCharacter(int ticketType)
        {
            gameData.currentCharacter = ticketType;
            LoadScene(2); // load CrAP
        }

        public void SetSlot(int slot)
        {
            PSUIHandler uiHandler = (PSUIHandler)gameUI;
            uiHandler.SetSlot(slot);
        }
        public void SetSlotGroup(Slot[] group)
        {
             PSUIHandler uiHandler = (PSUIHandler)gameUI;
             uiHandler.SetSlotGroup(group);
        }
        
    }
}
