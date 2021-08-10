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
        public GameObject darkener; // used to darken the screen
        
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
        public GameObject[] emptyEventBoxes;
        public GameEventDictionary gameEventData;
        public GameObject eventBoxPrefab;
        
        private List<GameEventBox> eventBoxes = new List<GameEventBox>();
        private string eventFilter;
        
        private void Start()
        {
            // get animator
            animator = GetComponent<Animator>();
            // open home screen
            HomeScreen();
        }

        private void Darken(bool value)
        {
            darkener.SetActive(value);
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
            gameUI.RemoveScreen("PDA");
        }

        public void SetActiveScreen(GameObject activeScreen)
        {
            if (this.activeScreen!=null)
            {
                this.activeScreen.SetActive(false);
            }
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
        private List<RelationshipData> friendList;
        [Header("Friends List")]
        // prefabs and icons for the friends list
        public GameObject friendButtonPrefab;
        public IconDatabase friendButtonIcons;
        // friend finder 
        public FriendFinder friendFinder;
        //friend requests
        public FriendRequestMenu friendRequests;
        public Button requestButton;
        
        // containers for the filter
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
            GenerateFriends();
            GenerateRequests();
        }

        public void FriendFinder(bool open)
        {
            Darken(open);
            friendFinder.gameObject.SetActive(open);
        }

        public void FriendRequestScreen(bool open)
        {
            friendRequests.gameObject.SetActive(open); 
        }
        
        private void GenerateFriends()
        {
            gameUI.system.ServerDataRequest("getFriends", (data) =>
            {
                friendList = data.ToObject<List<RelationshipData>>();
                SetRelationships();
                GenerateFriendsList();
            });
        }

        private void GenerateRequests()
        {
            gameUI.system.ServerDataRequest("getFriendRequests", data =>
            {
                List<string> requests = data.ToObject<List<string>>();
                friendRequests.requests = requests;
                requestButton.interactable = requests.Count > 0;
                //set text for button
                string plural = requests.Count == 1 ? "" : "s";
                requestButton.gameObject.transform.GetComponentInChildren<Text>().text = $"{requests.Count} request{plural}";
            });
        }

        private void SetRelationships()
        {
            ServerInformation serverInfo = gameUI.system.gameData.serverInformation;
            
            foreach (RelationshipData relationship in serverInfo.friendRelationships)
            {
                RelationshipData inFriendsList = friendList.Find((data) => data.userName == relationship.userName);
                if (inFriendsList != null)
                    inFriendsList.type = relationship.type;
                else 
                    friendList.Add(relationship);
            }
        }
        
        private void GenerateFriendsList()
        {
            ClearFriendsList();
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
            friendButton.pdaScreen = this;
            friendButton.data = friend;
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