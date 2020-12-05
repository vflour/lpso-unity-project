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
        private List<ServerInformation> serverSaves;
        private List<GameObject> serverButtons = new List<GameObject>();
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
            ClearServerButtons();
            UIHandler uiHandler = (UIHandler)gameUI;
            serverSaves = uiHandler.GetServerInformationSaves();
            

            for (int i = 0; i < serverSaves.Count ; i++)
            {
                GameObject button = Instantiate(joinServerButtonPrefab,serverButtonContainer);
                ServerSelectButton buttonComponent = button.GetComponent<ServerSelectButton>();
                buttonComponent.serverInformation = serverSaves[i];
                buttonComponent.uiHandler = uiHandler;
                buttonComponent.mpScreen = this;
                buttons.Add(button.GetComponent<UnityEngine.UI.Button>());
                serverButtons.Add(button);
            }
            InitializeServerPageButtons(serverButtons);
        }

        // lazy  way of clearing server buttons,
        public void ClearServerButtons()
        {
            foreach (GameObject button in serverButtons) GameObject.Destroy(button);

            serverButtons = new List<GameObject>();
            buttons = new List<UnityEngine.UI.Button>();

        }
        public void InitializeServerPageButtons(List<GameObject> serverButtons)
        {
            arrowList.Initialize(serverButtons.Count);
            arrowList.InitializePageObject(serverButtons.ToArray());
            // adds the arrowlist buttons to the button count if theres any
            foreach(ArrowListButton button in arrowList.activeButtons) buttons.Add(button.GetComponent<UnityEngine.UI.Button>());
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

        public override void ToggleInput(bool enabled){
            base.ToggleInput(enabled);
            disabled = !enabled; // disabled is set so that the inputs dont reactivate the button
        }

        public void RemoveServer(ServerInformation serverInformation)
        {
            UIHandler uiHandler = (UIHandler)gameUI;
            serverSaves.Remove(serverInformation);
            uiHandler.RemoveServer(new ServerInformation(IP,uName));
            LoadServers();
        }

    }
}