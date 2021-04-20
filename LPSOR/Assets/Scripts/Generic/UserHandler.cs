using System;
using System.Collections;
using System.Collections.Generic;
using Game;
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
            playerHandler.OnPlayerAdded();
            playerHandler.OnPlayerMoveTile();
            playerHandler.OnPlayerChatted();
            playerHandler.OnPlayerPropInteracted();
            playerHandler.OnPlayerRemoving();
            playerHandler.OnPlayerDressed();
            
            // Tell the server to join the room's map
            string mapName = system.GetHandler<MapHandler>().mapData.mapName;
            string roomName = system.gameData.roomData.name;
            system.ServerDataSend("joinMap", "{"+$"\"roomName\":\"{roomName}\",\"mapName\":\"{mapName}\""+"}" );
            
            // Enable raycasting on mouse
            MouseHandler mouseHandler = system.GetHandler<MouseHandler>();
            mouseHandler.RaycastListen("walkOnTile",RaycastOnTile);
            mouseHandler.RaycastListen("propInteract", RaycastOnProp);
            
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
        #region Local Player chatting

        public void SendChat(string message)
        {
            localPlayer.ChatCharacter(message);
            system.ServerDataSend("chat", message);
        }
        #endregion
        
    } 
}

