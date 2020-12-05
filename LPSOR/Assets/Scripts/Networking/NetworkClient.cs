using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

using Newtonsoft.Json.Linq;
using SocketIOClient;

namespace Game.Networking
{
    public class NetworkClient : MonoBehaviour, IHandler
    {
            private SocketIO socket; // The websocket
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
            private bool _display;
            public bool Display
            {
                get { return _display;}
                set { _display = value; }
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
                //StartCoroutine(SearchForIP());
                 // if there is no established connection, then connect to the ip
                if(!ConnectionRecieved) StartCoroutine(SearchForIP());
                // connect as a user if there is a connection to the server (re-authenticate)
                else ConnectAsUser();
            }

            // check if a server is there, and connect to it
            private IEnumerator SearchForIP() 
            {
                // sends an HTTP GET request to the server
                // note to self: this isn't really secure. MTM attacks could happen
                string uri = CurrentServer.IP;
                UnityWebRequest www = UnityWebRequest.Get(uri);
                yield return www.SendWebRequest();
            
                if(www.isNetworkError || www.isHttpError) system.ServerDisconnect(); // tells system theres no connection
                else // connect via websocket if theres a response
                {
                    /*IO.Options options = new IO.Options();
                    options.ReconnectionDelay = 500;
                    options.Reconnection = true;
                    options.Timeout = 50000;*/
                    SocketIOOptions options = new SocketIOOptions();
                    options.Reconnection = true;
                    options.ReconnectionDelay = 500;
                    
                    socket = new SocketIO(uri,options);
                    socket.ConnectAsync();
                    SocketEvents();
                } 
            }

            // request authentication from server
            public void ConnectAsUser()
            {
                string authenKey = "{"+$"\"userName\":\"{CurrentServer.userName}\",\"keyId\":\"{CurrentServer.keyId}\""+"}";
                
                socket.EmitAsync("userConnect", authenKey); // sends authenKey (keyid and username) for authentication
                
            }
            
#endregion  

#region Socket events  
        
        private void SocketEvents()
        {
            socket.OnConnected += (sender, e) => OpenConnection();// fires when the socket is opened
            socket.OnDisconnected += (sender, e) => ResetConnection();// fires when the player is disconnected from the server
            
            // authen
            socket.On("authenSuccess", data =>  authenticateUser(data.GetValue<JToken>()) ); // fires when player is authenticated*
            
            // character requests
            socket.On("allCharacterData",data=> recieveCharacterData(data.GetValue<JToken>()) );
        }
        private void OpenConnection()
        {
            ConnectionRecieved = true;
            // tells system to proceed
            system.Emit("openConnection",null);
        }

        // resets the connection and boots to login screen
        public void ResetConnection()
        {
            ConnectionRecieved = false;
            system.ServerDisconnect("startupScene");
        }    

        private void authenticateUser(JToken data)
        {
            if (CurrentServer.keyId == "") // the server hasn't saved a keyid, thus the player is being registered
            {
                _currentServer.keyId = (string)data["keyId"];
                system.Emit("registerSuccess",data);
            }
            // there is a keyId, thus the player is being authenticated
            else system.Emit("authenSuccess",data);          
        }

        private void recieveCharacterData(JToken data)
        {
            system.Emit("recieveCharacters", data);
        }
#endregion  
#region Character Data requests
        public void RequestCharacterData()
        {
            socket.EmitAsync("requestCharacterData");
        }

        public void SetCharacterData(CharacterData characterData)
        {
            socket.EmitAsync("newCharacter", JsonConvert.SerializeObject(characterData));
        }
#endregion
    }
}

