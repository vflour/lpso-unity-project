using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Startup;
using Game.UI;

namespace Game
{
    public class GameUI : MonoBehaviour
    {
        [Header("Screen Prefabs")]
        [SerializeField]
        protected GameObject[] screenPrefabs;
        [SerializeField]
        protected GameObject loadScreenPrefab;

        [SerializeField]
        protected Sprite loadScreenBackground;

        [Header("UI Spaces")]
        [SerializeField]
        protected Transform uiSpace;

        [Header("Message Boxes")]
        [SerializeField]
        public MessageBoxPrefabs messageBoxPrefabs;
       
        // protected/private classes
        private List<GameObject> loadedScreens = new List<GameObject>();
        private MessageBox currentMsgBox; 

#region Message box handling
        protected void NewMessageBox(MessageBoxTypes messageBoxType, string title, string contents)
        {
            if(currentMsgBox) RemoveMessageBox();
            GameObject connectionBox = Instantiate(messageBoxPrefabs.messageBoxes[(int)messageBoxType],uiSpace);
            currentMsgBox = connectionBox.GetComponent<MessageBox>();
            currentMsgBox.uiHandler = this;
            currentMsgBox.SetMessage(title,contents);
            
        }

        public void RemoveMessageBox()
        {
            Destroy(currentMsgBox.gameObject);
            currentMsgBox = null;
        }
        public void ShowErrorCode(string errorCode)
        {
            MessageBoxTypes messageBoxType = MessageBoxTypes.Default;
            NewMessageBox(messageBoxType, "Oops!",errorCode);
            currentMsgBox.EnableScreenOnClick = true;
        }


#endregion

#region loading new Screen objects
        // Loading a screen based on the index of the screen
        public void InstantiateScreen(int screenIndex)
        {
            GameObject screen = GameObject.Instantiate(screenPrefabs[screenIndex],uiSpace);
            LoadScreen(screen);
        }

        // Generating the loading screen
        public LoadingScreen InstantiateLoadingScreen()
        {
            GameObject screen = GameObject.Instantiate(loadScreenPrefab,uiSpace);
            LoadScreen(screen);
            LoadingScreen loadingComponent = screen.GetComponent<LoadingScreen>();
            loadingComponent.backgroundTexture = loadScreenBackground;
            return loadingComponent;
        }

        public void BringToFront(GameObject screen)
        {
            screen.transform.SetAsLastSibling();
        }

        public void SetToLast(GameObject screen)
        {
            screen.transform.SetAsFirstSibling();
        }

        // initializes the screen
        private void LoadScreen(GameObject screen)
        {
            screen.GetComponent<GameScreen>().gameUI = this;
            loadedScreens.Add(screen);
            SetToLast(screen);
        }

        // Completely removes the screen from the game
        public void RemoveScreen(GameObject screen)
        {
            loadedScreens.Remove(screen);
            GameObject.Destroy(screen);
        }

        public void RemoveScreen(int screen)
        {
            GameObject.Destroy(loadedScreens[screen]);
            loadedScreens.RemoveAt(screen);
        }

        public void ToggleScreenInput(bool value)
        {
            foreach(GameObject screen in loadedScreens)
            {
                screen.GetComponent<GameScreen>().ToggleInput(value);
            }
        }
#endregion

    }
}