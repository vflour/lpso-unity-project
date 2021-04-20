using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Startup
{
    public class GamemodeSelectScreen : GameScreen
    {
        public void MultiplayerSelect()
        {
            gameUI.InstantiateScreen("Multiplayer");
            gameUI.RemoveScreen("ModeSelect");
        }

    }
}
