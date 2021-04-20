using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ArrowListPSButton : ArrowListButton
    {
        protected int _value;
        public virtual int Value
        {
            get => _value;
            set
            {
                _value = value;
                numberField.text = (_value + 1).ToString();
            }
        }
[Header("Page Number Display")]        
        public Text numberField;
    }
}