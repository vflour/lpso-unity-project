using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
using Game.Networking;

namespace Game
{
    public class GameSystem : MonoBehaviour
    {    
#region Initialization
        // generic list of all the handlers
        protected List<IHandler> handlers = new List<IHandler>();

        // required handlers, in order of priority
        protected GameUI gameUI; // handles all ui 
        protected NetworkClient networkClient; // handles all the networking portion

        // Gamedata class
        protected GameData gameData;

        // initialize all handlers on awake
        protected virtual void Awake()
        {   
            DeclareRecievers();
            gameUI = GameObject.Find("UIHandler").GetComponent<GameUI>();
            networkClient = GameObject.Find("NetworkClient").GetComponent<NetworkClient>();

            handlers.Add(gameUI as IHandler);
            handlers.Add(networkClient as IHandler);
            gameData = GameObject.Find("GameData").GetComponent<GameData>();  
        }

        // start all the handlers
        protected virtual void Start()
        {
            foreach(IHandler handler in handlers)
            {
                handler.system = this;
                handler.Activate();
            }
        }
        
        // declare base recievers
        protected virtual void DeclareRecievers() 
        {
            Recieve("loadScene",(obj)=>
            {
                LoadScene((int)obj);
            });
            Recieve("startupScene",(obj)=>
            {
                LoadScene(0);
            });
        }
#endregion

#region Handler Adapter functions
        protected delegate void EmitDelegate<T>(T item);
        protected Dictionary<string,EmitDelegate<object>> stringEmits = new Dictionary<string,EmitDelegate<object>>();

        // Emit handles the sending > recieving portion of the system. Fires a delegate stored in the stringEmits dictionary
        public void Emit(string emitType, object item)
        {
            if (!stringEmits.ContainsKey(emitType))
            {
                Debug.Log(emitType+" emit does not exist");
                return;
            };
            EmitDelegate<object> recieveDel = stringEmits[emitType];
            recieveDel(item);
        }

        //recieve is initialized in gamesystem's subclasses
        protected void Recieve(string emitType, EmitDelegate<object> del)
        {
            stringEmits.Add(emitType,del);
        }

        // Stores all the objects that can be attained via request
        protected Dictionary<string,object> stringRequests = new Dictionary<string,object>();

        // basic request method for stuff like Rooms, ServerInformation, etc..
        public object Request(string emitType)
        {
            try
            {
                return stringRequests[emitType];
            }
            catch(KeyNotFoundException e)
            {
                throw new System.Exception(emitType+" does not exist.");
            }
            
        }
#endregion

#region System methods
        // honestly i just left these in here 

        // load a scene based on a scene index
        protected void LoadScene(int scene)
        {
            networkClient.Stall();
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
        }

        // protocol for whenever the network disconnects 
        public void ServerDisconnect()
        {
            gameUI.ToggleScreenInput(false); // disables input
            gameUI.NewErrorMessage(3); // sends errorcode to ui  
            gameUI.SetMessageEnableScreen(true); // sets emit code
        }
        // variant for setting an emit code when clicking on ok
        public void ServerDisconnect(string emitCode)
        {
            gameUI.ToggleScreenInput(false);
            gameUI.NewErrorMessage(3); // sends errorcode to ui 
            gameUI.SetMessageReturnEmit(emitCode); // sets emit code
        }
#endregion

#region Network signal checks
        protected bool processNetworkSignal(object obj) // checks if the object is a JSON and if theres no error codes
        {
            int returnCode = (int)((JSONObject)obj)["returnCode"].f;
            if(!CheckErrorCode(returnCode)) return false;

            return true; // returns true if everything is OK
        }
        protected bool CheckErrorCode(int code)
        {
            if(code == 0) return true; // lets the code pass, because code 0 = success
            else
            { // sends errorcode to ui 
                gameUI.NewErrorMessage(code); 
                gameUI.SetMessageEnableScreen(true); // enable screen input on click
                return false; 
            }
        }
 #endregion       
    }
}
