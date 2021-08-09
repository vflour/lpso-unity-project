using System;
using Game.UI;
using UnityEngine;

namespace Game.Map
{
    public class HUDScreen : GameScreen
    {
        private void Start()
        {
            gameUI.system.GetHandler<MouseHandler>().SetDialogPrompts(transform);
        }

        #region Right Bar Menu
        [Header("Right Bar")] 
        public GameObject rightBar;
        
        public void RightBarButton()
        {
            rightBar.SetActive(!rightBar.activeSelf);
        }

        public void InventoryButton()
        {
            if (!gameUI.HasScreen("Inventory"))
            {
                gameUI.InstantiateScreen("Inventory");
            }
            else
            {
                gameUI.RemoveScreen("Inventory");
            }
        }

        public void PDAButton()
        {
            if (!gameUI.HasScreen("PDA"))
            {
                gameUI.InstantiateScreen("PDA");
            }
            else
            {
                gameUI.RemoveScreen("PDA");
            }
        }

        #endregion
    }
}