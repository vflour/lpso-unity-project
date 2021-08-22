using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Networking;
using UnityEngine;

namespace Game.Map
{
    public class UserHandler : MonoBehaviour, IHandler
    {
        public Player localPlayer;
        public bool occupied;
        public Camera currentCamera;
        public Vector3 cameraPosition;
        
        #region Initialization
        // IHandler methods
        public GameSystem system {get; set;}

        private bool display = false;

        public bool Display
        {
            get { return display;}
            set { display = value; }
        }

        // Initializes the localplayer and its character
        public void Activate()
        {
            // Init localplayer and spawn character
            PlayerHandler playerHandler = system.GetHandler<PlayerHandler>();
            GameData gameData = system.gameData;
            localPlayer = playerHandler.LoadPlayer(gameData.playerData,gameData.sessionData);

            // Enable playerHandler events
            playerHandler.PlayerEvents();
            
            // Tell the server to join the room's map
            string mapName = system.GetHandler<MapHandler>().mapData.mapName;
            string roomName = system.gameData.roomData.name;
            system.ServerDataSend("joinMap", "{"+$"\"roomName\":\"{roomName}\",\"mapName\":\"{mapName}\""+"}" );
            
            // Enable raycasting on mouse
            MouseHandler mouseHandler = system.GetHandler<MouseHandler>();
            mouseHandler.RaycastListen("walkOnTile",RaycastOnTile);
            mouseHandler.RaycastListen("propInteract", RaycastOnProp);
            mouseHandler.RaycastListen("characterMenu", RaycastOnCharacter);
            // Init camera position
            Vector3 tilePosition = localPlayer.session.targetTile.transform.position;
            currentCamera.transform.position = new Vector3(tilePosition.x,tilePosition.y,-10);
            Display = true;
        }
        
        #endregion
        #region Camera following

        // Execute lateupdate to move the camera
        public void LateUpdate()
        {
            if (Display)
            {
                FocusCamera();
                FollowCamera();
            }
                
        }
        // Camera is meant to follow the player each frame
        private void FollowCamera()
        {
            Vector3 velocity = Vector3.zero;
            currentCamera.transform.position = Vector3.SmoothDamp(currentCamera.transform.position, cameraPosition, ref velocity, 0.23f);
        }

        public Vector3 vp;
        private void FocusCamera()
        {
            // Focus on the character's target tile if the character reaches the edge of the viewpor
            Vector2 center = new Vector2(0.5f, 0.5f);
            Vector2 viewportPosition = center - (Vector2)currentCamera.ScreenToViewportPoint(currentCamera.WorldToScreenPoint(localPlayer.session.transform.position));
            vp = viewportPosition;
            if (viewportPosition.magnitude > 0.17f)
            {
                Vector3 tilePosition = localPlayer.session.targetTile.transform.position;
                cameraPosition = new Vector3(tilePosition.x,tilePosition.y,-10);
            }
        }
        #endregion
        #region Tile Raycast
        // Raycast event method for user selecting a tile.
        public void RaycastOnTile(RaycastHit2D hit)
        {
            // First, try to get the node from the raycast hit. If not, then exit the program
            TileNode node = hit.transform.gameObject.GetComponent<TileNode>();
            if (node == null || occupied) return;
            
            // Tell the player to move to a specific point
            MapHandler mapHandler = system.GetHandler<MapHandler>();
            
            localPlayer.MoveCharacter(node.x, node.y);
            system.ServerDataSend("moveTile","{"+$"\"x\":{node.x},\"y\":{node.y}"+"}");
        }
        #endregion
        #region Prop raycast
        // Raycast event method for user selecting a prop
        public void RaycastOnProp(RaycastHit2D hit)
        {
            Prop prop = hit.transform.gameObject.GetComponent<Prop>();
            if (prop == null || occupied) return;
            if (prop.isOccupied) return;
            prop.Interact(localPlayer);
            system.ServerDataSend("propInteract",prop.id.ToString());
        }
        #endregion
        #region Character raycast

        public void RaycastOnCharacter(RaycastHit2D hit)
        {
            // try to get character   
            Character character = hit.transform.gameObject.GetComponent<Character>();
            if (character == null) return;
            // try to get player
            PlayerHandler playerHandler = system.GetHandler<PlayerHandler>();
            Player player = playerHandler.GetPlayerFromCharacter(character);
            if (player == null) return;
            //see if the character belongs to the user
            bool isUserCharacter = localPlayer.session.data._id == character.data._id;
            if (isUserCharacter) // if so, open the circle menu
            {
                
            }
            else // if not, open the player viewer
            {
                // get gameUI
                GameUI gameUI = system.GetHandler<GameUI>();
                if (!gameUI.HasScreen("PlayerViewer"))
                { // add pv if screen isnt there
                    PlayerViewerScreen screen = gameUI.InstantiateScreen("PlayerViewer").GetComponent<PlayerViewerScreen>(); 
                    gameUI.system.ServerDataRequest("getAllCharacters", player.data.userName, (data =>
                    {
                        CharacterData[] characters = data.ToObject<CharacterData[]>();
                        screen.SetCharacter(player.data, characters, player.session.data._id);
                        screen.SetPalette(PlayerViewerScreen.PaletteType.Player);
                    }));
                }
            }
                
        }
        

        #endregion
        #region Local Player chatting

        public void SendChat(string message)
        {
            localPlayer.ChatCharacter(message);
            system.ServerDataSend("chat", message);
        }
        #endregion
        #region Friendship and BFFs

        public void GetFriends(Action<List<RelationshipData>> resultDelegate)
        {
            system.ServerDataRequest("getFriends", (data) =>
            {
                List<RelationshipData> friendList = data.ToObject<List<RelationshipData>>();
                SetRelationships(friendList);
                resultDelegate(friendList);
            });
        }

        private void SetRelationships(List<RelationshipData> friendList)
        {
            ServerInformation serverInfo = system.gameData.serverInformation;
            // cycle through relationships stored locally
            foreach (RelationshipData relationship in serverInfo.friendRelationships)
            {
                RelationshipData match = GetRelationship(friendList, relationship.userName);
                if (match!=null)
                    match.type = relationship.type;
                else if(relationship.type == RelationshipType.Ignored) // add to friendlist ONLY if it's ignored
                    friendList.Add(relationship);
            }
        }

        public void UpdateFriendStatus(string userName, RelationshipType type)
        {   
            GetFriend(userName, (relationship) =>
            {
                // create a new relationship if it doesnt exist (used for ignoring)
                relationship = relationship == null ? new RelationshipData(){isOnline=false,type=type,userName=userName} : relationship;
                RelationshipType previousType = relationship.type;
                // get serverinfo and update the type
                ServerInformation serverInfo = system.gameData.serverInformation;
                relationship.type = type;
                int match =
                    serverInfo.friendRelationships.FindIndex((data) => data.userName == relationship.userName);
                if (match == -1) // add an entry if it doesnt exist
                    serverInfo.friendRelationships.Add(relationship);
                else if (previousType == RelationshipType.Ignored) // remove from list if they're being un-ignored
                    serverInfo.friendRelationships.RemoveAt(match);
                ServerDataPersistence.SaveOneServer(serverInfo);
            });
        }

        private RelationshipData GetRelationship(List<RelationshipData> friendList, string userName)
        {
            return friendList.Find((data) => data.userName == userName);
        }
        public void GetFriend(string userName, Action<RelationshipData> resultDelegate)
        {
            GetFriends((friendList) =>
            {
                RelationshipData friend = GetRelationship(friendList, userName);
                resultDelegate(friend);
            }); 
        }
        #endregion
    } 
}

