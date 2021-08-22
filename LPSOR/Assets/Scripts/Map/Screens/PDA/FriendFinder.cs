using UnityEngine;
using UnityEngine.UI;

namespace Game.Map
{
    public class FriendFinder : MonoBehaviour
    {
        public string defaultTitle = "Add a new buddy by their user.";
        public Color defaultColor = new Color(0.1058823f,0.4767394f,0.7843137f);
        
        public InputField input;
        public Text title;
        public PDAScreen pda;

        public void OnEnable()
        {
            title.text = defaultTitle;
            title.color = defaultColor;  
        }
        public void FindFriend()
        {
            pda.gameUI.system.ServerDataRequest("sendFrRequest",input.text, data =>
            {
                bool valid = data.ToObject<bool>();
                if (valid)
                {
                    title.text = "Friend request sent.";
                }
                else
                {
                    title.text = "Friend not found. Try again.";
                    title.color = Color.red;
                }
                
            });
        }

    }
}