using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CrAP
{
    public class CrAPSystem : GameSystem
    {
        private CrAPHandler crapHandler;
        private CharacterHandler characterHandler;
        
        protected override void Awake()
        {
            base.Awake();
            AddExtraHandlers();
            
            stringRequests.Add("playerData",gameData.playerData);
            stringRequests.Add("currentSpecies",0);
            stringRequests.Add("currentSubSpecies",0);
            stringRequests.Add("currentTicket",gameData.currentCharacter);
        }

        private void AddExtraHandlers()
        {
            // adds pet select and character handler to the list of handlers
            characterHandler = GameObject.Find("CharacterHandler").GetComponent<CharacterHandler>();
            crapHandler = GameObject.Find("CrAPHandler").GetComponent<CrAPHandler>();
            
            handlers.Add(characterHandler as IHandler);    
            handlers.Add(crapHandler as IHandler);
        }
#region Initialization

        // adds a couple of new recievers specific to this system
        protected override void DeclareRecievers()
        {
            base.DeclareRecievers();
            
            Recieve("setPart",(obj)=>crapHandler.SetPart((obj as int[])[0],(obj as int[])[1]));
            Recieve("setPalette",(obj)=>crapHandler.SetPalette((obj as int[])[0],(obj as int[])[1]));
            Recieve("setGender",(obj)=>crapHandler.SetGender((int)obj));
            Recieve("setName",(obj)=>crapHandler.SetName((string)obj));
            // changing the section
            Recieve("setSection", (obj) =>
            {
                int section = (int)obj;
                crapHandler.crapSection = (CrAPSection)section;
            });

            Recieve("setPetDatabase",(obj)=>stringRequests.Add("petDB",obj));
            
            // UI requests to modify the character data
            Recieve("savePetData",(obj)=>crapHandler.SaveCharacterData());
            
            // setting the pet data
            Recieve("setPetData",(obj)=>networkClient.SetCharacterData((CharacterData)obj));
            
            // UI request to set the current species
            Recieve("setCurrentSpecies",(obj)=>
            {
                crapHandler.SetSpecies((int) obj);
                stringRequests["currentSpecies"] = (int) obj;

            });
            // UI request to set the current subspecies
            Recieve("setCurrentSubSpecies",(obj)=>
            {
                crapHandler.SetSubSpecies((int) obj);
                stringRequests["currentSubSpecies"] = (int) obj;
            });

        }
#endregion       
    }
}