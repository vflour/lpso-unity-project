using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Map
{
    public class PDARequestBox : MonoBehaviour,IPointerClickHandler
    {
        [Header("Sprites")] 
        public Sprite enabled;
        public Sprite disabled;
        [Header("Elements")]
        public FriendRequestMenu friendRequests;
        [SerializeField]
        private Text _userName;
        public Image box;
        
        public string userName
        {
            get => _userName.text;
            set => _userName.text = value;
        }

        public void Enable()
        {
            box.sprite = enabled;
        }

        public void Disable()
        {
            box.sprite = disabled;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            friendRequests.SelectBox(this);
        }
    }
}