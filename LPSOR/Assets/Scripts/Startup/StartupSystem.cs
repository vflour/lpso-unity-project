using System.Collections;
using System.Collections.Generic;
using Game.UI.Startup;
using UnityEngine;
using Game.Networking;
using Game.UI;
using Newtonsoft.Json.Linq;

namespace Game.Startup
{
    public class StartupSystem : GameSystem 
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

            Recieve("authenSuccess", (obj) => AuthenticateUser((JObject)obj));
            Recieve("registerSuccess", (obj) => RegisterUser((JObject)obj));
        }
#endregion

#region Modifications to the Server and Room lists
        // Adds or updates a new server entry
        public void AddServer(ServerInformation serverInformation)
        {
            List<ServerInformation> serverList = Request("serverInformation") as List<ServerInformation>;
            if (!serverList.Contains(serverInformation)) // Adds the new entry if there's none
                serverList.Add(serverInformation);
            ServerDataPersistence.SaveServerData(serverList);
        }

        // Removes a server entry
        public void RemoveServer(ServerInformation serverInformation)
        {
            List<ServerInformation> serverList = Request("serverInformation") as List<ServerInformation>;
            serverList.Remove(serverInformation);
            ServerDataPersistence.SaveServerData(serverList);
        }
        
#endregion
        
        #region Server Connection
        public void ConnectUser(ServerInformation serverInformation)
        {
            networkClient.CurrentServer = serverInformation;
            networkClient.ConnectToServer();
        }
        private void AuthenticateUser(JObject data) 
        {
            // modifies gamedata values
            gameData.serverInformation = networkClient.CurrentServer;
            if (gameData.serverInformation.friendRelationships == null)
                gameData.serverInformation.friendRelationships = new List<RelationshipData>();
            gameData.playerData = data["playerData"].ToObject<PlayerData>();
            
            //removes the current screen (multiplayerScreen) and instantiates the room select screen
            gameUI.RemoveScreen("Multiplayer");
            gameUI.InstantiateScreen("RoomSelect");
        }

        private void RegisterUser(JObject data)
        {
            // adds a new server to the list and saves it
            AddServer(networkClient.CurrentServer);

            // tells the UIHandler to show the keyID
            string keyId = (string)data["keyId"];
            GetHandler<GameUI>().NewMessageBox(MessageBoxTypes.Default,"Connected to server!","Here is your user key for this server! Please write this down somewhere safe. \n"+keyId);
            GetHandler<GameUI>().CallDelegateOnMessageClose(() => AuthenticateUser(data));
        }
        #endregion

    }

}



