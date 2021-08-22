using UnityEngine;
using UnityEngine.UI;

namespace Game.Map
{
    public class FriendButton : MonoBehaviour
    {
        private RelationshipData _data;

        public RelationshipData data
        {
            get => _data;
            set
            {
                userName.text = value.userName;
                status.text = value.isOnline ? "Online" : "Offline";
                status.color = value.isOnline ? new Color(0,0.6f,0) : Color.red;
                icon.sprite = pdaScreen.friendButtonIcons.Data[value.type.ToString()];
                _data = value;
            }
        }

        public PDAScreen pdaScreen;

        public Text userName;
        public Text status;
        public Image icon;
        
        public void FriendButtonMenu()
        {
            pdaScreen.CreateFriendButtonMenu(data.userName);
        }
    }
}