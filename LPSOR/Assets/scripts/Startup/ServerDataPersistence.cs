using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

namespace Game.Networking
{
    public class ServerDataPersistence : MonoBehaviour
    {
        public static void SaveServerData(List<ServerInformation>  serverInfo) // Serializes server info into a binary file
        {
            string dataPath = Application.persistentDataPath+"/serverInformation.sav";
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(dataPath,FileMode.Create);
            bf.Serialize(fs,serverInfo.ToArray());
            fs.Close();
        }

        public static List<ServerInformation> LoadServerData() // deserializes the serverInformation binary if there is one
        {
            List<ServerInformation> data = new List<ServerInformation>();

            string dataPath = Application.persistentDataPath+"/serverInformation.sav";
            if(File.Exists(dataPath))
            {
                ServerInformation[] serverArray; // array for the server info
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(dataPath,FileMode.Open);
                serverArray = bf.Deserialize(fs) as ServerInformation[]; // loads the array

                data = serverArray.ToList(); // converts the serverinfo array into a list
                fs.Close();
            }     
            return data;       
        }
    }
}
