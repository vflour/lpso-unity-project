using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public Dictionary<string, Item> ItemBase;

    // 00 = Consumable
    // 11 = HeadItem
    // 20 = GlassesItem

    // etc for now
    public void InitDatabase()
    {
     ItemBase = new Dictionary<string, Item>()
        {
            { "000001",Resources.Load<Item>("Items/Cupcake")},
            { "110001",Resources.Load<Item>("Items/RedBow")}
        };
    }



}
