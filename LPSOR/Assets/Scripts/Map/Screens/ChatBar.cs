using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Map
{
    public class ChatBar : MonoBehaviour
    {
        [Header("Chat Bar")]
        public TMP_InputField inputBox;
        public TMP_Text resultBox;
        public Button button;
        public HUDScreen HudScreen;
        private bool filtered;
        public string filteredText = "";

        [Header("Speech Path")] 
        public SpeechPathMenu speechPathMenu;
        public Button speechPathButton;
        
        public bool Filtered
        {
            get => button.enabled;
            set => button.enabled = value;
        }
        
        public void SendButton()
        {
            // send message if queue is not full
            if (HudScreen.gameUI.system.GetHandler<ChatHandler>().CheckLocalQueue() && filteredText != "")
            {
                SendChat(inputBox.text);
                filteredText = "";
                inputBox.SetTextWithoutNotify(""); // reset text
            }
        }

        public void SendChat(string text)
        {
            HudScreen.gameUI.system.GetHandler<UserHandler>().SendChat(text);
        }

        private void Start()
        {
            inputBox.characterLimit = HudScreen.gameUI.system.GetHandler<ChatHandler>().filter.maxLength;
            speechPathMenu.speechPathFound = speech => SendChat(speech);
            speechPathMenu.tailRelativeTo = speechPathButton.transform;
        }
        private void Update()
        {
            resultBox.text = filteredText;
            if (Input.GetKeyUp(KeyCode.Return) && inputBox.isFocused && Filtered)
                SendButton();
        }

        public void TextChanged()
        {
            string message = inputBox.text;
            HudScreen.gameUI.system.GetHandler<ChatHandler>().FilterString(message, FilterText);
        }
        private void FilterText(FilteredMessage message)
        {
            string filteredText = " "+message.message+" ";
            foreach (string word in message.invalidWords)
            {
                filteredText = filteredText.Replace(" "+word+" ", $" <color=#ff0000>{word}</color> ");
            }
                
            this.filteredText = filteredText.Substring(1); // remove the leading space
            Filtered = message.invalidWords.Length==0; // enable the button if nothing is filtered
        }
    }
}
