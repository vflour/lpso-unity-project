using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.CrAP;
using UnityEngine.UI;

namespace Game.UI.CRaP
{
    // Create a new UI Handler so that CrAP UI can coordinate between the three screens
    public class CrAPUI : GameUI
    {
        // NOTE: The firstScreen is the CrAPScreen
        private int currentSection = 0;
        private string currentScreenSection;
        
        // Modifies the loading screen finished method to instantiate the first section
        protected override void LoadingScreenFinished()
        {
            base.LoadingScreenFinished();
            currentScreenSection = "CrAPSect" + (currentSection+1);
            InstantiateScreen("CrAPSect" + (currentSection+1));
            GetScreen("CrAP").GetComponent<CrAPScreen>().CurrentSection = currentSection;
        }

        // Decrements/Increments the section count then updates the UI
        public void NextSection()
        {
            currentSection = Mathf.Clamp(currentSection+1, 0, 2);
            SetSection();
        }
        public void PreviousSection()
        {
            currentSection = Mathf.Clamp(currentSection-1, 0, 2);
            SetSection();
        }

        // Updates the screen section
        private void SetSection()
        {
            // Tells craphandler to change section
            system.GetHandler<CrAPHandler>().crapSection = (CrAPSection)currentSection;
   
            // Replaces the current screens section
            RemoveScreen(currentScreenSection);
            currentScreenSection = "CrAPSect" + (currentSection+1);
            InstantiateScreen(currentScreenSection);
            
            // sets the current section icon
            GetScreen("CrAP").GetComponent<CrAPScreen>().CurrentSection = currentSection;
        }
    }
}