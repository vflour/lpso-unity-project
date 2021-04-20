using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class ArrowListIncrButton : ArrowListButton
    {
        // active doesn't change the sprite
        public virtual bool Active
        {
            get { return _active; }
            set { _active = value; }
        }
        
        // Replaces SetPage method 
        public override void SetPage()
        {
            arrowList.AddToPage(Value);
        }
    }
}