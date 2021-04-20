using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Map
{
    public class MapHandler : MonoBehaviour, IHandler
    {
        [Header("Map Data Properties")]
        public MapData mapData;
        public Transform mapContainer;
        [Header("Tile Data Properties")] 
        public GameObject tilePrefab;
        public Vector3 tileOffset;
        
        #region IHandler properties + methods       
        private GameSystem _system;
        public GameSystem system{get;   set;}
        public void Activate()
        {
            GenerateGraph();
            InitializeObjects();
        }
        private bool _display;
        public bool Display { get; set; }
        #endregion
        #region Map Generation
        // Generated tile nodes
        private TileNode[,] tileNodes;
        // Generates a node graph for the entire map
        public void GenerateGraph()
        {
            tileNodes = new TileNode[mapData.size.x,mapData.size.y];
            for (int i = 0; i < mapData.collisionMap.Length; i++)
            {
                if (mapData.collisionMap[i])
                {
                    int x = i % mapData.size.x;
                    int y = i / mapData.size.y;
                    float sX = mapContainer.lossyScale.x;
                    float sY = mapContainer.lossyScale.y;
                    Vector3 transX = new Vector3(mapData.tileSize.x/2, -mapData.tileSize.y/2) * x*sX; // translation for each x coordinate
                    Vector3 transY = new Vector3(-mapData.tileSize.x / 2, -mapData.tileSize.y / 2) * y*sY;
                    Vector3 position = transX + transY + new Vector3(0, mapData.yOffset,0); // tiles are generated from a down->left order
                    // instantiate the actual tile
                    GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity, mapContainer);
                    tileObject.transform.localScale = new Vector3(mapData.tileSize.x, mapData.tileSize.y, 1);
                    tileObject.name = $"Tile: {x}, {y}, {i}";

                    // create new tile object
                    TileNode tile = tileObject.GetComponent<TileNode>();
                    tile.x = x;
                    tile.y = y;
                    tileNodes[x, y] = tile;  
                }
            }
            
            // Generate neighbors for each tile once they're all instantiated
            for (int x = 0; x < tileNodes.GetLength(0); x++)
                for (int y = 0; y < tileNodes.GetLength(1); y++)
                    if(tileNodes[x,y]!=null)
                        GenerateNeighbors(x,y,tileNodes[x,y].neighbors);
        }
        // Generates the neighbors for each node
        public void GenerateNeighbors(int x, int y, List<TileNode> neighbors)
        {
            // all 8 possible directions in the grid
            int[,] directions = {{-1, 1}, {0, 1}, {1, 1}, {-1, 0}, {1, 0}, {-1, -1}, {0, -1}, {1, -1}};
            for (int i = 0; i < directions.GetLength(0); i++)
            {
                // get the coordinates for a possible neighter
                int neighborX = x + directions[i,0];
                int neighborY = y + directions[i,1];
                int neighborIndex = neighborY * mapData.size.y + neighborX;
                if (neighborX >= 0 && neighborX < mapData.size.x && // if the coordinates exist within range
                    neighborY >= 0 && neighborY < mapData.size.y)
                    if (mapData.collisionMap[neighborIndex]) // check if neighbor is collidable 
                        neighbors.Add(tileNodes[neighborX,neighborY]);
            }
        }
        #endregion
        #region Map Navigation

        // Checks if a coordinate exists. If not, sets it to the nearest spawn point.
        public Vector2Int CheckCoordinates(Vector2Int coordinates)
        {
            if(tileNodes[coordinates.x, coordinates.y] == null)
                coordinates = mapData.spawnPoints[0];
            return coordinates;
        }
        public Vector3 GetTileCoordinates(Vector2Int coordinates)
        {
            // Get last saved coordinates
            TileNode tile = tileNodes[coordinates.x, coordinates.y];
            return tile.transform.position+tileOffset;
        }
        public void MoveToTile(Character character, Vector2Int tile, Action callback)
        {
            TileNode start = tileNodes[character.tilePosition.x, character.tilePosition.y];
            tile = CheckCoordinates(tile); // Check coordinates in case it's invalid
            TileNode goal = tileNodes[tile.x, tile.y];
            List<TileNode> path = FindPath(start, goal);

            // character runs when the tile count is > 3
            bool walking = path.Count<=3;
            // move the physical character
            StartCoroutine(character.MoveToTile(path,tileOffset,callback));
        }

        public TileNode GetTile(Vector2Int tileCoords)
        {
            return tileNodes[tileCoords.x, tileCoords.y];
        }
        private IEnumerator NavigatePath(Character character, List<TileNode> path)
        {
            foreach (TileNode node in path)
            {
                character.MoveTo(node.transform.position + tileOffset);
                yield return new WaitUntil(() => character.gameObject.transform.position == character.targetPosition);
            }
            character.characterState = CharacterState.Idle;
        }
        
        // Finds the path from one node to another using the A* pathfinding algorithm
        private static List<TileNode> FindPath(TileNode start, TileNode goal)
        {
            List<TileNode> openSet = new List<TileNode>(); // All the open tiles 
            openSet.Add(start);
            List<TileNode> closedSet = new List<TileNode>(); // All closed tiles

            // Store all the nodes in a reference daisy chain so you can find the path once it's complete
            Dictionary<TileNode,TileNode> storedNodes = new Dictionary<TileNode, TileNode>();
            
            Dictionary<TileNode,int> gScore = new Dictionary<TileNode,int>(); // g score represents the distance from start
            gScore[start] = 0;
            Dictionary<TileNode,int> fScore = new Dictionary<TileNode,int>(); // f score is the sum of g score and h score,
            fScore[start] = GetHScore(start, goal);
    
            while(openSet.Count > 0)
            {
                // find the node with the lowest fscore
                TileNode current = openSet[0];
                foreach(TileNode n in openSet)
                    if(fScore[n] < fScore[current])
                        current = n;
                
                // exit if the goal is the same as current
                if(current==goal)
                    return StructurePath(current,storedNodes);
                
                //remove the current node from openSet
                openSet.Remove(current);
                closedSet.Add(current);
                
                // loops through each neighbor and tries to record and add the neighbor to the open set
                foreach(TileNode neighbor in current.neighbors)
                {
                    if(closedSet.Contains(neighbor)) // exit if the neighbor is in the closed set
                        continue;
                    
                    // project the gscore of the current tile and compare it to the neighbor's h score
                    int projectedGScore = GetGScore(current, gScore) + TileNode.DistanceTo(current, neighbor);
                    if (projectedGScore < GetGScore(neighbor, gScore)) // if the neighbor is farther away than the projected gscore OR if its undefined
                    {
                        storedNodes[neighbor] = current; // record a possible viable path for reference
                        // store the gscores and fscores
                        gScore[neighbor] = projectedGScore;
                        fScore[neighbor] = gScore[neighbor] + GetHScore(neighbor, goal);
                        if(!openSet.Contains(neighbor)) // if the neighbor isn't in the openset, add it.
                            openSet.Add(neighbor);
                    }
                }
            }
            return new List<TileNode>();
        }
        // Heuristic function for the algorithm. It's the sum of the absolute values of (X2-X1) and (Y2-Y1)
        private static int GetHScore(TileNode start, TileNode goal)
        {
            int dx = goal.x - start.x;
            int dy = goal.y - start.y;
            return Mathf.Abs(dx) + Mathf.Abs(dy);
        }
        // Finds the GScore of a node. If it doesn't exist, defaults to the maximum int value.
        private static int GetGScore(TileNode n,  Dictionary<TileNode,int> gScore) {
            int score = int.MaxValue;
            if (gScore.ContainsKey(n)) // TryGetValue is being mean, so I've put it in timeout.
                score = gScore[n];
            return score;    
        }
        // Basically creates a list by referencing the daisy-chain of nodes connected to the goal
        // Goal -> Node next to it -> Node after that... etc. It works its way backwards until it finds a path from the origin
        private static List<TileNode> StructurePath(TileNode current, Dictionary<TileNode,TileNode> storedNodes)
        {
            List<TileNode> path = new List<TileNode>();
            path.Add(current);
            // check if current will still be a reference. that means theres another node attached to it
            while (storedNodes.ContainsKey(current))
            {
                current = storedNodes[current]; // reset the current to the referenced value
                path.Add(current);
            }
            path.Reverse(); // Nodes are added from goal to start, you want to know the reverse of that
            return path;
        }
        #endregion
        #region Map Object Handling
        [Header("Prop Data Properties")] 
        public Transform backgroundObjectContainer;
        private Dictionary<int, Prop> props = new Dictionary<int, Prop>();
        public Material propOutline;
        public Material propNormal;
        public void InitializeObjects()
        {
            // set sprite order of all background objects
            foreach (SortingGroup child in backgroundObjectContainer.GetComponentsInChildren<SortingGroup>())
                child.sortingOrder = (int) -child.transform.position.y*10;
            // add props if applicable
            foreach (Prop prop in backgroundObjectContainer.GetComponentsInChildren<Prop>())
            {
                prop.mapHandler = this;
                props.Add(prop.id, prop);
            }
        }

        public void InteractObject(Player player, int id)
        {
            if (!props[id].isOccupied)
            {
                props[id].Interact(player);
            }
        }
        #endregion
    }
}
