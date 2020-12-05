using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ArrowListButton : MonoBehaviour
    {
        // Value assigned to the button
        protected int _value;
        public virtual int Value
        {
            get => _value;
            set => _value = value;
        }

        public ArrowListButtonType buttonType;
        
        //Setting the button to active/inactive
        protected bool _active;
        public virtual bool Active
        {
            get { return _active;}
            set
            {
                // Set active size + sprite accordingly
                _active = value;
                if (_active)
                {
                    GetComponent<Image>().sprite = activeSprite;
                    transform.localScale = activeSize;
                }
                else
                {
                    GetComponent<Image>().sprite = inactiveSprite;
                    transform.localScale = inactiveSize;   
                }
            }
        }
        
[Header("Sprites")] 
        public Sprite activeSprite;
        public Sprite inactiveSprite;

        public Vector3 activeSize;
        public Vector3 inactiveSize;
        
 [Header("References")]    
        // Button click
        public ArrowList arrowList;
        
        public virtual void SetPage()
        {
            arrowList.CurrentPage = Value;
        }
    }
}
