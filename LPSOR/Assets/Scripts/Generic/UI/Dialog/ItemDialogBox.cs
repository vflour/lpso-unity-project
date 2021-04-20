using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ItemDialogBox : DialogBox
    {
        public Image imageIcon;
        public Text title;
        public Text caption;
        
        public override void SetData(DialogData data)
        {
            base.SetData(data);
            imageIcon.sprite = data.icon;
            title.text = data.text[0];
            caption.text = data.text[1];
        }
    }
}