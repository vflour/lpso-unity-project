using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Startup
{
    public class MultiplayerScreen : GameScreen
    {
        [Header("Buttons")]
        public UnityEngine.UI.Button connectButton;
        public GameObject joinServerButtonPrefab;

        [Header("Server container")]
        public Transform serverButtonContainer;   
        public ArrowList arrowList;

        [Header("Textbox entries")]
        public UnityEngine.UI.InputField ipBox;
        public UnityEngine.UI.InputField unameBox;
        
        private bool disabled = false;

        private string IP
        {
            get{return ipBox.text;}
        }
        private string uName
        {
            get{return unameBox.text;}
        }

        public void Start()
        {
            buttons.Add(connectButton);
            LoadServers();
        }

        public void LoadServers()
        {
            UIHandler uiHandler = (UIHandler)gameUI;
            List<ServerInformation> serverSaves = uiHandler.GetServerInformationSaves();
            List<GameObject> serverButtons = new List<GameObject>();

            for (int i = 0; i < serverSaves.Count ; i++)
            {
                GameObject button = Instantiate(joinServerButtonPrefab,serverButtonContainer);

                ServerSelectButton buttonComponent = button.GetComponent<ServerSelectButton>();
                buttonComponent.serverInformation = serverSaves[i];
                buttonComponent.uiHandler = uiHandler;
                buttons.Add(button.GetComponent<UnityEngine.UI.Button>());
                serverButtons.Add(button);
            }
            InitializeServerPageButtons(serverButtons);
        }

        public void InitializeServerPageButtons(List<GameObject> serverButtons)
        {
            arrowList.Initialize(serverButtons);
            // adds the arrowlist buttons to the button count if theres any
            foreach(ArrowListButton button in arrowList.arrowListButtons) buttons.Add(button.GetComponent<UnityEngine.UI.Button>());
        }
        public bool IsValid() // Checks if the fields below are correctt
        {
            return (uName.Length > 4 && IP.Length > 0 && !disabled);
        }
        public void ConnectButtonVisibility()
        {

            if(IsValid()) connectButton.interactable = true;
            else connectButton.interactable = false;
        }

        public void ConnectButton(){
            UIHandler uiHandler = (UIHandler)gameUI;
            uiHandler.ConnectToServer(new ServerInformation(IP,uName));
        }

        public override void ToggleInput(bool enabled){
            base.ToggleInput(enabled);
            disabled = !enabled; // disabled is set so that the inputs dont reactivate the button
        }

    }
}