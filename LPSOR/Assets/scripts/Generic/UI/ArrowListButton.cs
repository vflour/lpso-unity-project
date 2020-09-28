using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class ArrowListButton : MonoBehaviour
    {
        public int value;
        public ArrowListButtonType type;
        public ArrowList arrowList;

        public void SetPage()
        {
            switch(type)
            {
                case ArrowListButtonType.Page: // if theres no adding, just sets the index
                    arrowList.SetPage(value);
                    break;
                case ArrowListButtonType.Add:
                    arrowList.ChangePage(value);
                    break;
                case ArrowListButtonType.Reset:
                // max or min pages
                    int page = value == 1? arrowList.pageCount-1:0;
                    arrowList.ChangePage(page);
                    break;
            }
        }
    }
    public enum ArrowListButtonType{Page,Add,Reset}
}
