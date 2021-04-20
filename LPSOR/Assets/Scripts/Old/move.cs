using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class move : MonoBehaviour {
	
	const int LEFT_MOUSE_BUTTON = 0;
	public int tileX;
	public int tileY;
    public bool collide = false;

	public TileMap maps;


	void OnMouseUp(){
        if (EventSystem.current.IsPointerOverGameObject() || !collide) return;
        maps.GeneratePathTo(tileX, tileY, this.gameObject);
	}

    private void OnMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject() || !collide)
        {
            maps.SetMovingCursorTo(0);
            return;
        }
        maps.SetMovingCursorTo(1);
    }

}
	
