using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameData : MonoBehaviour
    {   
        // Character and player data
        public PlayerData playerData;
        public CharacterData sessionData;
        public int currentTicket;
        public int spawnLocation = -1;

        // Map + server information
        public Room roomData;
        public string map;
        public ServerInformation serverInformation;
        
        // In-game data
        public List<GameEvent> events = new List<GameEvent>();
        
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        public void LoadGameData(PlayerData player)
        {
            playerData = player;
        }
        
        // Disables self by destroying the game object
        public void Disable()
        {
            Destroy(this.gameObject);
        }

    }
}