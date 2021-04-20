using System;
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
        public Transform uiSpace;
        [SerializeField]
        public Transform backgroundSpace;
        
        [Header("Message Boxes")]
        [SerializeField]
        protected PrefabDatabase messageBoxPrefabs;
        
        [Header("Announcement Boxes")]
        public PrefabDatabase announceBoxPrefabs;
        public IconDatabase announceBoxIcons;
        
        #endregion
        #region Handler fields
        private GameSystem _system; // basic reference to system
        public GameSystem system
        {
            get{return _system;}
            set{_system = value;}
        }
        private bool _display;
        public bool Display
        {
            get { return _display;}
            set { _display = value; }
        }
        #endregion
        #region Initialization
        // Activate instantiates the loading screen
        public virtual void Activate()
        {
            LayerBackground("Overlap");
            InstantiateBackground("Loading");
            InstantiateScreen("Loading"); // sets the loading screen
            SetScreenVisibility("Loading",true);
        } 
        // FinishLoading removes the loading screen and instantiates the first screen
        public void FinishLoading() // NOTE TO SELF: think of a better way to store screen ids. maybe a dictionary?
        {
            StartCoroutine(GetScreen("Loading").GetComponent<LoadingScreen>().FinishLoading(() => LoadingScreenFinished()));
        }
        protected virtual void LoadingScreenFinished()
        {
            InstantiateScreen(screenPrefabs.firstItem);
            if(backgroundPrefabs.firstItem!="%NA%") //Some scenes do not have backgrounds
                InstantiateBackground(backgroundPrefabs.firstItem);
            RemoveBackground("Loading");
            RemoveScreen("Loading");
            LayerBackground("Background");
            system.DisplayScene();
        }
        #endregion
        #region UI-To-System handling
        // Method that emits a string to system without an object
        public void EmitToSystem(string emit) 
        {
            EmitToSystem(emit,null);
        }
        //  Method that emits a string to system
        public void EmitToSystem(string emit, object obj) 
        {
            system.Emit(emit,obj);
        }
        //Method request any object from the system
        public object RequestFromSystem(string emit) 
        {
            return system.Request(emit);
        }
        //Generic method variant for system requests
        public T RequestFromSystem<T>(string emit) 
        {
            return (T)system.Request(emit);
        } 
        #endregion
#region Message box handling
        public void NewMessageBox(MessageBoxTypes messageBoxType, string title, string contents)
        {
            if(currentMsgBox) RemoveMessageBox(); // remove if there already is one

            GameObject objectBox = Instantiate(messageBoxPrefabs.Data[messageBoxType.ToString()],uiSpace);

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
            Debug.Log(message);
            MessageBoxTypes messageBoxType = MessageBoxTypes.Default;
            NewMessageBox(messageBoxType, "Oops!",message);
        }

        // Sets specific values for the current message box
        
        // Enables screen input when clicked
        public void SetMessageEnableScreen(bool value)
        {
            currentMsgBox.EnableScreenOnClick = value;
        }
        // Call delegate if there is one
        public void CallDelegateOnMessageClose(Action callbackAction)
        {
            currentMsgBox.callbackAction = callbackAction;
        }
#endregion

#region Announcement Box handling
        public AnnounceBox NewAnnounceBox(AnnounceBoxType type, AnnounceBoxIcon icon, string message)
        {
            AnnounceBox announceBox = InstantiateScreen("AnnounceBox",announceBoxPrefabs.Data[type.ToString()]).GetComponent<AnnounceBox>();
            announceBox.icon.sprite = announceBoxIcons.Data[icon.ToString()];
            announceBox.message.text = message;
            return announceBox;
        }


#endregion
#region Active UI element storage
    private Dictionary<string,GameObject> loadedScreens = new Dictionary<string,GameObject>(); // All instantiated screens + backgrounds that are active
    private Dictionary<string,GameObject> loadedBackgrounds = new Dictionary<string,GameObject>();
    private MessageBox currentMsgBox; // Active message box. There can only be one
#endregion
#region Screen and Background object handling

        // Instantiates a screen object into the game using an int index
        public GameObject InstantiateScreen(string screenId)
        {
            GameObject screen = GameObject.Instantiate(screenPrefabs.Data[screenId],uiSpace);
            screen.name = screenId;
            screen.GetComponent<GameScreen>().gameUI = this;
            loadedScreens.Add(screenId,screen);
            return screen;
        }
        public GameObject InstantiateScreen(string screenId, GameObject screenPrefab)
        {
            GameObject screen = GameObject.Instantiate(screenPrefab,uiSpace);
            screen.name = screenPrefab.name;
            screen.GetComponent<GameScreen>().gameUI = this;
            loadedScreens.Add(screenPrefab.name,screen);
            return screen;
        }

        // Instantiates a background object into the game using an int index
        public GameObject InstantiateBackground(string bgId) 
        {
            GameObject background = GameObject.Instantiate(backgroundPrefabs.Data[bgId],backgroundSpace);
            loadedBackgrounds.Add(bgId,background);
            //SetToLast(bgId);
            return background;
        }

        public void LayerBackground(string layerName)
        {
            backgroundSpace.GetComponent<Canvas>().sortingLayerName = layerName;
        }

        public GameObject GetScreen(string screenId)
        {
            return loadedScreens[screenId];
        }

        // removal methods
        
        // Completely removes the screen from the game

        public void RemoveScreen(string screenId)
        {
            Destroy(loadedScreens[screenId]);
            loadedScreens.Remove(screenId);
        }

        // Removes ALL screens
        public void RemoveAllScreens()
        {
            foreach(GameObject screen in loadedScreens.Values)
                Destroy(screen);
            
            loadedScreens = new Dictionary<string, GameObject>();
        }
        // Completely removes the background from the game
        public void RemoveBackground(string bgId) // removes using the list index
        {
            GameObject.Destroy(loadedBackgrounds[bgId]);
            loadedBackgrounds.Remove(bgId);
        }
        
        
        // aesthetic methods

        // brings the screen to the front
        public void BringToFront(string screenId)
        {
            loadedScreens[screenId].transform.SetAsLastSibling();
        }
        
        //pushes the screen to be the back
        public void SetToLast(string screenId)
        {
            loadedScreens[screenId].transform.SetAsFirstSibling();
        }
        
        
        // sets the visibility to true or false
        public void SetScreenVisibility(string screenId,bool value)
        {
            loadedScreens[screenId].SetActive(value);
        }


        // disables or enables all button inputs in the screen
        public void ToggleScreenInput(bool value) 
        {
            foreach(GameObject screen in loadedScreens.Values)
                screen.GetComponent<GameScreen>().ToggleInput(value);
            
        }

#endregion

    }
}