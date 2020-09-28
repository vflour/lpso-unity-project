using System.Collections;
using System.Collections.Generic;
using Game.UI.Startup;
using UnityEngine;
using SocketIO;
using Game.Networking;

namespace Game.Startup
{
    public class SystemHandler : GameSystem 
    {

#region Initialization
        // initialize the request dictionary
        protected override void Awake() 
        {
            stringRequests.Add("serverInformation",ServerDataPersistence.LoadServerData()); // Add the serverInformation request
            base.Awake();
        }

        // adds a couple of new recievers specific to this system
        protected override void DeclareRecievers()
        {
            base.DeclareRecievers();

            // connection recievers
            Recieve("serverConnect",(obj)=>RequestConnectionToIP((ServerInformation)obj));
            Recieve("userConnect",(obj)=>RequestUserAuthen());
            
            // setting room data
            Recieve("setRoom",(obj)=> SetRoom((Room)obj));

            // network signals
            // processNetworkSignal exits the code if obj isnt a JSONobject or if there's an error
            Recieve("openConnection",(obj)=>RequestUserAuthen());

            Recieve("authenSuccess",(obj)=>
            {
                if (!processNetworkSignal(obj)) return;
                authenticateUser((JSONObject)obj);
            });

            Recieve("registerSuccess",(obj)=>
            {
                if (!processNetworkSignal(obj)) return;
                registerUser((JSONObject)obj);
            });

        }
#endregion

#region GameData modification requests
        private void SetRoom(Room room) // sets the current room
        {
            gameData.roomData = room;
        }
        
#endregion

#region Connection requests
        // sets the server information and tells the networkClient to connect to it
        private void RequestConnectionToIP(ServerInformation serverInformation)
        {
            networkClient.SetCurrentServer(serverInformation);
            networkClient.ConnectToIP();
        }

        // asks networkClient to authenticate the user if theres a connection to the server
        private void RequestUserAuthen()
        {
            networkClient.ConnectAsUser();
        }
#endregion

#region Modifications to the Server and Room lists
        private void AddNewServer(ServerInformation serverInformation)
        {
            List<ServerInformation> serverList = Request("serverInformation") as List<ServerInformation>;
            serverList.Add(serverInformation);
            ServerDataPersistence.SaveServerData(serverList);
        }

        //gets all the rooms sent from the server and adds it to the request dictionary
        private void SetRoomList(JSONObject data)
        {
            List<JSONObject> array = data["rooms"].list;
            stringRequests.Add("roomInformation",TransformJSONintoRoom(array));
        }

        // cycles through each JSONObject in the list and turns it into a Room object
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
#endregion

#region Network signal adapters
        private void authenticateUser(JSONObject data) 
        {
            SetRoomList(data); // sets the room lists
     
            // modifies gamedata values
            gameData.serverInformation = networkClient.CurrentServer; 
            gameData.playerData = new PlayerData(data["playerData"]);
            
            //removes the current screen (multiplayerScreen) and instantiates the room select screen
            gameUI.RemoveScreen(0);
            gameUI.InstantiateScreen(3);
            

        }

        private void registerUser(JSONObject data)
        {
            // adds a new server to the list and saves it
            AddNewServer(networkClient.CurrentServer);

            // tells the startup UIHandler to show the keyID
            UIHandler startupUI = (UIHandler)gameUI; 
            startupUI.ShowKey(data["keyId"].str);

            authenticateUser(data);
        }
#endregion

    }

}



