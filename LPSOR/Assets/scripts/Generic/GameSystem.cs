using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
using Game.Networking;

namespace Game
{
    public class GameSystem : MonoBehaviour
    {
        protected NetworkClient networkClient;

        public GameUI uiHandler;
        public GameData gameData;
        public virtual void ProcessNetworkSignal(string signal, int code, JSONObject data)
        {}
        public void LoadScene(int scene)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
        }
        public void ServerDisconnect()
        {
            uiHandler.ShowErrorCode(ErrorCodes.CodeStrings[3]); // sends errorcode to ui 
        }

    }
}
