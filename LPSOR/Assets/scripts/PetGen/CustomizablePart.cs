using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomizationSprite", menuName = "PetSprites/CustomizationSprite", order = 1)]

public class CustomizablePart : ScriptableObject
{
    public CustomSprite[] SpritesByOrder; // Sorts them in one big array.
}

[System.Serializable]
public struct CustomSprite
{
    public Sprite Sprite; // Sprite texture
    public string Angle; // Angle of which it is found
    public string PartName; // Specifies in which bone it's stored
    public int SortOrder; // Sprite sort order
}
