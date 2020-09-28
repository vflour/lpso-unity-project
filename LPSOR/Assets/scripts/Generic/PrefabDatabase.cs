using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabArray", menuName = "Databases/PrefabArray", order = 1)]
public class PrefabDatabase : ScriptableObject
{
    public GameObject[] DB;
}
