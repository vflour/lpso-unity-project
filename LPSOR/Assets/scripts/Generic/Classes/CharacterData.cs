using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData 
{
    public CharacterData(int species)
    {
        this.palette = new int[] { 0, 0, 0 };
        this.species = species;
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
    public PlayerData(JSONObject data)
    {
        this.userName = data["userName"].str;
        this.kibble = (int)data["kibble"].n;
        this.friends = JSONtoIntArray(data["friends"].list);
        this.inventory = JSONtoIntArray(data["inventory"].list);;
           
    }

    private int[] JSONtoIntArray(List<JSONObject> list)
    {
        int[] array = new int[list.Count];
        for(int i = 0; i < list.Count; i++)
        {
            array[i] = (int)list[i].n;
        }
        return array;
    }

    [SerializeField]
    private string userName;

    [SerializeField]
    private int[] friends;

    [SerializeField]
    private int[] inventory;
    
    [SerializeField]
    private int kibble;

}

public class CharacterAction // Actions are basically animations/interacting with items
{
    public bool Active; // if Action is being performed
    public int ActionType; // Actiontype
    public int ObjectIndex; // If there's an object involved
}
