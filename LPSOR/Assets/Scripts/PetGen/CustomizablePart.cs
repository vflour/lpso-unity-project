using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomizationSprite", menuName = "PetSprites/CustomizationSprite", order = 1)]

public class CustomizablePart : ScriptableObject
{
    public CustomSprite[] SpritesByOrder; // Sorts them in one big array.
    public int Ticket;
}

[System.Serializable]
public struct CustomSprite
{
    public GameObject Sprite; // Sprite texture
    public string Angle; // Angle of which it is found
    public string PartName; // Specifies in which bone it's stored
}
