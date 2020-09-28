using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Startup;
using Game.UI;
using Game.Networking;

namespace Game
{
    public class GameUI : MonoBehaviour, IHandler
    {
#region Fields initialized in the editor
        [Header("Screen Prefabs")]
        [SerializeField]
        protected PrefabDatabase screenPrefabs;

        [Header("Background Prefabs")]
        [SerializeField]
        protected PrefabDatabase backgroundPrefabs;

        [Header("Transform Spaces")]
        [SerializeField]
        protected Transform uiSpace;
        [SerializeField]
        protected Transform backgroundSpace;
        
        [Header("Message Boxes")]
        [SerializeField]
        protected PrefabDatabase messageBoxPrefabs;
 #endregion
 
 #region Handler fields
        private GameSystem _system; // basic reference to system
        public GameSystem system
        {
            get{return _system;}
            set{_system = value;}
        }

#endregion

 #region Active UI element storage
        private List<GameObject> loadedScreens = new List<GameObject>(); // All instantiated screens + backgrounds that are active
        private List<GameObject> loadedBackgrounds = new List<GameObject>();
        private MessageBox currentMsgBox; // Active message box. There can only be one
 #endregion

 #region Initialization
        public virtual void Activate()
        {
            InstantiateBackground(0);
            GameObject loadScreen = GetScreen(InstantiateScreen(0)); // sets the loading screen
            loadScreen.GetComponent<LoadingScreen>().FinishLoading();
            FirstScreen();
        } 
        public virtual void FirstScreen()
        {
            InstantiateScreen(1);
            SetScreenVisibility(1,false);
            InstantiateBackground(1);
        } 
        public void EmitToSystem(string emit) // Generic class to emit a string to system
        {
            system.Emit(emit,null);
        }    

#endregion

#region Message box handling
        protected void NewMessageBox(MessageBoxTypes messageBoxType, string title, string contents)
        {
            if(currentMsgBox) RemoveMessageBox(); // remove if there already is one

            GameObject objectBox = Instantiate(messageBoxPrefabs.DB[(int)messageBoxType],uiSpace);

            // get the messagebox component and set default fields
            currentMsgBox = objectBox.GetComponent<MessageBox>();
            currentMsgBox.gameUI = this;
            currentMsgBox.SetMessage(title,contents);
            
        }

        // removes the current message box completely
        public void RemoveMessageBox()
        {
            Destroy(currentMsgBox.gameObject);
            currentMsgBox = null;
        }

        // Display error message
        public void NewErrorMessage(int code) 
        {
            string message = ErrorCodes.CodeStrings[code];

            MessageBoxTypes messageBoxType = MessageBoxTypes.Default;
            NewMessageBox(messageBoxType, "Oops!",message);
        }

        // Sets specific values for the current message box

        // Emit string for emitting to the system
        public void SetMessageReturnEmit(string returnEmit)
        {
            currentMsgBox.returnEmit = returnEmit;
        }
        // Enables screen input when clicked
        public void SetMessageEnableScreen(bool value)
        {
            currentMsgBox.EnableScreenOnClick = value;
        }
#endregion


#region Screen and Background object handling

        // Instantiates a screen object into the game using an int index
        public int InstantiateScreen(int screenIndex)
        {
            GameObject screen = GameObject.Instantiate(screenPrefabs.DB[screenIndex],uiSpace);
            screen.GetComponent<GameScreen>().gameUI = this;
            loadedScreens.Add(screen);
            SetToLast(screen);
            return loadedScreens.Count-1;
        }

        // Instantiates a background object into the game using an int index
        public int InstantiateBackground(int bgIndex) 
        {
            GameObject background = GameObject.Instantiate(backgroundPrefabs.DB[bgIndex],backgroundSpace);
            loadedBackgrounds.Add(background);
            SetToLast(background);
            return loadedBackgrounds.Count-1;
        }

        public GameObject GetScreen(int screenIndex)
        {
            return loadedScreens[screenIndex];
        }

        // removal methods
        
        // Completely removes the screen from the game
        public void RemoveScreen(GameObject screen)
        {
            loadedScreens.Remove(screen);
            Destroy(screen);
        }

        public void RemoveScreen(int screen)
        {
            Destroy(loadedScreens[screen]);
            loadedScreens.RemoveAt(screen);
        }

        // Removes ALL screens
        public void RemoveAllScreens()
        {
            foreach(GameObject screen in loadedScreens)
            {
                Destroy(screen);
            }
            loadedScreens = new List<GameObject>();
        }
        // Completely removes the background from the game
       public void RemoveBackground(GameObject screen) // removes using a gameobject
        {
            loadedBackgrounds.Remove(screen);
            GameObject.Destroy(screen);
        }

        public void RemoveBackground(int screen) // removes using the list index
        {
            GameObject.Destroy(loadedBackgrounds[screen]);
            loadedBackgrounds.RemoveAt(screen);
        }
        

        // aesthetic methods

        // brings the element to the front of the screen
        public void BringToFront(GameObject element)
        {
            element.transform.SetAsLastSibling();
        }
        
        //brings the element to the back of the screen
        public void SetToLast(GameObject element)
        {
            element.transform.SetAsFirstSibling();
        }


        // sets the visibility to true or false
        public void SetScreenVisibility(int index,bool value)
        {
            loadedScreens[index].SetActive(value);
        }


        // disables or enables all button inputs in the screen
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