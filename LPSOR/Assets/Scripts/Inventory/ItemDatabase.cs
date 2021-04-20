using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Databases/Item", order = 1)]
    public class ItemDatabase : ScriptableObject
    {
        public Item[] data;
    }
}