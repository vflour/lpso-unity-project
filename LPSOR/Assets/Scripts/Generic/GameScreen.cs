using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Startup;

namespace Game.UI
{
    public class GameScreen : MonoBehaviour
    {
        public GameUI gameUI;
        protected List<UnityEngine.UI.Button> buttons = new List<UnityEngine.UI.Button>();

        public virtual void ToggleInput(bool enabled)
        {
            foreach(UnityEngine.UI.Button button in buttons)
            {
                button.interactable = enabled;
            }
        }
        
    }
}