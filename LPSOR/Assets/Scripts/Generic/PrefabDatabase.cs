using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabArray", menuName = "Databases/PrefabArray", order = 1)]
public class PrefabDatabase : ScriptableObject
{
    public PrefabDictionary Data;
    public string firstItem;
}







