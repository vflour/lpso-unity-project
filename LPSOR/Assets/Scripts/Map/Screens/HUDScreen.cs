using System;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Map
{
    public class HUDScreen : GameScreen
    {
        private void Start()
        {
            gameUI.system.GetHandler<MouseHandler>().SetDialogPrompts(transform);
            CharacterProfile();
        }

        #region Right Bar Menu
        [Header("Right Bar")] 
        public GameObject rightBar;
        public RawImage profileImage;
        // Generate the character profile for the rightbar
        public void CharacterProfile()
        {
            CharacterHandler charHandler = gameUI.system.GetHandler<CharacterHandler>();
            CharacterData data = gameUI.system.gameData.sessionData;
            StartCoroutine(charHandler.GenerateProfile(data._id,data,true, (texture) => profileImage.texture = texture));
        }
        public void RightBarButton()
        {
            if (!gameUI.HasScreen("PlayerViewer"))
            {
                PlayerViewerScreen screen = gameUI.InstantiateScreen("PlayerViewer").GetComponent<PlayerViewerScreen>();
                gameUI.system.ServerDataRequest("getAllCharacters",(data =>
                {
                    PlayerData playerData = gameUI.system.gameData.playerData;
                    CharacterData[] characters = data.ToObject<CharacterData[]>();
                    screen.SetCharacter(playerData,characters,gameUI.system.gameData.sessionData._id);
                    screen.SetPalette(PlayerViewerScreen.PaletteType.User);
                }));
            }
            else
                gameUI.RemoveScreen("PlayerViewer");

           // rightBar.SetActive(!rightBar.activeSelf);
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