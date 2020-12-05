using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.CrAP;
using UnityEngine.UI;

namespace Game.UI.CRaP
{
    public class CrAPUI : GameUI
    {
        // NOTE: The firstScreen is the CrAPScreen
        private int currentSection = 0;
        private GameObject currentScreenSection;
        
        // Modifies init to instantiate the first section
        public override void Activate()
        {
            base.Activate();
            currentScreenSection = InstantiateScreen(currentSection + 2);    
            firstScreen.GetComponent<CrAPScreen>().CurrentSection = currentSection;
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
            // Tells system to change section
            system.Emit("setSection", currentSection);
            
            // Replaces the current screens section
            RemoveScreen(currentScreenSection);
            currentScreenSection = InstantiateScreen(currentSection + 2);
            
            // sets the current section icon
            firstScreen.GetComponent<CrAPScreen>().CurrentSection = currentSection;
        }
    }
}