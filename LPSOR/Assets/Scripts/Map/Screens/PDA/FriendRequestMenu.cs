using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Map
{
    public class FriendRequestMenu : MonoBehaviour
    {
        public PDAScreen pda;
        [Header("Container")]
        public Transform container;
        [Header("Prefabs")] 
        public GameObject boxPrefab;
        
        public List<string> requests;
        private List<PDARequestBox> requestBoxes = new List<PDARequestBox>();

        #region Generation
        public void OnEnable()
        {
            GetRequests();
        }

        private void GetRequests()
        {
            Generate();
   
        }
        
        private void Generate()
        {
            Clear();
            foreach (string userName in requests)
            {
                GenerateBox(userName);
            }

        }

        private void GenerateBox(string userName)
        {
            GameObject box = Instantiate(boxPrefab, container);
            PDARequestBox boxComponent = box.GetComponent<PDARequestBox>();
            boxComponent.userName = userName;
            boxComponent.friendRequests = this;
            requestBoxes.Add(boxComponent);
        }

        private void Clear()
        {
            foreach (PDARequestBox box in requestBoxes)
            {
                Destroy(box.gameObject);
            }

            requestBoxes = new List<PDARequestBox>();
        }
        #endregion

        #region Selection and Interaction

        private PDARequestBox _currentBox;
        private PDARequestBox currentBox
        {
            get => _currentBox;
            set
            {
                value?.Enable();
                _currentBox?.Disable();
                _currentBox = value;
            }
        }

        public void SelectBox(PDARequestBox box)
        {
            currentBox = box;
        }

        public void RemoveBox(PDARequestBox box)
        {
            requestBoxes.Remove(box);
            Destroy(box.gameObject);
        }

        public void AcceptRequest()
        {
            SendFeedback("true");
        }

        public void DenyRequest()
        {
            SendFeedback("false");
        }

        public void SendFeedback(string feedback)
        {
            pda.gameUI.system.ServerDataSend("recieveFrRequest", $"{{\"targetUser\":\"{currentBox.userName}\",\"accepted\":{feedback}}}");
            RemoveBox(currentBox);
            currentBox = null;
        }

        #endregion
        
    }
}