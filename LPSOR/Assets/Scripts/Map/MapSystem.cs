using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Game.Map
{
   public class MapSystem : GameSystem
   {
      #region Initialization

      protected override void Awake()
      {
         // regular awake stuff
         base.Awake();

         // Set the local character location before playerhandler activates
         SetLocalCharacterLocation();
      }

      // Rewrites the lastLocation data when exiting/entering a map in game
      public void SetLocalCharacterLocation()
      {
         if (gameData.spawnLocation != -1) // -1 means there's nowhere to spawn from
         {
            Vector2Int location = GetHandler<MapHandler>().mapData.spawnPoints[gameData.spawnLocation];
            gameData.sessionData.lastLocation = location;
         }
      }

      #endregion
      
   }
}
