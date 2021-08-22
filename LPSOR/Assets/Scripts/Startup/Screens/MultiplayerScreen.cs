using System.Collections;
using System.Collections.Generic;
using Game.Startup;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("Form entries")]
        public UnityEngine.UI.InputField ipBox;
        public UnityEngine.UI.InputField unameBox;
        public UnityEngine.UI.Toggle enableKeyBox;
        public UnityEngine.UI.InputField keyBox;
        public UnityEngine.UI.Toggle keyVisible;

        [Header("Images")]
        public Sprite keyVisibleOn;
        public Sprite keyVisibleOff;
        
        private bool disabled = false;
        private List<ServerInformation> serverSaves;
        private ServerInformation serverInfo;
        private List<GameObject> serverButtons = new List<GameObject>();
        private string IP
        {
            get{return ipBox.text;}
            set { ipBox.text = value; }
        }
        private string uName
        {
            get{return unameBox.text;}
            set
            {
                unameBox.text = value;
            }
        }

        private bool keyIdEnabled
        {
            set
            {
                keyBox.transform.parent.gameObject.SetActive(value);
                keyVisible.transform.parent.gameObject.SetActive(value);
                enableKeyBox.isOn = value;
                if (!value)
                    keyId = "";
                else
                    keyIdVisible = false;
            }
        }

        private bool keyIdVisible
        {
            set
            {
                if (value)
                {
                    keyBox.contentType = InputField.ContentType.Alphanumeric;
                    keyVisible.GetComponentInChildren<Image>().sprite = keyVisibleOn;
                }
                else
                {
                    keyBox.contentType = InputField.ContentType.Password;
                    keyVisible.GetComponentInChildren<Image>().sprite = keyVisibleOff; 
                }
                keyBox.ForceLabelUpdate();
            }
        }
        private string keyId
        {
            get
            {
                if (keyBox.text == "")
                    return null;
                return keyBox.text;
            }
            set
            {
                keyBox.text = value;
            }
        }

        public ServerInformation serverInformation
        {
            get { return serverInfo; }
            set
            {
                serverInfo = value;
                uName = serverInformation.userName;
                IP = serverInformation.IP;
                keyId = serverInformation.keyId;
                keyIdEnabled = true;
            }
        }

        public void Start()
        {
            buttons.Add(connectButton);
            LoadServers();
        }

        #region Server Entries
        public void LoadServers()
        {
            ClearServerButtons();
            serverSaves = gameUI.system.Request<List<ServerInformation>>("serverInformation");
            
            for (int i = 0; i < serverSaves.Count ; i++)
            {
                GameObject button = Instantiate(joinServerButtonPrefab,serverButtonContainer);
                ServerSelectButton buttonComponent = button.GetComponent<ServerSelectButton>();
                buttonComponent.serverInformation = serverSaves[i];
                buttonComponent.gameUI = gameUI;
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
            arrowList.Initialize(serverButtons.ToArray());
            // adds the arrowlist buttons to the button count if theres any
            foreach(ArrowListButton button in arrowList.activeButtons) buttons.Add(button.GetComponent<UnityEngine.UI.Button>());
        }
        
        // Removes the specified server entry
        public void RemoveServer(ServerInformation serverInformation)
        {
            StartupSystem startupSystem = (StartupSystem) gameUI.system;
            serverSaves.Remove(serverInformation);
            startupSystem.RemoveServer(new ServerInformation(IP,uName,new List<RelationshipData>()));
            LoadServers();
        }
        #endregion
        
        #region Field Checking
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
        #endregion
        #region Buttons
        public void ConnectButton()
        {
            UpdateServerInfo();
            StartupSystem startupSystem = (StartupSystem) gameUI.system;
            startupSystem.ConnectUser(serverInformation);
        }
        public void UpdateServerInfo()
        {
            serverInfo.userName = uName;
            serverInfo.IP = IP;
            serverInfo.keyId = keyId;
        }
        public void EnableKeyButton()
        {
            keyIdEnabled = enableKeyBox.isOn;
        }

        public void KeyVisibleButton()
        {
            keyIdVisible = keyVisible.isOn;
        }
        #endregion
    }
}