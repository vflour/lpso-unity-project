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
            UIHandler uiHandler = (UIHandler)gameUI;
            List<Room> rooms = uiHandler.GetRooms();
            List<GameObject> roomButtons = new List<GameObject>();

            for (int i = 0; i < rooms.Count ; i++)
            {
                GameObject button = Instantiate(roomButtonPrefab,roomButtonContainer);
                RoomSelectButton roomSelect = button.GetComponent<RoomSelectButton>();
                roomSelect.roomInformation = rooms[i];
                roomSelect.uiHandler = uiHandler;
                
                roomButtons.Add(button);

            }
            InitializeRoomPageButtons(roomButtons);
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
