using System;
using Game.UI;
using UnityEngine;

namespace Game.Map
{
    public class HUDScreen : GameScreen
    {
        private void Start()
        {
            GameObject inventory = gameUI.InstantiateScreen("Inventory");
            gameUI.system.GetHandler<MouseHandler>().SetDialogPrompts(transform);
        }
    }
}