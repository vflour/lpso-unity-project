using UnityEngine;

namespace Game.UI
{
    [System.Serializable]
    public class DialogData
    {
        public DialogType type;
        public Sprite icon;
        public string[] text;
        public Vector2 size;
        public Vector2 offset;
    }
}