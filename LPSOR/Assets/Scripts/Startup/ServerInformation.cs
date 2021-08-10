using System.Collections;
using System.Collections.Generic;
using Game.UI.Startup;
using UnityEngine;
using Game;

[System.Serializable]
public struct ServerInformation
{
    // variant meant for connecting to the server without a key
    public ServerInformation(string ip, string uName,List<RelationshipData> friendRelationships)
    { 
        this.IP = ip;
        this.userName = uName;
        this.keyId = "";
        this.friendRelationships = friendRelationships;
    }
    // variant that contains key
    public ServerInformation(string ip, string uName, string keyId, List<RelationshipData> friendRelationships)
    { 
        this.IP = ip;
        this.userName = uName;
        this.keyId = keyId;
        this.friendRelationships = friendRelationships;
    }
    public string userName;
    public string keyId;
    public string IP;
    public List<RelationshipData> friendRelationships;
}