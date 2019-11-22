using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public Dictionary<string, Item> ItemBase = new Dictionary<string, Item>();
    void Start()
    {
        
    }

    // 00 = Consumable
    // 11 = HeadItem
    // etc for now
    public void InitDatabase()
    {
        ItemBase = new Dictionary<string, Item>()
        {
            { "100001",Resources.Load<Item>("Items/Cupcake")},
            { "110001",Resources.Load<Item>("Items/RedBow")},

        };
    }



}
