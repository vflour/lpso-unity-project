using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Game;

namespace LPSOMapImporter
{
	public class JSONMapAsset : AssetPostprocessor
	{
		private static IList<string> cachedPaths = new List<string>();

		//Called after an import, detects if imported files end in .scml
		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
			string[] movedAssets, string[] movedFromAssetPaths)
		{
			var filesToProcess = new List<string>();
			// adds the file to the list of files that need to be processed
			foreach (var path in importedAssets)
			{
				if (path.EndsWith(".lpsm"))
				{
					filesToProcess.Add(path);
				}
			}

			// checks cached processes, clears the old one
			foreach (var path in cachedPaths)
			{
				//Are there any incomplete processes from the last import cycle?
				if (!filesToProcess.Contains(path))
					filesToProcess.Add(path);
			}

			cachedPaths.Clear();

			// begin processing each file if there are any to process
			if (filesToProcess.Count > 0)
			{
				ProcessFiles(filesToProcess);
			}
		}
		private static void ProcessFiles(IList<string> paths)
		{
			foreach (var path in paths)
				try
				{
					ProcessJson(Deserialize(path), path);
				}
				catch (UnityException exception)
				{
					Debug.LogWarning("Failed Map Import "+ exception.Message);
					cachedPaths.Add(path); //Failed processes will be saved and re-attempted during the next import cycle
				}
			AssetDatabase.Refresh();
			AssetDatabase.SaveAssets();
		}

		// Processes a JSON file and creates a new MapData ScriptableObject
		private static void ProcessJson(JObject jsonObject, string path)
		{
			// get both height and width
			int height = jsonObject["height"].ToObject<int>();
			int width = jsonObject["width"].ToObject<int>();
			//find the CollisionMap layer
			JToken collisionLayer = jsonObject;
			
			foreach (JToken layer in jsonObject["layers"].ToArray())
				if (layer["name"].ToString() == "CollisionMap")
					collisionLayer = layer;
			
			//find the gid of the CollisionMap tileset
			int firstGid = 0;
			foreach (JToken tileSet in jsonObject["tilesets"].ToArray())
				if (tileSet["source"].ToString() == "CollisionMap.tsx")
					firstGid = tileSet["firstgid"].ToObject<int>();
			// store all properties
			int collideId = 0;
			int noCollideId = 0;
			int spawnId = 0;
			foreach (JToken property in collisionLayer["properties"].ToArray())
			{
				int value = property["value"].ToObject<int>();
				switch (property["name"].ToString())
				{
					case "Collidable":
						collideId = value;
						break;
					case "NonCollidable":
						noCollideId = value;
						break;
					case "DefaultSpawnPoint":
						spawnId = value;
						break;
				}	
			}
			// store data for the spawn coordinates
			Vector2Int spawnCoordinates = new Vector2Int(0,0);
			// create the collision map
			bool[,] collisionMap = new bool[width,height];
			JToken[] layerData = collisionLayer["data"].ToArray();
			for(int i = 0; i < layerData.Length; i++)
			{
				// store the collision tile
				int currentId = layerData[i].ToObject<int>();
				bool collidable = currentId - firstGid != noCollideId;
				int currentX = i % width;
				int currentY = i / height;
				collisionMap[currentX, currentY] = collidable;
				
				// check if its a spawn coord
				if(currentId - firstGid == spawnId)
					spawnCoordinates = new Vector2Int(currentX,currentY);
			}
			
			// create the scritableobject
			MapData map = ScriptableObject.CreateInstance<MapData>();
			map.size = new Vector2Int(width,height);
			map.defaultSpawn = spawnCoordinates;
			map.collisionMap = collisionMap;

			string assetPath = path.Replace(".lpsm",".asset");
			AssetDatabase.CreateAsset(map,assetPath);
		}

		private static JObject Deserialize(string path)
		{
			// Opens and closes the stream reader, telling it to read the JSON file from start to finish. Returns the JObject
			using (var reader = new StreamReader(path))
			{
				return JObject.Parse(reader.ReadToEnd());
			}
		}
	}
}