using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Startup;

namespace Game.UI.Startup
{
    public class UIHandler : GameUI
    {
        
        #region Private classes
        private SystemHandler systemHandler;

        #endregion
    

        // Starting up the UI
        public void StartupGame(SystemHandler systemHandler)
        {
            this.systemHandler = systemHandler;

            LoadingScreen loadScreen = InstantiateLoadingScreen();
            loadScreen.FinishLoading();
            
            InstantiateScreen(0);
        }


        // Adapter functions meant to handle the bridge between the networking and the UI input
        public void ConnectToServer(ServerInformation serverInformation)
        {
            // disabling all input to the screen
            ToggleScreenInput(false);
            // inform system to connect to an ip
            systemHandler.RequestConnectionToIP(serverInformation);
        } 

        public void ConnectAsUser()
        {
            systemHandler.RequestUserAuthen();
        }

        public void JoinRoom(Room room)
        {
            ToggleScreenInput(false);
            systemHandler.SetRoom(room);
            systemHandler.LoadScene(1);
        }

        // requesting serverinformation data that was saved
        public List<ServerInformation> GetServerInformationSaves()
        {
            return systemHandler.RequestServerInformation();
        }

        public List<Room> GetRooms()
        {
            return systemHandler.RequestRooms();
        }

        // displaying error codes/messages for the connection

        public void ShowKey(string userKey){
            MessageBoxTypes messageBoxType = MessageBoxTypes.Default;
            NewMessageBox(messageBoxType, "Connected to server!","Here is your user key for this server! Please write this down somewhere safe. \n"+userKey);
        }

    
    }
}




