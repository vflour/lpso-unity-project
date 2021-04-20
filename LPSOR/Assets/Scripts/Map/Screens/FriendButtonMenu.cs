using UnityEngine;

namespace Game.Map
{
    public class FriendButtonMenu : MonoBehaviour
    {
        public PDAScreen pdaScreen;
        public FriendButton friendButton;

        public void SetFriendButton(FriendButton friendButton)
        {
            this.friendButton = friendButton;
            transform.position = friendButton.transform.position + new Vector3(1, 0, 0);
        }
        
        public void SetBFF()
        {
            RelationshipType statusSwitch = friendButton.data.type == RelationshipType.BFF? RelationshipType.Friend : RelationshipType.BFF ;
            string data = "{" + $"\"userName\":\"{friendButton.data.userName}\",\"relationship\":\"{statusSwitch}\"";
            pdaScreen.gameUI.system.ServerDataSend("setRelationship",data);
        }

        public void SendMail()
        {
            pdaScreen.ComposeScreen();
        }

        public void Ignore()
        {
            string data = "{" + $"\"userName\":\"{friendButton.data.userName}\",\"relationship\":\"{RelationshipType.Ignored}\"";
            pdaScreen.gameUI.system.ServerDataSend("setRelationship",data);
        }

        public void Report()
        {
            pdaScreen.gameUI.system.ServerDataSend("reportUser",friendButton.data.userName);
        }
    }
}