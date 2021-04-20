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
                userName.text = data.userName;
                status.text = data.isOnline ? "Online" : "Offline";
                status.color = data.isOnline ? Color.red : Color.green;
                icon.sprite = pdaScreen.friendButtonIcons.Data[data.type.ToString()];
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