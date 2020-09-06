using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class ArrowListButton : MonoBehaviour
    {
        public int adder;
        public int index;
        public ArrowList arrowList;

        public void SetPage()
        {
            switch(adder)
            {
                case 0: // if theres no adding, just sets the index
                    arrowList.SetPage(index);
                    break;
                default:
                    arrowList.ChangePage(adder);
                    break;
            }
        }
    }
}
