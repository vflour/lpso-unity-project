using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

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
    public string charId;
    public string userName;
    public string name;

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
    public string adoptionDate;
    public int[] favFood;
    public int[] needs;

    public int tile;
}

[System.Serializable]
public class PlayerData
{
    public PlayerData(JToken data)
    {
        this.userName = (string)data["userName"];
        this.kibble = (int)data["kibble"];
        this.friends = JSONtoIntArray((JArray)data["friends"]);
        this.inventory = JSONtoIntArray((JArray)data["inventory"]);;   
        this.bronze = (int)data["bronze"];
        this.silver = (int)data["silver"];  
    }

    private int[] JSONtoIntArray(JArray list)
    {
        int[] array = new int[list.Count];
        for(int i = 0; i < list.Count; i++)
        {
            array[i] = (int)list[i];
        }
        return array;
    }

    public string userName;
    public int[] friends;
    public int[] inventory;
    
    public int kibble;
    public int bronze;

    public int silver;
}

public class CharacterAction // Actions are basically animations/interacting with items
{
    public bool Active; // if Action is being performed
    public int ActionType; // Actiontype
    public int ObjectIndex; // If there's an object involved
}
