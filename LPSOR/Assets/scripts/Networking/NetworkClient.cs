using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.Networking;

namespace Game.Networking
{
    public class NetworkClient : SocketIOComponent
    {
            private ServerInformation currentServerInfo;
            public ServerInformation CurrentServer
            {
                get{return currentServerInfo;}
            }
            private GameSystem systemHandler;

            public bool ConnectionRecieved;
            
            public override void Awake(){
                base.Awake();
                systemHandler = GetComponent<GameSystem>();
            }
            public void SetCurrentServer(ServerInformation serverInfo){
                currentServerInfo = serverInfo;
            }

            public void ConnectToIP()
            {
                if(IsConnected && socket.IsConnected)
                {
                    ConnectAsUser();
                    return;
                }
                else StartCoroutine(CheckForIP());
                
            }

            public IEnumerator CheckForIP()
            {
                string uri = "http://"+currentServerInfo.IP+":"+currentServerInfo.PORT;
                UnityWebRequest www = UnityWebRequest.Get(uri);
                yield return www.SendWebRequest();
                if(www.isNetworkError || www.isHttpError)
                {
                    systemHandler.ServerDisconnect();
                }
                else
                {          
                    this.url = "ws://"+currentServerInfo.IP+":"+currentServerInfo.PORT+"/socket.io/?EIO=4&transport=websocket";// sets the url based on port and ip
                    Connect();
                    SocketEvents();
                } 
            }
            public void ConnectAsUser()
            {
                PlayerKey authenKey = new PlayerKey(currentServerInfo.userName,currentServerInfo.keyId);
                Emit("userConnect", new JSONObject(JsonUtility.ToJson(authenKey)));
                // system io user shit;
            }

            public void ResetConnection()
            {
                ConnectionRecieved = false;
            }

            private void SocketEvents(){
                
                On("disconnect",(E)=>
                {
                    ResetConnection();
                    systemHandler.ServerDisconnect();
                });
                // note that the socket.io fires 'open' twice. 
                On("open",(E)=>
                { 
                    if(ConnectionRecieved) return;
                    ConnectionRecieved = true;
                    Debug.Log("Connected to server.");
                    systemHandler.ProcessNetworkSignal("open",0,E.data);
                });

                On("authenSuccess",(E)=>
                {
                    if (currentServerInfo.keyId == "") // the server hasn't saved a keyid, thus plris being registered
                    {
                        currentServerInfo.keyId = E.data["keyId"].str;
                        systemHandler.ProcessNetworkSignal("registerSuccess",(int)E.data["returnCode"].f,E.data);
                    }
                    else // there is a keyId, thus the player is being registered to the server
                    {
                       systemHandler.ProcessNetworkSignal("authenSuccess",(int)E.data["returnCode"].f,E.data); 
                    } 
                    
                    
                });


            }

    }
}

