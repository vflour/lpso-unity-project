using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractItem : MonoBehaviour
{
    public string animatecharacter = "";
    public string animateself = "";
    public string msg = "";
    public Vector3 startpos;
    public TileMap tilemap;
    public bool mirrored = false;
    PropDialog dialogt;

    public Material normal;
    public Material outline;
    public string Angle;
    public GameObject objectlayer;
    public bool loops;
    public AnimationClip animclip;
    public GameObject dialogbox;
    

    public int x1;
    public int y1;
    public int x2;
    public int y2;


    GameObject copyinst;
    List<Node> cmap;
    Animator charanimate;
    Transform angle;

    private void OnMouseEnter()
    {
        if (tilemap.PlayerMove.busy) return;
        gameObject.GetComponent<SpriteRenderer>().material = outline;
        tilemap.playerchar.GetComponent<CursorSet>().SetDialogVis(true, (dialogt as DialogType), dialogbox);

        tilemap.SetMovingCursorTo(0);
    }

    private void OnMouseExit()
    {
        if (tilemap.PlayerMove.busy) return;
        gameObject.GetComponent<SpriteRenderer>().material = normal;
        tilemap.playerchar.GetComponent<CursorSet>().SetDialogVis(false, (dialogt as DialogType), dialogbox);
    }

    private void OnMouseUp()
    {
        if (tilemap.PlayerMove.busy) return;
        
        charanimate = tilemap.playerchar.GetComponent<Animator>();
        StartCoroutine(ObjInteract());
        gameObject.GetComponent<SpriteRenderer>().material = normal;
        
        tilemap.playerchar.GetComponent<CursorSet>().SetDialogVis(false, (dialogt as DialogType), dialogbox);
    }

    private void Start()
    {
        copyinst = objectlayer.transform.Find(gameObject.name).gameObject;
        dialogt = new PropDialog(msg);
    }


    IEnumerator ObjInteract()
    {
        tilemap.GeneratePathTo(x1, y1, this.gameObject);
        cmap = tilemap.currentpath;

        yield return new WaitUntil(() => tilemap.reachedpath);
        if (cmap != tilemap.currentpath) yield break;
        if(!loops) tilemap.PlayerMove.busy = true;
        tilemap.PlayerMove.newpos = startpos;
        tilemap.playerchar.transform.position = startpos;

        if (animatecharacter != "") StartCoroutine(LoadPlrAnim());
        if (animateself != "")
        {
            Animator animate = copyinst.GetComponent<Animator>();
            animate.SetTrigger(animateself);
        }
    }

    IEnumerator LoadPlrAnim()
    {
        angle= tilemap.playerchar.transform.Find(Angle);
        if (mirrored) angle.localScale = new Vector3(-0.7f, 0.7f, 0);
        else angle.localScale = new Vector3(0.7f, 0.7f, 0);

        charanimate.SetLayerWeight(1, 0);
        charanimate.SetLayerWeight(2, 0);
        charanimate.SetLayerWeight(3, 1);
        objectlayer.SetActive(true);
        SetChildrenVis(new Color(1, 1, 1, 0));
        charanimate.SetTrigger(animatecharacter);
        if (!loops)
        {
            float animlength = animclip.averageDuration;
            yield return new WaitForSeconds(animlength);
        }
        else yield return new WaitUntil(() => cmap != tilemap.currentpath);

        restoreplayerpos();

        charanimate.SetLayerWeight(1, 1);
        charanimate.SetLayerWeight(3, 0);
        

        SetChildrenVis(new Color(1, 1, 1, 1));
        objectlayer.SetActive(false);
        tilemap.PlayerMove.busy = false;
    }

    void setcharangle()
    {
        string angles = Angle;
        if (mirrored) angles = Angle.Replace("L", "R");

        tilemap.PlayerMove.currentdirection = angles;
        angle.localScale = new Vector3(0.7f, 0.7f, 0);
        tilemap.PlayerMove.SetCharacterAngle();
    }

    void restoreplayerpos()
    {
        tilemap.cX = x2;
        tilemap.cY = y2;
        Vector3 cpos = tilemap.NodeToWorld(x2, y2);
        tilemap.PlayerMove.newpos = cpos;
        tilemap.playerchar.transform.position = cpos;
        setcharangle();
    }


    void SetChildrenVis(Color val)
    {
        SpriteRenderer[] children = gameObject.GetComponentsInChildren<SpriteRenderer>();
        gameObject.GetComponent<SpriteRenderer>().color = val;

        foreach (SpriteRenderer v in children)
        {
            v.color = val;
        }
        
    }
}
