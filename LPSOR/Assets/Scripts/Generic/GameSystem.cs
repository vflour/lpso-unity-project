using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
using Game.Networking;
using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Game
{
    public class GameSystem : MonoBehaviour
    {    
        #region Initialization
        // generic list of all the handlers
        protected List<IHandler> handlers = new List<IHandler>();
        public MonoBehaviour[] additionalHandlers;
        
        // required handlers, in order of priority
        protected GameUI gameUI; // handles all ui 
        protected NetworkClient networkClient; // handles all the networking portion

        // Gamedata class
        protected GameData _gameData;

        public GameData gameData
        {
            get { return _gameData; }
        }
        
        // initialize all handlers on awake
        protected virtual void Awake()
        {   
            #if UNITY_EDITOR
                CreateTemporaryHandlers();
            #endif
            DeclareRecievers();
            gameUI = GameObject.Find("UIHandler").GetComponent<GameUI>();
            networkClient = GameObject.Find("NetworkClient").GetComponent<NetworkClient>();
            _gameData = GameObject.Find("GameData").GetComponent<GameData>();
            MouseHandler mouseHandler = GameObject.Find("MouseHandler").GetComponent<MouseHandler>();
            
            handlers.Add(gameUI);
            handlers.Add(networkClient);
            handlers.Add(mouseHandler);
            
            foreach (MonoBehaviour handler in additionalHandlers)
                if (handler is IHandler)
                {
                    handlers.Add(handler as IHandler);
                }
                    
        }
        
        // Mainly intended for debugging. Adds the handlers, but do remember that they will not function normally.
        public void CreateTemporaryHandlers()
        {
            GameObject networkClient = GameObject.Find("NetworkClient");
            GameObject gameData = GameObject.Find("GameData");

            if (gameData == null)
            {
                gameData = new GameObject();
                gameData.AddComponent<GameData>();
                gameData.name = "GameData";
            }

            if (networkClient == null)
            {
                networkClient = new GameObject();
                networkClient.AddComponent<NetworkClient>();
                networkClient.name = "NetworkClient";
            }
        }
        public T GetHandler<T>() where T: class, IHandler
        {
            IHandler foundHandler = null;
            foreach(IHandler handler in handlers)
                if (handler is T)
                    foundHandler = handler;
            if(foundHandler==null)
                throw new NullReferenceException("Could not find handler of type "+typeof(T));
            return foundHandler as T;
        }

        // start all the handlers
        protected virtual void Start()
        {
            foreach(IHandler handler in handlers)
            {
                handler.system = this;
                handler.Activate();
            }
            GetHandler<GameUI>().FinishLoading();
        }
        
        // tell handlers to elements visually after loading
        public virtual void DisplayScene()
        {
            foreach(IHandler handler in handlers)
                handler.Display = true;
        }       
        
        // declare base recievers
        protected virtual void DeclareRecievers() 
        {
            Recieve("loadScene",(obj)=> LoadScene((int)obj));
            Recieve("startupScene",(obj)=> LoadScene(0));
            Recieve("errorCode", (obj) => ErrorCode((int)obj,true));
        }
#endregion

#region Handler Adapter functions
        protected delegate void EmitDelegate<T>(T item);
        private Dictionary<string,Action<object>> stringEmits = new Dictionary<string,Action<object>>();
        private Queue<object> stringObjects = new Queue<object>();
        private Queue<Action<object>> emitQueue = new Queue<Action<object>>();

        private bool queueAvailable = false;
        // Calls the Emits in the main thread
        public void Update()
        {
            if (!queueAvailable || emitQueue.Count == 0) return;
            queueAvailable = false;
            
            Action<object> enqueuedDel = emitQueue.Dequeue();
            object param = stringObjects.Dequeue();
            enqueuedDel(param);

            queueAvailable = true;
        }

        // Emit handles the sending > recieving portion of the system. Fires a delegate stored in the stringEmits dictionary
        public void Emit(string emitType, object item)
        {
            try
            {
                emitQueue.Enqueue(stringEmits[emitType]);
                stringObjects.Enqueue(item);
                queueAvailable = true;
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogError(emitType+" emit does not exist: "+e.Message);
            }

        }

        //recieve is initialized in gamesystem's subclasses
        protected void Recieve(string emitType, EmitDelegate<object> del)
        {
            Action<object> action = new Action<object>(del);
            stringEmits.Add(emitType,action);
        }

        // Stores all the objects that can be attained via request
        protected Dictionary<string,object> stringRequests = new Dictionary<string,object>();

        // basic request method for stuff like Rooms, ServerInformation, etc..
        
        public T Request<T>(string emitType)
        {
            try
            {
                return (T)stringRequests[emitType];
            }
            catch(KeyNotFoundException e)
            {
                Debug.LogError(emitType+" request does not exist: "+e.Message);
                return default(T);
            }
            
        }
        public object Request(string emitType)
        {
            try
            {
                return stringRequests[emitType];
            }
            catch(KeyNotFoundException e)
            {
                Debug.LogError(emitType+" request does not exist: "+e.Message);
                return null;
            }
        }
        public void Store(string emitType, object storage)
        {
            if (stringRequests.ContainsKey(emitType))
                stringRequests[emitType] = storage;
            else
                stringRequests.Add(emitType, storage);
        }
        
        // Methods for server data requests
        public void ServerDataSend(string request, string data)
        {
            networkClient.SendData(request, data);
        }
        public void ServerDataRequest(string request, NetworkClient.SocketRequestDelegate requestDelegate)
        {
            ServerDataRequest(request, null, requestDelegate);
        }
        public void ServerDataRequest(string request, string data, NetworkClient.SocketRequestDelegate requestDelegate)
        {
            networkClient.RequestData(request, data, requestDelegate);
        }

        public void ServerDataEvent(string eventName, NetworkClient.SocketRequestDelegate requestDelegate)
        {
            networkClient.AddEvent(eventName,requestDelegate);
        }
        
        #endregion

#region System methods
        // load a scene based on a scene index
        protected void LoadScene(int scene)
        {
            networkClient.Stall();
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
        }

        // protocol for whenever the network disconnects 
        public void ServerDisconnect()
        {
            ErrorCode(3);
        }
        public void ServerDisconnect(bool reset)
        {
            ErrorCode(3,reset);
        }
        private void ResetGame()
        {
            networkClient.Disable();
            _gameData.Disable();
            LoadScene(0);
        }
        public void ErrorCode(int errorCode)
        {
            gameUI.ToggleScreenInput(false);
            gameUI.NewErrorMessage(errorCode);
            gameUI.SetMessageEnableScreen(true); // enable screen input on click
        }
        public void ErrorCode(int errorCode, bool reset)
        {
            ErrorCode(errorCode);
            if (reset)
                gameUI.CallDelegateOnMessageClose( ()=> ResetGame()); // sets emit code
        }
#endregion

    }
}
