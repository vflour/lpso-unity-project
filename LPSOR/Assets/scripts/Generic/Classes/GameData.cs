using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameData : MonoBehaviour
    {   
        public PlayerData playerData;
        public CharacterData[] characterDataArray;
        public int currentCharacter; // stores the current character, but also create a pet ticket data
        public Room roomData;
        public ServerInformation serverInformation;
        public string world;
        
        
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        public void LoadGameData(PlayerData player, CharacterData[] characterArray )
        {
            playerData = player;
            characterDataArray = characterArray;
        }

    }
}