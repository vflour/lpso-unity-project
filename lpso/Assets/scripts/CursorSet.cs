using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorSet : MonoBehaviour
{
    public GameObject CursorT;
    //public GameObject dialogbox;

    GameObject dialogv;

    void Start()
    {
        Cursor.visible = false;
        GameObject prefab = Resources.Load<GameObject>("mouse") as GameObject;
        CursorT = Instantiate(prefab);
        
    }

    void Update()
    {
        Vector2 cursorpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CursorT.transform.position = cursorpos;
        CursorT.transform.position-= new Vector3(0, 0, 1);
        if (dialogv != null)
        {
            Transform canvas = transform.GetComponent<player_move>().canvas;
            Vector2 pos = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle( canvas as RectTransform, Input.mousePosition, Camera.main, out pos);

            dialogv.transform.position = canvas.TransformPoint(pos) + new Vector3(1.7f, -0.7f, 0); //Camera.main.ScreenToWorldPoint(Input.mousePosition) +new Vector3(75, -42, 0);
            CursorT.GetComponent<ParticleSystem>().Play();
        }
        else CursorT.GetComponent<ParticleSystem>().Stop();
    }


    public void SetDialogVis(bool val, DialogType dialogue, GameObject dialogbox)
    {
        if (val && dialogv == null)
        {
            dialogv = (dialogue as IDialog).NewDialog(dialogbox, transform.GetComponent<player_move>().canvas);
        }
        else
        {
            Destroy(dialogv);
            dialogv = null;
        }
    }


}
