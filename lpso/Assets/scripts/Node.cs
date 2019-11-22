using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node{
    public List<Node> neighbors;
    public int x;
    public int y;
    public Node()
    {
        neighbors = new List<Node>();
    }
    public float DistanceTo(Node n)
    {
        return Vector2.Distance(new Vector2(x, y), new Vector2(n.x, n.y));
    }

}
