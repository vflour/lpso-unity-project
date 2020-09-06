using System.Collections;
using System.Collections.Generic;
using Game.UI.Startup;
using UnityEngine;
using SocketIO;
using Game.Networking;

namespace Game.Startup
{
    public class SystemHandler : GameSystem // I recommend splitting the Networking component from this script.
    {
        private List<ServerInformation> savedServerInfo;
        private List<Room> serverRooms;
        
        public void Awake() // setting up basic variables upon startup
        {
            networkClient = GetComponent<NetworkClient>();
            uiHandler = GetComponent<GameUI>();
            UIHandler startupUI = (UIHandler)uiHandler;

            startupUI.StartupGame(this); 
        }

        public void RequestConnectionToIP(ServerInformation serverInformation)
        {
            networkClient.SetCurrentServer(serverInformation);
            networkClient.ConnectToIP();
        }

        public void RequestUserAuthen()
        {
            networkClient.ConnectAsUser();
        }

        public List<ServerInformation> RequestServerInformation()
        {
            savedServerInfo = ServerDataPersistence.LoadServerData();
            return savedServerInfo;
        }

        public List<Room> RequestRooms()
        {
            return serverRooms;
        }

        public void SetRoom(Room room)
        {
            gameData.roomData = room;
        }


        public void AddNewServer(ServerInformation serverInformation)
        {
            savedServerInfo.Add(serverInformation);
            ServerDataPersistence.SaveServerData(savedServerInfo);
        }

        public override void ProcessNetworkSignal(string signal, int code, JSONObject data){
            if (CheckErrorCode(code)) // checks if no errors in server
            {
                CheckSignalType(signal,data); // sends signal to uI
            }
        }

        public bool CheckErrorCode(int code)
        {
            if(code == 0) return true; // lets the code pass, because code 0 = success
            else
            { /// hghghdhghsgjks fuckign why
                UIHandler startupUI = (UIHandler)uiHandler;
                startupUI.ShowErrorCode(ErrorCodes.CodeStrings[code]); // sends errorcode to ui 
                return false; 
            }
        }

        public void CheckSignalType(string signal, JSONObject data)
        {
            UIHandler startupUI = (UIHandler)uiHandler;
            switch(signal)
            {
                case "open":
                    startupUI.ConnectAsUser();
                    break;
                case "authenSuccess":
                    SetServerConnection(data);
                    break;
                case "registerSuccess":
                    AddNewServer(networkClient.CurrentServer);
                    startupUI.ShowKey(data["keyId"].str);
                    SetServerConnection(data);
                    break;
            }
        }
        
        private void SetServerConnection(JSONObject data)
        {
            SetServerData(data);
            uiHandler.RemoveScreen(0);
            uiHandler.InstantiateScreen(3);

            gameData.serverInformation = networkClient.CurrentServer;
            gameData.playerData = new PlayerData(data["playerData"]);
        }
        private void SetServerData(JSONObject data)
        {
            List<JSONObject> array = data["rooms"].list;
            serverRooms = TransformJSONintoRoom(array);
        }
        private List<Room> TransformJSONintoRoom(List<JSONObject> JSONlist)
        {
            List<Room> rooms = new List<Room>();
            foreach(JSONObject jsonobject in JSONlist)
            {
                Room room = new Room(jsonobject["name"].str,(int)jsonobject["population"].f,(int)jsonobject["buddies"].f,(int)jsonobject["maxPopulation"].f);
                rooms.Add(room);
            }
            return rooms;
        }


    }

}



