using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class MessageBox: MonoBehaviour
    {
        public UnityEngine.UI.Text titleString;
        public UnityEngine.UI.Text msgString;

        public GameUI uiHandler;
        public bool EnableScreenOnClick;
        
        public void SetMessage(string title, string msg)
        {
            titleString.text = title;
            msgString.text = msg; // sets the current text + pending gui 
            

        }
        
        public void ConfirmButton()
        {
            uiHandler.RemoveMessageBox();
            if (EnableScreenOnClick) uiHandler.ToggleScreenInput(true);
        }


    }

}
