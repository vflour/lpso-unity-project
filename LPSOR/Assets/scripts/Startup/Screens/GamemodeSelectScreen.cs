using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Startup
{
    public class GamemodeSelectScreen : GameScreen
    {
        public void MultiplayerSelect()
        {
            gameUI.InstantiateScreen(1);
            gameUI.RemoveScreen(this.gameObject);
        }

    }
}
