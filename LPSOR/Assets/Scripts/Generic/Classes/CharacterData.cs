using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Inventory;

[System.Serializable]
public class CharacterData 
{
    public CharacterData()
    {
        this.palette = new int[] { 0, 0, 0 };
        this.species = 0;
        this.speciesSubtype = 0;
        this.parts = new int[] { 0, 0, 0, 0, 0, 0 };
    }
    // userdata + character info
    public string _id;
    public string userName;
    public string name;
    public int ticket;

    // Classes related to the character's sprite
    public int species; // Species of the pet
    public int speciesSubtype; // Species subtype ( ea: dog 1, dog 2, etc.)
    public int[] palette; // Palette that corresponds to 3 palettevalue indexes

    // Each Part type
    // 0 = Head
    // 1 = Eyes
    // 2 = Mouth
    // 3 = Ears
    // 4 = Tail
    // 5 = Hair
    public int[] parts;

    // Classes that just basically dictate misc. affairs
    public int gender; // 0 = F, 1 = M
    public ItemData[] wearing = new ItemData[8];
    public string adoptionDate;
    public int[] favFood;
    public int[] needs;
    public Vector2Int lastLocation;
    
    public int tile;
}


[System.Serializable]
public class PlayerData
{
    public string userName;
    public List<string> friends;
    public List<ItemData> inventory;
    public bool isMember;
    
    public int kibble;
    public int bronze;

    public int silver;
}

