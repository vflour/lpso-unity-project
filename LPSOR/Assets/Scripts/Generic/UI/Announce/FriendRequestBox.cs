using UnityEngine;

namespace Game.UI
{
    public class FriendRequestBox : AnnounceChoiceBox
    {
        public string userName;
        public void Start()
        {
            message.text = $"Add {userName} as a buddy?";
            
            SetCallbacks(
                () =>
                {
                    gameUI.system.ServerDataSend("recieveFrRequest", $"{{\"targetUser\":\"{userName}\",\"accepted\":false}}");
                }, // only close the prompt for deny
                () => // accept "accepts" the friend request
                {
                    gameUI.system.ServerDataSend("recieveFrRequest", $"{{\"targetUser\":\"{userName}\",\"accepted\":true}}");
                });
        }
    }
}