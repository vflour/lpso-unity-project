using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Startup
{
    public class RoomSelectScreen : GameScreen
    {

        [Header("Buttons")]
        public GameObject roomButtonPrefab;

        [Header("Room container")]
        public Transform roomButtonContainer;   
        public ArrowList arrowList;

        private void Start()
        {
            LoadRooms();
        }

        private void LoadRooms()
        {
           gameUI.system.ServerDataRequest("getRooms", data =>
            {

                List<Room> rooms = data.ToObject<List<Room>>();
                List<GameObject> roomButtons = new List<GameObject>();

                foreach(Room room in rooms)
                {
                    GameObject button = Instantiate(roomButtonPrefab,roomButtonContainer);
                    RoomSelectButton roomSelect = button.GetComponent<RoomSelectButton>();
                    roomSelect.roomInformation = room;
                    roomSelect.gameUI = gameUI;
                    roomButtons.Add(button);
                }
                InitializeRoomPageButtons(roomButtons);
            });
        }

        public void InitializeRoomPageButtons(List<GameObject> roomButtons)
        {
            arrowList.Initialize(roomButtons.Count);
            arrowList.InitializePageObject(roomButtons.ToArray());
            // adds the arrowlist buttons to the button count if theres any
            foreach(ArrowListButton button in arrowList.activeButtons) buttons.Add(button.GetComponent<UnityEngine.UI.Button>());
        }
    }
}
