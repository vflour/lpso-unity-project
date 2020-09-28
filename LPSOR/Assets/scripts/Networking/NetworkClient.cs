using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.Networking;

namespace Game.Networking
{
    public class NetworkClient : SocketIOComponent, IHandler
    {
            private ServerInformation _currentServer; // Stored server data
            public ServerInformation CurrentServer
            {
                get{return _currentServer;}
            }

            private GameSystem _system; // basic reference to system
            public GameSystem system
            {
                get{return _system;}
                set{_system = value;}
            }

            private bool ConnectionRecieved; // put in place so open doesnt fire twice (unfixed bug in socket.io for unity xP)
            private bool stalling; // disables the 'on' events if true

 #region Initialization           
            public virtual void Activate() // system is loaded, stop stalling on activation
            {
                stalling = false;
            }  

            public virtual void Awake() // NetworkClient can't be destroyed from scene to scene
            {
                base.Awake();
                DontDestroyOnLoad(this.gameObject); 
            }

            // stalls the networkclient until system is established
            public void Stall() 
            {
                stalling = true;    
            }

            // sets the current server information
            public void SetCurrentServer(ServerInformation serverInfo){
                _currentServer = serverInfo;
            }
#endregion

#region Server connection  
            public void ConnectToIP()
            {
                 // if there is no established connection, then connect to the ip
                if(!IsConnected || socket.IsConnected) StartCoroutine(SearchForIP());
                // connect as a user if there is a connection to the server (re-authenticate)
                else ConnectAsUser();
            }

            // check if a server is there, and connect to it
            private IEnumerator SearchForIP() 
            {
                // sends an HTTP GET request to the server
                // note to self: this isn't really secure. MTM attacks could happen
                string uri = "https://"+CurrentServer.IP;
                UnityWebRequest www = UnityWebRequest.Get(uri);
                yield return www.SendWebRequest();
            
                if(www.isNetworkError || www.isHttpError) system.ServerDisconnect(); // tells system theres no connection
                else // connect via websocket if theres a response
                {          
                    this.url = "ws://"+CurrentServer.IP+"/socket.io/?EIO=4&transport=websocket";// sets the url based on port and ip
                    Connect();
                    SocketEvents();
                } 
            }

            // request authentication from server
            public void ConnectAsUser()
            {
                PlayerKey authenKey = new PlayerKey(CurrentServer.userName,CurrentServer.keyId); 
                Emit("userConnect", new JSONObject(JsonUtility.ToJson(authenKey))); // sends authenKey (keyid and username) for authentication
            }
            
#endregion  

#region Server connection  
        
        private void SocketEvents(){
                On("disconnect",(E)=> ResetConnection()); // fires when the player is disconnected from the server
                On("open",(E)=> OpenConnection(E.data)); // fires when the socket is opened
                On("authenSuccess",(E)=>authenticateUser(E.data)); // fires when player is authenticated
        }
        private void OpenConnection(JSONObject data)
        {
            if(ConnectionRecieved) return; // adds connection recieved so it doesnt open twice
            ConnectionRecieved = true;

            // tells system to proceed
            system.Emit("openConnection",data);
        }

        // resets the connection and boots to login screen
        public void ResetConnection()
        {
            ConnectionRecieved = false;
            system.ServerDisconnect("startupScene");
        }    

        private void authenticateUser(JSONObject data)
        {
            if (CurrentServer.keyId == "") // the server hasn't saved a keyid, thus the player is being registered
            {
                _currentServer.keyId = data["keyId"].str;
                system.Emit("registerSuccess",data);
            }
            // there is a keyId, thus the player is being authenticated
            else system.Emit("authenSuccess",data);          
        }
#endregion  
    }
}

