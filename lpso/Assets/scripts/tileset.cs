using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityEngine.Tilemaps;

public class tileset : Tile {
	[SerializeField]
	private Sprite[] roadsprites;
	[SerializeField]
	private Sprite preview;


	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData){
		
		tileData.sprite = roadsprites [UnityEngine.Random.Range (0, 6)];
	}

	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Tiles/RDtile")]

	public static void CreateRanRoad(){
		string path = EditorUtility.SaveFilePanelInProject ("Save RDtile", "New RDtile", "asset", "Save rdtile", "Assets");
		if (path==""){
			return;
		}
		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<tileset>(),path);
	
	}


	#endif
}
