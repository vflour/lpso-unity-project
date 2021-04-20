using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpeciesSubtype
{
    public GameObject BodyPrefab; // Prefab for species subtype
    public string SpeciesName; // Name of the species (not subspecies)

    // the below represents each CustomizablePart (contains specific sprite data) for each part type

    public CustomizablePart[] Head;
    public CustomizablePart[] Eyes;
    public CustomizablePart[] Mouth;
    public CustomizablePart[] Ears;
    public CustomizablePart[] Tail;
    public CustomizablePart[] Hair;

    public CustomizablePart[] GetCustomizablePartArray( int Index)
    {
        // awful. absolutely disguisting. why cant i just serialize the list. perhaps its for the better
        // gets the index data for the part and returns the array
        switch(Index){
            case 0:
                return Head;
            case 1:
                return Eyes;
            case 2:
                return Mouth;
            case 3:
                return Ears;
            case 4:
                return Tail;
            default:
                return Hair;
        }
    }
}
