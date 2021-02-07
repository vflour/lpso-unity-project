using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNode : MonoBehaviour
{
    public List<TileNode> neighbors;
    public int x;
    public int y;
    public TileNode()
    {
        neighbors = new List<TileNode>();
    }
    // Static function that figures out the distance between two nodes
    public static int DistanceTo(TileNode start, TileNode goal)
    {
        return (int)Vector2.Distance(new Vector2(start.x, start.y), new Vector2(goal.x, goal.y));
    }
}

