using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public enum MessageBoxTypes {Default,Warning,Choice};
    [CreateAssetMenu(fileName = "MessageBoxes", menuName = "UIPrefabs/MessageBoxes", order = 1)]
    public class MessageBoxPrefabs : ScriptableObject
    {
        
        public GameObject[] messageBoxes;
    }
}
