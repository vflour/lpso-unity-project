
using Game;
using Game.Inventory;
using UnityEditor;
[CustomPropertyDrawer(typeof(PrefabDictionary))]
[CustomPropertyDrawer(typeof(IconDictionary))]
[CustomPropertyDrawer(typeof(GameEventDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}
