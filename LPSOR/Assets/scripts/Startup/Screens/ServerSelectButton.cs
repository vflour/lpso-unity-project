using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Startup
    {
    public class ServerSelectButton : MonoBehaviour
    {
        [Header("Server Information")]
        private ServerInformation serverInfo;
        public ServerInformation serverInformation
        {
            set
            {
                serverInfo = value;
                ipField.text = serverInfo.IP;
                UNameField.text = serverInfo.userName;
            }
            get {return serverInfo;}
        }

        [Header("Text fields")]
        public Text ipField;
        public Text UNameField;

        [Header("Classes")]
        public UIHandler uiHandler;
        public void SelectServer()
        {
            uiHandler.ConnectToServer(serverInformation);
        }
    
    }

}
