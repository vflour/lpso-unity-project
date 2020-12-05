using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Startup;

namespace Game.UI.Startup
{
    public class UIHandler : GameUI
    {

        // Adapter functions meant to handle the bridge between the networking and the UI input
        public void ConnectToServer(ServerInformation serverInformation)
        {
            // disabling all input to the screen
            ToggleScreenInput(false);

            // connect to server
            system.Emit("serverConnect",serverInformation);
        } 

        // tell system to connect as user
        public void ConnectAsUser()
        {
            system.Emit("userConnect",null);
        }

        // tells the system to switch to the pet select screen
        public void JoinRoom(Room room)
        {
            ToggleScreenInput(false); 

            // set the room values and load scene 1 (pet select)
            system.Emit("setRoom",room);
            system.Emit("loadScene",1);
        }

        /// System requests
        public List<ServerInformation> GetServerInformationSaves() // request server information
        {
            return system.Request("serverInformation") as List<ServerInformation>;
        }
        public List<Room> GetRooms() // request room information
        {
            return system.Request("roomInformation") as List<Room>;
        }

        public void RemoveServer(ServerInformation server)
        {
            system.Emit("removeServer", server);
        }

        // method used to display the userKey to the player
        public void ShowKey(string userKey){
            MessageBoxTypes messageBoxType = MessageBoxTypes.Default;
            NewMessageBox(messageBoxType, "Connected to server!","Here is your user key for this server! Please write this down somewhere safe. \n"+userKey);
        }

    
    }
}




