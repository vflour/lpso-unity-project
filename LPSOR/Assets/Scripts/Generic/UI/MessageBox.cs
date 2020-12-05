using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class MessageBox: MonoBehaviour
    {

[Header("Text fields")]
        public UnityEngine.UI.Text titleString;
        public UnityEngine.UI.Text msgString;

[Header("Confirmation variables")]
        public bool EnableScreenOnClick;
        public string returnEmit = "";
        
[Header("Handler")]
        public GameUI gameUI;
        
        public void SetMessage(string title, string msg) //sets the main text field message
        {
            titleString.text = title;
            msgString.text = msg; 
        }
        
        public void ConfirmButton() // sends signal to the ui handler when pressed
        {
            if (EnableScreenOnClick) gameUI.ToggleScreenInput(true);
            if (returnEmit!="") gameUI.EmitToSystem(returnEmit);            
            gameUI.RemoveMessageBox();
        }

    }

}
