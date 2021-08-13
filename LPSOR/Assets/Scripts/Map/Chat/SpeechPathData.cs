using System;
using UnityEngine;

namespace Game.Map
{
    [CreateAssetMenu(fileName = "SpeechPath", menuName = "Databases/SpeechPath", order = 2)]
    public class SpeechPathData : ScriptableObject
    {
        public SpeechPath contains;
    }
    [Serializable]
    public struct SpeechPath
    {
        public string data;
        public SpeechPath[] contains;
    }
}