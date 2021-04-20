using System;
using System.Collections;
using UnityEngine;

namespace Game.Map
{
    using Inventory;
    public class Player: MonoBehaviour
    {
        // Default fields
        public PlayerData data;
        public Character session;
        public string room;
        public string map;
        
        // Reference fields
        public PlayerHandler playerHandler;

        // Recommended to remove the player through PlayerHandler.RemovePlayer()

        // Executes whenever the player leaves the room. 
        // Either via disconnection, exiting the room or changing maps
        public void Remove()
        {
            // Wait for the player's action to finish
            // Destroy self and character
            RemoveCharacter();
            Destroy(gameObject);
        }
        #region Character generation
        public void SpawnCharacter(CharacterData characterData)
        {
            playerHandler.RequestCharacterGeneration(ref session, characterData);
            session.transform.SetParent(transform);
          
        }
        // removes the current character
        public void RemoveCharacter()
        {
            if (session != null)
            {
                session.Remove();
                session = null;
            }
        }
        public void ReloadCharacter(CharacterData characterData)
        {
            playerHandler.RequestCharacterRemoval(session);
            SpawnCharacter(characterData);
        }
        #endregion
        #region Character Controller
        // Character controller events fire whenever a user from this or another server controls their character.
        
        // This executes when the player uses the chat.
        public void ChatCharacter(string message)
        {
            ChatHandler chatHandler = playerHandler.system.GetHandler<ChatHandler>();
            chatHandler.Chatted(session,message);
        }
        
        // This executes whenever the player decides to move their character by picking a tile.
        public void MoveCharacter(int tileX, int tileY)
        {
            MapHandler mapHandler = playerHandler.system.GetHandler<MapHandler>();
            mapHandler.MoveToTile(session,new Vector2Int(tileX,tileY),()=>{});
        }
        public void MoveCharacter(int tileX, int tileY, Action callback)
        {
            MapHandler mapHandler = playerHandler.system.GetHandler<MapHandler>();
            mapHandler.MoveToTile(session,new Vector2Int(tileX,tileY),callback);
        }
        // This executes whenever the user interacts with an object in the map
        public void ActionCharacter()
        {
            
        }
        
        //When the player adds/removes clothes their character
        public void DressCharacter(int id)
        {
            session.AddClothes(id);
        }

        public void UndressCharacter(int id)
        {
            session.RemoveClothes(id);
        }
        
        // This executes when the player does an action that boosts the player's mood
        public void CareCharacter()
        {
            
        }
        #endregion
    } 
}
