using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Startup
    {
    public class RoomSelectButton : MonoBehaviour
    {
        [Header("Room Information")]
        private Room roomInfo;
        public Room roomInformation
        {
            set
            {
                roomInfo = value;
                nameField.text = roomInfo.name;
                buddyField.text = roomInfo.buddiesInRoom.ToString();
                
                populationSlider.value = Mathf.Clamp01((float)roomInfo.population/(float)roomInfo.maxPopulation);
            }
            get {return roomInfo;}
        }

        [Header("Text fields")]
        public Text nameField;
        public Text buddyField;
        public Slider populationSlider;
        public Image populationSprite;

        [Header("Classes")]
        public GameUI gameUI;
        public void SelectServer()
        {
            gameUI.ToggleScreenInput(false); 
            gameUI.system.gameData.roomData = roomInfo;
            gameUI.system.Emit("loadScene", 1);
        }
    
    }

}
