using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Networking 
{

    public class PlayerKey
    {
        public PlayerKey(string userName){
            this.userName = userName;
            this.keyId = null;
        }
        public PlayerKey(string userName, string keyId){
            this.userName = userName;
            this.keyId = keyId;
        }
        public string userName;
        public string keyId;
    }
    public class NetworkJSONClass 
    {
    }

}
