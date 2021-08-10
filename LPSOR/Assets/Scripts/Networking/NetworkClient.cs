using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using Dpoch.SocketIO;
using Newtonsoft.Json.Linq;

namespace Game.Networking
{
    public class NetworkClient : MonoBehaviour, IHandler
    {
        //private QSocket socket; // The websocket
        private SocketIO socket;
        
        private ServerInformation _currentServer; // Stored server data
        public ServerInformation CurrentServer
        {
            set { _currentServer = value;}
            get{return _currentServer;}
        }

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
        private bool stalling; // disables the 'on' events if true

        #region Initialization           
        public virtual void Activate() // system is loaded, stop stalling on activation
        {
            stalling = false;
        }  
        
        public virtual void Awake() // NetworkClient can't be destroyed from scene to scene
        {
            
            DontDestroyOnLoad(this.gameObject); 
        }
        private void OnDestroy () {
            socket.Close ();
        }

        // Disables self by destroying the game object
        public void Disable()
        {
            stalling = true;
            Destroy(this.gameObject);
        }

        // stalls the networkclient until system is established
        public void Stall() 
        {
            stalling = true;    
        }
        
        #endregion
        #region Server connection  
        public void ConnectToServer()
        {
            StartCoroutine(TryConnect());
        }

        // Sends an HTTP GET request to the server
        // If it's successful, connect the socket. If not, tell the system to disconnect
        private IEnumerator TryConnect()
        {
            if (socket == null)
            {
                // Get URI and yield return
                string uri = CurrentServer.IP;
                UnityWebRequest www = UnityWebRequest.Get(uri);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                    system.ServerDisconnect(); // tells system theres no connection
                else // connect via websocket if theres a response
                    TrySocketIo();
            }
            else
                TryAuthen();
        }
        
        // Tries to connect to the socket if there's a response from the server
        private void TrySocketIo()
        {
            // Connect the socket
            string uri = CurrentServer.IP;
            //IO.Options options = new IO.Options {Reconnection = true, Timeout = 5000, ReconnectionDelay = 5000};
            Regex regex = new Regex("^(http[s]*)");
            uri = regex.Replace(uri, "ws")+ "/socket.io/?EIO=4&transport=websocket";
            socket = new SocketIO(uri);
            socket.Connect();
            SocketEvents();
            
            //yield break;
            // Set up the events and wait for the connection to call back
            //yield return StartCoroutine(WaitForData("onConnect"));
            //socketRequestData.Remove("onConnect");
        }

        private void TryAuthen()
        {
            // Request for user authentication
            string authenKey = "{"+$"\"userName\":\"{CurrentServer.userName}\",\"keyId\":\"{CurrentServer.keyId}\""+"}";
            RequestData("userConnect", authenKey, data =>
            {
                bool isNewPlayer = CurrentServer.keyId == null; // if theres no key id, the player is joining the server
                if (isNewPlayer) 
                {
                    _currentServer.keyId = (string)data["keyId"];
                    system.Emit("registerSuccess",data);
                }
                else 
                    system.Emit("authenSuccess",data);          
            });  
        }
        #endregion
        #region Connecting to the server
        
        // Disconnect and Connect events for the socket
        private void SocketEvents()
        {
            socket.OnOpen += () => OpenConnection();// fires when the socket is opened
            socket.OnClose += () => ResetConnection();
            //socket.On(QSocket.EVENT_DISCONNECT, e => ResetConnection());// fires when the player is disconnected from the server
            //socket.On(QSocket.EVENT_CONNECT_ERROR, e=> ResetConnection()); // or when there's issues connecting in general
            // socket.On(QSocket.EVENT_CONNECT_TIMEOUT, e => ResetConnection())
        }
        private void TestCock()
        {
            Debug.Log("large cock");
        }
        // Event method for opening the connection
        private void OpenConnection()
        {
            TryAuthen();
        }

        // resets the connection and boots to login screen
        public void ResetConnection()
        {
            system.ServerDisconnect();
        }
        #endregion
        #region Emit and On event recievers
        // Receiving and Waiting for server requests
        private Dictionary<string, Action<SocketIOEvent>> socketRequestData = new Dictionary<string, Action<SocketIOEvent>>();
        private List<string> requestsToRemove = new List<string>();
        public delegate void SocketRequestDelegate(JToken data);

        // Emit data to the server
        public void SendData(string eventName, string data)
        {
            socket.Emit(eventName, data);
        }
        public void SendData(string eventName)
        {
            socket.Emit(eventName);
        }
        
        public void AddEvent(string eventName)
        {
            socket.On(eventName, (data)=>
            {
                Debug.Log(data);
                ReceiveData(eventName, data.Data);
            });
        }
        public void AddEvent(string eventName, SocketRequestDelegate requestDelegate)
        {
            Action<SocketIOEvent> receiveEvent =
                data => ReceiveData(eventName, data.Data, requestDelegate);
            
            socketRequestData.Add(eventName, receiveEvent);  
            socket.On(eventName, receiveEvent);
        }
        public void RemoveEvent(string eventName)
        {
            socket.Off(eventName,socketRequestData[eventName]); // ARE YOU SERIOUS
            socketRequestData.Remove(eventName);
        }

        // Emit and wait for data (callback emit) from the server
        public void RequestData(string eventName, SocketRequestDelegate setResult)
        {
            RequestData(eventName, "", setResult);
        }
        public void RequestData(string eventName, string sendData, SocketRequestDelegate setResult)
        {
            AddEvent(eventName, setResult);
            SendData(eventName, sendData);
            requestsToRemove.Add(eventName);
        }
        
        // Method that sets the socket request data once the .On event has been triggered
        private bool ReceiveData(string eventName, JArray arrayData)
        {
            JObject data = (JObject)arrayData[0];
            bool success = ProcessResponse(data);
            
            if (requestsToRemove.Contains(eventName))
            {
                requestsToRemove.Remove(eventName);
                RemoveEvent(eventName);
            }
            return success;
        }
        Queue<SocketRequestDelegate> requestQueue = new Queue<SocketRequestDelegate>();
        Queue<string> requestIdQueue = new Queue<string>();
        
        private bool ReceiveData(string eventName, JArray arrayData, SocketRequestDelegate requestDelegate)
        {
            JObject data = (JObject)arrayData[0];
            bool success = ReceiveData(eventName, arrayData);
            if (success)
                requestDelegate(data["data"]);
            
            return success;
        }
        #endregion

        #region Request response handler
        
        // Processes a response from the Jtoken value
        protected bool ProcessResponse(JToken data) // checks if the object is a JSON and if theres no error codes
        {
            int responseCode = (int)data["returnCode"];
            return !CheckErrorCode(responseCode);
        }
        
        // Checks if the responseCode in the token is an error
        protected bool CheckErrorCode(int code)
        {
            bool isError = code != 0;
            if (isError)
                system.Emit("errorCode",code);
            return isError;
        }
        #endregion
    }
}

