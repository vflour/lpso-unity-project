using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;



public class TileMap : MonoBehaviour {

    int MapSizeX;
    int MapSizeY;
    public int cX = 0;
    public int cY = 0;
    public List<Node> currentpath = null;

    public TileTypes[] tiletypes;
    public GameObject TileItem;
    public string map_file;

    public GameObject playerchar;
    int[,] tiles;
    public bool rdy = false;

    public player_move PlayerMove;
    public Sprite[] mousetextures;

    public GameObject clicktilem;
    public bool reachedpath = false;
    public Transform canvas;

    public void StartMap() // tells map to load up the character + tiles
    {
        MapSet();
        GeneratePathFindingGraph();
        mapVisual();
        SetPlayerMoveData();
    }


    //

    public void SetMovingCursorTo(int text)
    {
        CursorSet cursorset = playerchar.GetComponent<CursorSet>();
        cursorset.CursorT.gameObject.GetComponent<SpriteRenderer>().sprite = mousetextures[text];
    }


    //

    public void SetPlayerMoveData()
    {
        PlayerMove.setcharangledict();
        PlayerMove.animator = playerchar.GetComponentInChildren<Animator>();
        PlayerMove.canvas = canvas;
    }


    void FindCharacterAngle(int oldX, int oldY, int cX, int cY)
    {
        string dix = "";
        string diy = "";

        if (cX < oldX)
        {
            dix = "L";
            diy = "b";

            if (cY < oldY)
            {
                dix = "";

            }
            else if (cY > oldY)
            {
                diy = "";
            }
        }
        else if( cX > oldX)
        {
            dix = "R";
            diy = "f";
            if (cY < oldY)
            {
                diy = "";

            }
            else if (cY > oldY)
            {
                dix = "";
            }
        }
        else
        {
            if (cY < oldY)
            {
 
                dix = "R";
                diy = "b";

            }
            else if (cY > oldY)
            {
                dix = "L";
                diy = "f";
            }
        }
        PlayerMove.currentdirection = diy + dix;
        

    }
    public Vector3 NodeToWorld(int x, int y){ // converts tile nodes into actual position
        float posx = (x * (1.55F / 2)) - (y * (1.55F / 2));
        float posy = -(x * (0.77F / 2)) - ((0.77F / 2) * y);
        Vector3 findpos = new Vector3(posx,posy,0) + new Vector3(0.15f, 0.1f, 0);

        return findpos;
    }



    IEnumerator SetDestination(List<Node> cpath){ // basically sets newpos whenever called
        int oldX = cX;
        int oldY = cY;
        reachedpath = false;
        foreach (Node v in cpath){
            
            if (currentpath != cpath)  break; // if currentpath has changed then breaks
                FindCharacterAngle(oldX, oldY, v.x, v.y);
                PlayerMove.SetCharacterAngle();


                cX = v.x;
                cY = v.y;

                oldX = cX;
                oldY = cY;

                PlayerMove.newpos = NodeToWorld(v.x, v.y);

                yield return new WaitUntil(() => playerchar.transform.position == PlayerMove.newpos);
            
        }
       
        reachedpath = true;
        
    }

    float CanWalkOnTile(int sx, int sy,int tx, int ty){ // checks walking on tile cost
        TileTypes tt = tiletypes[tiles[tx, ty]];
        float cost = 1f;
        
        if(sx!= tx && sy!= ty){
            cost += 0.001f;
        }
        if (tt.IsWalkable != true){
            cost = Mathf.Infinity;
        }
        return cost;
    }

