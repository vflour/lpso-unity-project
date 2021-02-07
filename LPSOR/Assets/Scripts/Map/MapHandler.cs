using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            for (int x = 0; x < mapData.size.x; x++)
                for (int y = 0; y < mapData.size.y; y++)
                {
                    if (mapData.collisionMap[x, y]) // check if map is collidable 
                    {
                        Vector3 position = new Vector3(mapData.tileSize.x*-x,mapData.tileSize.y*-y); // tiles are generated from a down->left order
                        // instantiate the actual tile
                        GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity, mapContainer);
                        // create new tile object
                        TileNode tile = tileObject.AddComponent<TileNode>();
                        tile.x = x;
                        tile.y = y;
                        // generate the tile's neighbors
                        GenerateNeighbors(x, y, tile.neighbors);
                        tileNodes[x, y] = tile;
                    }
                }
            
        }
        // Generates the neighbors for each node
        public void GenerateNeighbors(int x, int y, List<TileNode> neighbors)
        {
            // all 8 possible directions in the grid
            int[,] directions = {{-1, 1}, {0, 1}, {1, 1}, {-1, 0}, {1, 0}, {-1, -1}, {0, -1}, {1, -1}};
            for (int i = 0; i < directions.Length; i++)
            {
                // get the coordinates for a possible neighter
                int neighborX = x + directions[i, 0];
                int neighborY = y + directions[i,1];
                if (neighborX >= 0 && neighborX < mapData.size.x && // if the coordinates exist within range
                    neighborY >= 0 && neighborY < mapData.size.y)
                    if (mapData.collisionMap[neighborX, neighborY]) // check if neighbor is collidable 
                        neighbors.Add(tileNodes[neighborX,neighborY]);
            }
        }
        #endregion
        #region Map Navigation
        public void MoveToTile(Character character, Vector2Int tile)
        {
            TileNode start = tileNodes[character.tilePosition.x, character.tilePosition.y];
            TileNode goal = tileNodes[tile.x, tile.y];
            List<TileNode> path = FindPath(start, goal);
            // whether the character should run or walk
            // tbd: study how this works
            bool walking = path.Count>5;
            // move the physical character
            StartCoroutine(NavigatePath(character, path));
            
        }

        private IEnumerator NavigatePath(Character character, List<TileNode> path)
        {
            foreach (TileNode node in path)
            {
                character.MoveTo(node.gameObject.transform.position + tileOffset);
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
            gScore.TryGetValue(n, out score);
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
        #endregion
    }
}
