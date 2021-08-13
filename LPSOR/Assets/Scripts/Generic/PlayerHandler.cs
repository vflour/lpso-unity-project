using System;
using System.Collections;
using System.Collections.Generic;
using Game.Networking;
using Game.UI;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Game.Map
{
    using Inventory;
    public class PlayerHandler : MonoBehaviour, IHandler
    {
        public Dictionary<string,Player> players = new Dictionary<string,Player>();
        #region Initialization
        // IHandler methods
        public GameSystem system {get; set;}
        
        public bool Display { get; set; }

        public void Activate()
        {
            
        }
        #endregion
        #region Player Events

        public void PlayerEvents()
        {
            system.ServerDataEvent("spawnPlayer", PlayerAdded);
            system.ServerDataEvent("removePlayer", PlayerRemoving);
            system.ServerDataEvent("moveTile", PlayerMoveTile);
            system.ServerDataEvent("chat",PlayerChatted);
            system.ServerDataEvent("propInteract", PlayerPropInteracted);
            system.ServerDataEvent("dress", PlayerDressed);
            system.ServerDataEvent("undress", PlayerUndressed);
            system.ServerDataEvent("receiveFrRequest",FriendRequested);
            system.ServerDataEvent("acceptFrRequest",FriendAdded);
            
        }

        public bool ValidPlayer(JToken data)
        {
            string userName = (string) data["userName"];
            if (userName == null) return false;
            return HasPlayer(userName) && (userName != GetLocalPlayer().data.userName); // it's valid if the user is not the local one, and if the player exists
        }
        public void PlayerAdded(JToken data)
        {
            PlayerData playerData = data["playerData"].ToObject<PlayerData>();
            if (playerData.userName == GetLocalPlayer().data.userName) // exit if the localplayer is spawning
                return;
            
            CharacterData characterData = data["characterData"].ToObject<CharacterData>();
            CharacterHandler characterHandler = system.GetHandler<CharacterHandler>();
            if (characterHandler.HasCharacter(characterData._id)) // return if the character already exists
                return;

            Debug.Log(playerData.userName+" joined");
            //Check if the user is respawning
            if (HasPlayer(playerData.userName))
            {
                Player player = GetPlayer(playerData.userName);
                player.ReloadCharacter(characterData);
            }
            else
                LoadPlayer(playerData,characterData);  
        }
        
        // Remove the player on disconnect
        public void PlayerRemoving(JToken data)
        {
            if (!ValidPlayer(data)) // exit if not valid username
                return;
            string userName = (string) data["userName"];
            Debug.Log(userName+" left");
            
            Player player = GetPlayer(userName);
            player.Remove();
        }
        
        // Move the player when they click on a tile
        public void PlayerMoveTile(JToken data)
        {
            if (!ValidPlayer(data)) // exit if not valid username
                return;
            string userName = (string) data["userName"];
            if (data["tile"] == null)
                return;
            Player player = GetPlayer(userName);
            player.MoveCharacter((int) data["tile"]["x"], (int) data["tile"]["y"]);
        }
        
        // When the player types out a chat message

        public void PlayerChatted(JToken data)
        {
            if (!ValidPlayer(data)) // exit if not valid username
                return;
            string userName = (string) data["userName"];
            Player player = GetPlayer(userName);
            Debug.Log(userName+": "+data["message"]);
            player.ChatCharacter((string)data["message"]);
        }
        
        // When the player interacts with a prop


        public void PlayerPropInteracted(JToken data)
        {
            if (!ValidPlayer(data)) // exit if not valid username
                return;
            string userName = (string) data["userName"];
            Player player = GetPlayer(userName);
            
            int propId = (int) data["prop"];
            MapHandler mapHandler = system.GetHandler<MapHandler>();
            mapHandler.InteractObject(player,propId);
        }
        
        // When the player dresses/undresses character
        public void PlayerDressed(JToken data)
        {
            if (!ValidPlayer(data)) // exit if not valid username
                return;
            string userName = (string) data["userName"];
            Player player = GetPlayer(userName);
            
            int itemId = (int) data["item"];
            player.DressCharacter(itemId);
        }

        public void PlayerUndressed(JToken data)
        {
            if (!ValidPlayer(data)) // exit if not valid username
                return;
            string userName = (string) data["userName"];
            Player player = GetPlayer(userName);
            
            int itemId = (int) data["item"];
            player.UndressCharacter(itemId);
            
        }
        public void FriendRequested(JToken data)
        {
            if (!ValidPlayer(data)) // exit if not valid username
                return;
            string userName = (string) data["userName"];
            FriendRequestBox requestBox = system.GetHandler<GameUI>().NewAnnounceBox(AnnounceBoxType.FriendRequest) as FriendRequestBox;
            requestBox.userName = userName;
        }

        public void FriendAdded(JToken data)
        {
            string userName = (string) data["userName"];
            NewFriendBox friendBox = system.GetHandler<GameUI>().NewAnnounceBox(AnnounceBoxType.NewFriend) as NewFriendBox;
            friendBox.userName = userName;
        }
        #endregion
        // Adds a new player to the list by loading in their data
        public Player LoadPlayer(PlayerData playerData, CharacterData characterData)
        {
            GameObject playerObject = new GameObject();
            playerObject.name = playerData.userName;
            playerObject.transform.SetParent(transform);
            Player player = playerObject.AddComponent<Player>();
            player.data = playerData;
            player.playerHandler = this;
            
            // Load the character
            player.SpawnCharacter(characterData);
            
            players.Add(playerData.userName,player);
            return player;
        }
        public Player GetLocalPlayer()
        {
            return system.GetHandler<UserHandler>().localPlayer;
        }
        
        // Gets a player by the username
        public Player GetPlayer(string userName)
        {
            if(!players.ContainsKey(userName))
                throw new KeyNotFoundException("Could not find user with player name");
            return players[userName];
        }

        public bool HasPlayer(string userName)
        {
            return players.ContainsKey(userName);
        }
        // Removes a player from the game
        public void RemovePlayer(Player player)
        {
            players.Remove(player.name);
            player.Remove();
        }

        // Requests the Character to be removed
        public void RequestCharacterRemoval(Character character)
        {
            CharacterHandler characterHandler = system.GetHandler<CharacterHandler>();
            characterHandler.RemoveCharacter(character.name);
        }
        
        // Requests character data, it, and sets the session
        public void RequestCharacterGeneration(ref Character generatedCharacter, CharacterData characterData)
        {
            // Generate character sprite
            CharacterHandler characterHandler = system.GetHandler<CharacterHandler>();
            generatedCharacter = characterHandler.AddCharacter(characterData._id, characterData);
            
            // Set saved location
            MapHandler mapHandler = system.GetHandler<MapHandler>();
            generatedCharacter.tilePosition = mapHandler.CheckCoordinates(characterData.lastLocation);
            generatedCharacter.targetTile = mapHandler.GetTile(generatedCharacter.tilePosition);
            generatedCharacter.transform.localPosition = mapHandler.GetTileCoordinates(generatedCharacter.tilePosition);
        }

    }
}

