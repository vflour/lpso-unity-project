using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class TileTypes {
	public string name;
    public bool IsWalkable = true;
}

[System.Serializable]
public class SpeciesTypes
{
    public string name;
    public string model;
   
}

[System.Serializable]
public class PadSlots
{
    public int slotn;
    public Vector3 petposition;
    public Vector3 charsize;
    public string charangle;

}

public class MapData
{
    public int NonCollidable;
    public int Collidable;
    public int SizeX;
    public int SizeY;
    public int[] layers;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}

