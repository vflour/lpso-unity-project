using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI.PetSelect;
using Game.UI;

namespace Game.PetSelect
{
    public class PSSystem : GameSystem
    {
        
        private PetSelectHandler petSelect;
        private CharacterHandler characterHandler;
        
        protected override void Awake()
        {
            base.Awake();
            petSelect = GetHandler<PetSelectHandler>();
            characterHandler = GetHandler<CharacterHandler>();
            stringRequests.Add("playerData",gameData.playerData);
        }

        protected override void DeclareRecievers()
        {
            base.DeclareRecievers();

            Recieve("playAsCharacter",(obj)=>   PlayAsCharacter((int)obj));
            Recieve("createCharacter",(obj)=>   CreateCharacter((int)obj));
            
        }

        public void PlayAsCharacter(int saveIndex)
        {
            
            gameData.sessionData = petSelect.characterData[saveIndex];
            networkClient.SendData("setCharacter", JsonUtility.ToJson(gameData.sessionData));
            LoadScene(3); // load map
        }

        public void CreateCharacter(int ticketType)
        {
            gameData.currentTicket = ticketType;
            LoadScene(2); // load CrAP
        }

    }
}
