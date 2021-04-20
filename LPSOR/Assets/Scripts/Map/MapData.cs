using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MapData : ScriptableObject
    {
        public bool[] collisionMap;
        public Vector2Int size;
        public Vector2 tileSize;
        public Vector2Int[] spawnPoints;
        public float yOffset;
        public string mapName;
    }
}