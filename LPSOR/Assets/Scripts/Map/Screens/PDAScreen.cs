using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Map
{
    using UI;
    public class PDAScreen : GameScreen
    {
        [Header("Sub-Screen Presets")] 
        [SerializeField]
        private GameObject homeScreen;
        [SerializeField]
        private GameObject inboxScreen;
        [SerializeField]
        private GameObject adventuresScreen;
        [SerializeField]
        private GameObject friendsScreen;
        
        private Animator animator;

        private GameObject activeScreen;
        // pda opens on home screen
        // buttons open mail, friends, adventures and ???
        
        #region Home Screen / Start

        [Header("Home Screen")] 
        public GameObject eventBoxPrefab;
        public GameEventDictionary gameEventData;
        public GameObject[] emptyEventBoxes;

        private List<GameEventBox> eventBoxes;
        private string eventFilter;
        
        private void Start()
        {
            animator = GetComponent<Animator>();
            HomeScreen();
        }

        public void HomeScreen()
        {
            SetActiveScreen(homeScreen);
            GenerateEvents();
        }

        
        private void GenerateEvents()
        {
            ClearEvents();
            List<GameEvent> gameEvents = gameUI.system.gameData.events;
            Transform eventBoxLocation = emptyEventBoxes[0].transform.parent;
            int emptyBoxIndex = 0;
            foreach(GameEvent gameEvent in gameEvents)
            {
                GameEventBox eventBox = Instantiate(eventBoxPrefab,eventBoxLocation).GetComponent<GameEventBox>();
                eventBox.title.text = gameEvent.title;
                eventBox.text.text = gameEvent.text;
                eventBox.icon.sprite = friendButtonIcons.Data[gameEvent.icon.ToString()];
                eventBoxes.Add(eventBox);
                
                // disable empty boxes
                if (emptyBoxIndex < 4)
                {
                    emptyEventBoxes[emptyBoxIndex].SetActive(false);
                    emptyBoxIndex++;
                }
            }    
        }

        private void ClearEvents()
        {
            for (int i = 0; i<eventBoxes.Count;i++)
            {
                Destroy(eventBoxes[i].gameObject);
                eventBoxes.RemoveAt(i);
            }
                
            foreach (GameObject emptyBox in emptyEventBoxes)
                emptyBox.SetActive(true);    
        }

        public void Exit()
        {
            
        }

        public void SetActiveScreen(GameObject activeScreen)
        { 
            this.activeScreen.SetActive(false);
            this.activeScreen = activeScreen;
            activeScreen.SetActive(true);
        }

        #endregion

        #region Inbox

        // Show user's inbox screen
        public void InboxScreen()
        {
            SetActiveScreen(inboxScreen);
        }

        // Screen where the user composes mail
        public void ComposeScreen()
        {
            
        }

        #endregion

        #region Adventures

        public void AdventuresScreen()
        {
            SetActiveScreen(adventuresScreen);
        }

        #endregion

        #region Friends list
        [Header("Friends list")]
        public List<RelationshipData> friendList;
        public GameObject friendButtonPrefab;
        public IconDatabase friendButtonIcons;
        
        public Transform friendButtonContainer;
        public Dropdown filterDropdown;
        public InputField filterNameInput;
        
        private RelationshipType friendFilterType;
        private string friendFilterName;
        private Dictionary<string, FriendButton> friendButtons = new Dictionary<string, FriendButton>();
        private FriendButtonMenu friendButtonMenu;
        
        
        public void FriendsScreen()
        {
            SetActiveScreen(friendsScreen);
            friendFilterType = RelationshipType.Friend;
            GenerateFriendsList();
        }

        private void GenerateFriendsList()
        {
            
            foreach (RelationshipData friend in friendList)
            {
                bool validType = friend.type == friendFilterType || (friend.type != RelationshipType.Ignored &&
                                                                     friendFilterType == RelationshipType.Friend);
                bool validName = CheckValidSearchName(friend.userName);
                if(validType && validName)
                    CreateFriendButton(friend);
            }
        }

        private void ClearFriendsList()
        {
            string[] keys = friendButtons.Keys.ToArray();
            foreach (string userName in keys)
            {
                Destroy(friendButtons[userName].gameObject);
                friendButtons.Remove(userName);
            }

        }

        private void CreateFriendButton(RelationshipData friend)
        {
            FriendButton friendButton = Instantiate(friendButtonPrefab, friendButtonContainer).GetComponent<FriendButton>();
            friendButton.data = friend;
            friendButton.pdaScreen = this;
            friendButtons.Add(friend.userName, friendButton);
        }

        private bool CheckValidSearchName(string userName)
        {
            if (friendFilterName != "")
            {
                Regex regex = new Regex("^"+friendFilterName);
                return regex.IsMatch(userName);
            }
            else
                return true;
        }

        public void FriendTypeFilter()
        {
            friendFilterName = filterNameInput.text;
            GenerateFriendsList();
        }
        public void FriendSearchFilter()
        {
            friendFilterType = (RelationshipType) filterDropdown.value;
            GenerateFriendsList();
        }

        public void CreateFriendButtonMenu(string userName)
        {
            friendButtonMenu.gameObject.SetActive(true);
            friendButtonMenu.SetFriendButton(friendButtons[userName]);
        }

        
        #endregion
    }
}