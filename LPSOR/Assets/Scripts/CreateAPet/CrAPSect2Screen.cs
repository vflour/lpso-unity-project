using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.CrAP;
using Game.UI.Startup;
using UnityEngine.UI;

namespace Game.UI.CRaP
{
    public class CrAPSect2Screen : GameScreen
    {
 #region Generic Section Handling
         private void Start()
         {
             CurrentSubSpecies = 0;
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

        public void PreviousSection()
        {
            CrAPUI uiHandler = gameUI as CrAPUI;
            uiHandler.PreviousSection(); 
        }
 #endregion
 #region Section 2 Handling (Changing the species)

        public Text speciesField;
        
        // get the subspecies count from the database
        private int SubSpeciesCount
        {
            get { return PetDB.GetSpeciesArray(CurrentSpecies).Length; }
        }
        // Selecting the subspecies species
        private int _currentSubSpecies = 0;
        public int CurrentSubSpecies
        {
            get { return _currentSubSpecies; }
            set
            {
                _currentSubSpecies = (int) Mathf.Repeat(value, SubSpeciesCount);
                speciesField.text = ((PetDatabase.SpeciesNames)CurrentSpecies).ToString();

                gameUI.EmitToSystem("setCurrentSubSpecies",_currentSubSpecies);
            }
        }
        // get currently stored species from system
        public int CurrentSpecies
        {
            get { return gameUI.RequestFromSystem<int>("currentSpecies"); }
        }
        
        // Button functions
        public void NextSubSpecies()
        {
            CurrentSubSpecies++;
        }
        public void PreviousSubSpecies()
        {
            CurrentSubSpecies--;
        }
#endregion
    }
}