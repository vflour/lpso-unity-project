using UnityEngine;

namespace Game.UI
{
    public class NewFriendBox : AnnounceBox
    {
        public string userName;
        public void Start()
        {
            title.text = $"New Buddy: {userName}!";
        }
    }
}