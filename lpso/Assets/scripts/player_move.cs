using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class player_move : MonoBehaviour
{
    public Vector3 newpos;
    public string currentdirection;
    private Dictionary<string, int> angleanim;

    public GameObject playerchar;
    public Animator animator;
    public bool busy = false;

    public Camera camera;
    public bool rdy = false;
    public Transform canvas;
    

    void Update() // moves player model to location
    {
        if (rdy)
        {
            if (playerchar.transform.position != newpos)
            {
                playerchar.transform.position = Vector3.MoveTowards(playerchar.transform.position, newpos, Time.deltaTime * 5);
                SetAnimState(0, 1);
            }
            else SetAnimState(1, 0); 
            
        }
    }

    private void LateUpdate()
    {
        InterpolateCamera();
    }

    void SetAnimState(float iw, float ww) // animation wait for player
    {
       animator.SetLayerWeight(1, iw);
       animator.SetLayerWeight(2, ww);
       
    }

    //
    //

    void InterpolateCamera() // interpolates camera
    {
        float smoothtime = 0.075f;
        Vector3 velocity = Vector3.zero;
        Vector3 targetpos = playerchar.transform.position - new Vector3(0, 0, 10);
        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, targetpos, ref velocity, Time.deltaTime*2);
    }


    public void setcharangledict() // initialises angle dictionary
    {
        angleanim = new Dictionary<string, int>();
        angleanim["f"] = 0;
        angleanim["fL"] = 1;
        angleanim["L"] = 2;
        angleanim["bL"] = 3;
        angleanim["b"] = 4;
    }

    public void SetCharacterAngle() // sets the character angle and its animations
    {
        bool mirror = false;
        string neededasset = currentdirection;
        Transform newmodelpers = null;

        if (currentdirection.Contains("R") == true)
        {
            neededasset = neededasset.Replace("R", "L");
            mirror = true;
        }

        foreach (Transform v in GetChildren(playerchar.transform))
        {
            if (v.gameObject.name == neededasset) newmodelpers = playerchar.transform.Find(neededasset);
        }

        if (newmodelpers != null)
        {
            animator.SetInteger("Idle_Pose", angleanim[neededasset]);

            if (mirror == true) newmodelpers.localScale = new Vector3(-0.7f, 0.7f, 0);
            else newmodelpers.localScale = new Vector3(0.7f, 0.7f, 0);
        }
    }


    // other functions


    List<Transform> GetChildren(Transform transform) // gets all children of a transform
    {
        int nbofchildren = transform.childCount;
        List<Transform> children = new List<Transform>();

        for (int i = 0; i < (nbofchildren); i++)
        {
            children.Add(transform.GetChild(i));
        }
        return children;
    }

}
