using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.CrAP;
using Game.UI.Startup;
using UnityEngine.UI;

namespace Game.UI.CRaP
{
    public class CrAPSect1Screen : GameScreen
    {
#region Generic Section Handling

        private void Start()
        {
            CurrentSpecies = 0;
        }
        // Gets the pet database from System
        public PetDatabase PetDB
        {
            get { return gameUI.RequestFromSystem<PetDatabase>("petDB"); }
        }
        // Continue Button
        public void NextSection()
        {
            CrAPUI uiHandler = gameUI as CrAPUI;
            uiHandler.NextSection();
        }
#endregion
#region Section 1 Handling (Changing the species)

        public Text speciesField;
        public Text subSpeciesField;

        // get the total species count from the database
        private int TotalSpeciesCount
        {
            get { return PetDB.SpeciesCount; }
        }
        // get the subspecies count from the database
        private int SubSpeciesCount
        {
            get { return PetDB.GetSpeciesArray(CurrentSpecies).Length; }
        }
    
        // Selecting species
        private int _currentSpecies = 0;
        public int CurrentSpecies
        {
            get { return _currentSpecies; }
            set
            {
                _currentSpecies = (int) Mathf.Repeat(value, TotalSpeciesCount);
                speciesField.text = ((PetDatabase.SpeciesNames)_currentSpecies).ToString();
                subSpeciesField.text = $"Choice 1 of {SubSpeciesCount}";
                
                gameUI.EmitToSystem("setCurrentSpecies",_currentSpecies);
            }
        }
        // Button functions
        public void NextSpecies()
        {
            CurrentSpecies++;
        }
        public void PreviousSpecies()
        {
            CurrentSpecies--;
        }
#endregion
    }
}