    public void GeneratePathTo(int posX, int posY, GameObject currentunit){ // Generates Node List path using Djikstra pathfinding

        if (PlayerMove.busy == true) return;
        currentpath = null;

        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
 
        Node target = graph[cX, cY];
        Node source = graph[posX, posY];

        List<Node> unvisited = new List<Node>(); 

        dist[source] = 0;
        prev[source] = null;

        foreach(Node v in graph){
            if (v != source){
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }
            unvisited.Add(v);
        }

        while(unvisited.Count > 0){
            Node u = null;
            foreach(Node possibleU in unvisited){
                if (u == null || dist[possibleU]<dist[u]){
                    u = possibleU;
                }
            }

            unvisited.Remove(u);

            if (u == target){
                break;
            }
            foreach(Node v in u.neighbors){
                float alt = dist[u] + CanWalkOnTile(posX,posY,v.x,v.y);
                if (alt < dist[v]){
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }
      
        if (prev[target] == null){
            return;
        }
        currentpath = new List<Node>();
        Node curr = target;

        while (curr != null){
            currentpath.Add(curr);
            curr = prev[curr];
        }
        
        StartCoroutine(SetDestination(currentpath));
        StartCoroutine(ClickAnimTile(posX, posY));
    }

	void mapVisual(){ // Generates Clickable tiles in map
        for (int x = 0; x < MapSizeX; x++){
            for (int y = 0; y < MapSizeX; y++){
                int tt = tiles[x, y];
                float posx = (x * (1.55F/2)) - (y * (1.55F/2));
                float posy = -(x*(0.77F/2))-((0.77F/2)*y);
                GameObject tile_dat = Instantiate(TileItem, new Vector3(posx, posy, 0), Quaternion.identity,transform.GetChild(1));
                tile_dat.name = "tile";
                move ct = tile_dat.gameObject.AddComponent<move>();
                ct.tileX = x;
                ct.tileY = y;
                ct.maps = this;

                if (tt == 0) ct.collide = true;
            }
        }
    }

    MapData jsonmap() // loads json map from Resources
    {
        TextAsset jsonfile = Resources.Load<TextAsset>(map_file);
        string jsonstring = jsonfile.text;
        MapData mapData = JsonUtility.FromJson<MapData>(jsonstring);
        return mapData;
    }

    int tileIDtocoll(int t1, int tileid) // Sets tile id in json map to 0 or 1
    {
        if (tileid == t1) tileid = 0;
        else tileid = 1;
        return tileid;
    }

    void MapSet(){ // Sets Map data  in tiles according to JSON map
        MapData mapData = jsonmap();
        MapSizeX = mapData.SizeX;
        MapSizeY = mapData.SizeY;
        int globalindex = 0;
        tiles = new int[MapSizeX, MapSizeY];
        for (int y = 0; y < MapSizeX; y++)
        {
            for (int x = 0; x < MapSizeX; x++){
                tiles[x, y] = tileIDtocoll(mapData.NonCollidable,mapData.layers[globalindex]);
                
                globalindex++;
            }
        }

    }

    IEnumerator ClickAnimTile(int x, int y)
    {
        GameObject clickpos = Instantiate(clicktilem, NodeToWorld(x,y), Quaternion.identity);
        yield return new WaitUntil(() => reachedpath == true);
        Destroy(clickpos);
    }

    Node[,] graph;
    void GeneratePathFindingGraph() { // Generates a grid of nodes based on the tilemap
        graph = new Node[MapSizeX, MapSizeY];
        for (int x = 0; x < MapSizeX; x++)
        {
            for (int y = 0; y < MapSizeX; y++)
            {
                graph[x, y] = new Node();

                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }

        for (int x = 0; x < MapSizeX; x++){
            for (int y = 0; y < MapSizeX; y++){

                if (x > 0) { // trying left
                    graph[x, y].neighbors.Add(graph[x - 1, y]);
                    if (y > 0) {
                        graph[x, y].neighbors.Add(graph[x-1, y - 1]);
                    }
                    if (y < MapSizeY - 1){
                        graph[x, y].neighbors.Add(graph[x-1, y + 1]);
                    }
                }

                if (x < MapSizeX - 1){ // trying right
                    graph[x, y].neighbors.Add(graph[x + 1, y]);
                    if (y > 0) {
                        graph[x, y].neighbors.Add(graph[x + 1, y - 1]);
                    }
                    if (y < MapSizeY - 1){
                        graph[x, y].neighbors.Add(graph[x + 1, y + 1]);
                    }
                }

                if (y > 0){ // good ol up or down
                    graph[x, y].neighbors.Add(graph[x, y-1]);
                }
                if (y < MapSizeY - 1){
                    graph[x, y].neighbors.Add(graph[x ,y+1]);
                }
            }
        }
    }


}


