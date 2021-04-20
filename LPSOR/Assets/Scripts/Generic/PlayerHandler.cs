using System;
using System.Collections;
using System.Collections.Generic;
using Game.Networking;
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
        public void OnPlayerAdded()
        {
            system.ServerDataEvent("spawnPlayer", PlayerAdded);
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
        public void OnPlayerRemoving()
        {
            system.ServerDataEvent("removePlayer", PlayerRemoving);
        }
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
        public void OnPlayerMoveTile()
        {
            system.ServerDataEvent("moveTile", PlayerMoveTile);
        }
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
        public void OnPlayerChatted()
        {
            system.ServerDataEvent("chat",PlayerChatted);
        }

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

        public void OnPlayerPropInteracted()
        {
            system.ServerDataEvent("propInteract", PlayerPropInteracted);
        }

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
        public void OnPlayerDressed()
        {
            system.ServerDataEvent("dress", PlayerDressed);
            system.ServerDataEvent("undress", PlayerUndressed);
        }

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

