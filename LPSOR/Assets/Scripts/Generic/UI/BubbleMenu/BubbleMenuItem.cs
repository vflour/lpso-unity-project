using System;
using UnityEngine.Events;

namespace Game.UI
{
    [Serializable]
    public class BubbleMenuItem
    {
        public string itemData;
        public UnityEvent itemEvent;
    }
}