using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Map
{
    public class PlayerViewerScreen : GameScreen
    {
        [Header("Palette")] 
        public PlayerViewerPalette[] palettes;
        private PlayerViewerPalette palette; //palette more like PAINette
        private PaletteType type;
        
        public enum PaletteType{User,Player}
        // elements changed by palette 
        public Button[] petButtons;
        public PrefabDictionary mainBoxes;
        public Image[] textFields;
        public Text[] colorableText;
        
        [Header("GameObjects")]
        public IconDictionary buttonIcons;
        public IconDatabase membershipIcons;
        
        public RawImage characterFrame; 
        public Text userNameBox;
        public Text serverBox;
        public Text characterNameBox;
        public Text ticketTypeBox;
        public Image memberStatusBox;
        public Text characterCount;
        public Image FriendshipStatus;
        public Button[] viewerButtons;
        
        public enum ViewerButtons{Quest,Journal,Wardrobe,Home,Emotes,BFF,AddBFF,RemoveBFF,Friend,AddFriend,RemoveFriend,Ignore,Report}
        [Header("Pet Collection Box")] 
        public ArrowList petArrowList;
        [Header("Player data")] 
        public PlayerData playerData;
        public int currentCharacter;
        public CharacterData[] characters;

        private CharacterHandler characterHandler;
        private UserHandler userHandler;
        #region Character Loading
        // Loads the arrowlist and the user's character
        public void SetCharacter(PlayerData playerData, CharacterData[] characters, string sessionId)
        {
            // set the data fields
            this.playerData = playerData;
            this.characters = characters;
            // load the sprites
            characterHandler = gameUI.system.GetHandler<CharacterHandler>();
            userHandler = gameUI.system.GetHandler<UserHandler>();
            StartCoroutine(LoadCharacters(sessionId));
            // load the text fields
            LoadText();
        }
 
        //loads the arrow list by instantiating the profile rawimages
        public IEnumerator LoadCharacters(string sessionId)
        {
            // instantiate all of the profiles
            List<GameObject> profiles = new List<GameObject>();
            foreach (CharacterData characterData in characters)
            {
                GameObject profile = new GameObject("profile",typeof(RectTransform),typeof(RawImage));
                RawImage image = profile.GetComponent<RawImage>();
                profile.GetComponent<RectTransform>().sizeDelta = new Vector2(150,150);
                yield return characterHandler.GenerateProfile(characterData._id, characterData, true, (texture) => image.texture = texture );
                yield return new WaitForEndOfFrame();
                profiles.Add(profile);
            }
            // then parent them to a specific button
            for (int i = 0; i < profiles.Count; i++)
            {
                int parentIndex = i % 12;
                GameObject profile = profiles[i];
                Transform parent = petButtons[parentIndex].transform;
                profile.transform.SetParent(parent);
                profile.transform.localScale = Vector3.one;
                profile.transform.rotation = Quaternion.Euler(0,0,20);
                profile.transform.localPosition = Vector3.zero;
            }
            // set the arrowlist
            petArrowList.Initialize(profiles.ToArray());
            // load the char frame
            yield return LoadCharacterFrame(sessionId);
        }
        public void SetEmptySlotButtons(int max)
        {
            for (int i = 0; i < petButtons.Length; i++)
            {
                petButtons[i].gameObject.SetActive(i<max);    
                petButtons[i].transform.parent.GetComponent<Image>().enabled = i>=max;
            }
        }
        
        // Loads the characterframe
        public IEnumerator LoadCharacterFrame(string id)
        {
            yield return new WaitForEndOfFrame();
            CharacterData characterData = characters.ToList().Find(data => data._id == id);
            StartCoroutine(characterHandler.GenerateProfile(characterData._id+"_pviewer", characterData, false, (texture) => characterFrame.texture = texture ));

            characterNameBox.text = characterData.name;
            ticketTypeBox.text = ((PetSelect.SlotType) characterData.ticket).ToString();
        }

        public void LoadText()
        {
            userNameBox.text = playerData.userName;
            serverBox.text = gameUI.system.gameData.roomData.name;
            serverBox.color = new Color(serverBox.color.r, serverBox.color.g, serverBox.color.b, 0.75f);
            
            characterCount.text = characters.Length.ToString();
            FriendshipStatus.gameObject.SetActive(type==PaletteType.Player);
        }
        #endregion
        #region Palette 
        // Sets the palette and the buttons
        public void SetPalette(PaletteType type)
        {
            palette = palettes[(int) type];
            this.type = type;
            LoadPalette();
            LoadButtons();
        }
        // Loads the palette
        public void LoadPalette()
        {
            // load the pet buttons
            for(int i = 0; i<petButtons.Length;i++)
            {
                Button button = petButtons[i];
                button.GetComponent<Image>().sprite = palette.petButtonSprites[i];
                button.transform.parent.GetComponent<Image>().sprite = palette.emptyBoxSprite;
            }
            // load the fields
            foreach (Image field in textFields)
                field.sprite = palette.fieldSprite;
            //load the text colors
            foreach (Text text in colorableText)
                text.color = palette.textColor;
            //load the main boxes
            foreach (string box in mainBoxes.Keys)
                mainBoxes[box].GetComponent<Image>().sprite = palette.mainBoxes[box];
        }
        #endregion

        #region Buttons

        public void LoadButtons()
        {
            for(int i = 0; i < palette.buttons.Length; i++)
                LoadButton(i);    
        }

        public void LoadButton(int i)
        {
            ViewerButtons buttonType = palette.buttons[i];
            bool disabled = false;
            // there are cases where the button type must change (bff/friend)
            // or if it must be disabled
            switch(buttonType)
            {
                case ViewerButtons.BFF: // if the button is a "Bff" type, check if the user is a bff or a friend
                    userHandler.GetFriend(playerData.userName, (relationshipData) =>
                    {
                        buttonType = ViewerButtons.AddBFF;
                        if (relationshipData != null)
                            buttonType = relationshipData.type == RelationshipType.BFF
                                ? ViewerButtons.RemoveBFF
                                : ViewerButtons.AddBFF;
                        else
                            disabled = true;
                        SetButton(i,disabled,buttonType);
                    });
                    break;
                case ViewerButtons.Friend: // if the button is a "Friend" type, check if the user is in the friends list
                    userHandler.GetFriend(playerData.userName, (relationshipData) =>
                    {
                        Debug.Log(relationshipData);
                        buttonType = relationshipData != null ? ViewerButtons.RemoveFriend : ViewerButtons.AddFriend;
                        SetButton(i,disabled,buttonType);
                    });
                    break;
                case ViewerButtons.Home: // home is disabled by default for now
                    disabled = true;
                    SetButton(i,disabled,buttonType);
                    break;
                default:
                    SetButton(i,disabled,buttonType);
                    break;
            }

            // now the button instance itself
        }

        public void SetButton(int i, bool disabled, ViewerButtons buttonType)
        {
            Button button = viewerButtons[i];
            button.transform.Find("icon").GetComponent<Image>().sprite = buttonIcons[buttonType.ToString()];
            button.interactable = !disabled;
            SetButtonFunction(buttonType,button);
        }

        public void SetButtonFunction(ViewerButtons buttonType, Button button)
        {
            // why
            switch (buttonType)
            {
                case ViewerButtons.AddFriend:
                    button.onClick.AddListener(AddFriendButton);
                    break;
                case ViewerButtons.RemoveFriend:
                    button.onClick.AddListener(RemoveFriendButton);
                    break;
                case ViewerButtons.AddBFF:
                    button.onClick.AddListener(AddBFFButton);
                    break;
                case ViewerButtons.RemoveBFF:
                    button.onClick.AddListener(RemoveBFFButton);
                    break;
            }
        }
        
        // below are all the methods for each button type
        private void QuestButton()
        {
            
        }

        private void JournalButton()
        {
            
        }

        private void WardrobeButton()
        {
            
        }

        private void HomeButton()
        {
            
        }

        private void EmoteButton()
        {
            
        }

        private void AddBFFButton()
        {
            userHandler.UpdateFriendStatus(playerData.userName,RelationshipType.BFF);
        }

        private void RemoveBFFButton()
        {
            userHandler.UpdateFriendStatus(playerData.userName,RelationshipType.Friend);
        }

        private void AddFriendButton()
        {
            Debug.Log("pressed");
            gameUI.system.ServerDataRequest("sendFrRequest",playerData.userName, (valid) =>
            {
                Debug.Log("Valid friend request "+valid);
            });
        }

        private void RemoveFriendButton()
        {
            gameUI.system.ServerDataSend("removeFriend",playerData.userName);
        }

        private void BlockButton()
        {
            
        }

        private void ReportButton()
        {
            
        }
        #endregion
    }
}

