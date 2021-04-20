using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconArray", menuName = "Databases/IconArray", order = 2)]
public class IconDatabase : ScriptableObject
{
    public IconDictionary Data;
    public string firstItem;
}

[System.Serializable]
public class IconDictionary : SerializableDictionary<string, Sprite> {}
