using System;
using UnityEngine;

namespace Game.UI
{
    public class DialogPrompt : MonoBehaviour
    {
        public MouseHandler mouseHandler;
        public DialogData data;
        public void OnMouseEnter()
        {
            mouseHandler.CreateDialog(data);
        }

        public void OnMouseExit()
        {
            mouseHandler.RemoveDialog();
        }
    }
}