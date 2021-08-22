using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

namespace Game.Networking
{
    public class ServerDataPersistence
    {
        public static void SaveServerData(List<ServerInformation>  serverInfo) // Serializes server info into a binary file
        {
            try
            {
                string dataPath = Application.persistentDataPath+"/serverInformation.sav";
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(dataPath,FileMode.Create);
                bf.Serialize(fs,serverInfo.ToArray());
                fs.Close();
            } 
            catch( System.Exception ex )
            {
                Debug.LogException(ex);
            }
        }

        public static void SaveOneServer(ServerInformation serverInfo)
        {
            try
            {
                List<ServerInformation> serverList = LoadServerData();
                int serverMatch = serverList.FindIndex((data) => serverInfo.IP == data.IP);
                if (serverMatch!=-1)
                {
                    serverList[serverMatch] = serverInfo;
                    SaveServerData(serverList);
                }
            }            
            catch( System.Exception ex )
            {
                Debug.LogException(ex);
            }
            
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
