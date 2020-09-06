using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameData : MonoBehaviour
    {   
        public PlayerData playerData;
        public CharacterData[] characterDataArray;
        public Room roomData;
        public ServerInformation serverInformation;
        public string world;

        public GameSystem gameSystem;

        private void Awake()
        {
            gameSystem = GameObject.Find("System").GetComponent<GameSystem>();
            gameSystem.gameData = this;
            DontDestroyOnLoad(this.gameObject);
        }
        public void LoadGameData(PlayerData player, CharacterData[] characterArray )
        {
            playerData = player;
            characterDataArray = characterArray;
        }

    }
}