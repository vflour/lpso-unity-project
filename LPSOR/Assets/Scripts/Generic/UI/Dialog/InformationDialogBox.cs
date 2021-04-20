using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class InformationDialogBox : DialogBox
    {
        public Image imageIcon;
        public Text caption;
        
        public override void SetData(DialogData data)
        {
            base.SetData(data);
            imageIcon.sprite = data.icon;
            caption.text = data.text[0];
        }
    }
}