using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that holds information about a certain position
// so that it can be used in a pathfinding algorithm.
public class Node
{


    // Nodes have horizontal X and vertical Y positions
    public int posX;
    public int posY;

    // G is a basic *cost* value to go form one node to another.
    public int g_cost = int.MaxValue;

    // H is the heuristic that *estimates* the cost of the closest path. 
    public int f_cost = int.MaxValue;

    // Nodes have references to other nodes
    // so that it is possible to build a path
    public Node parent = null;

    // The value of the node.
    public NavTile value = null;

    // Constructor.
    public Node(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
        
    }
}


public class AStar : MonoBehaviour
{

    // Public Variables.
    public GameObject backgroundContainer;
    public GameObject player;
    public GameObject enemy;


    // Variables.
    private int mapWidth;
    private int mapHeight;    
    private Node[,] nodeMap;
    


    // Start is called before the first frame update
    public List<Node> FindPath()
    {
        // Preload values.

        mapHeight = backgroundContainer.transform.childCount;
        mapWidth = backgroundContainer.transform.GetChild(0).childCount;

        // Parse the map.
        nodeMap = new Node[mapWidth, mapHeight]; // 9 and 11.
        Node start = null;
        Node goal = null;

        
        // Do every column.
        for (int y = 0; y < backgroundContainer.transform.childCount; y++)
        {
            // Access the sub elements of the background container. (horizontal values)
            Transform backgroundRow = backgroundContainer.transform.GetChild(y);
            
            // Do evert row within that column.
            for (int x = 0; x < backgroundRow.childCount; x++)
            {
                NavTile tile = backgroundRow.GetChild(x).GetComponent<NavTile>();
                
                Node node = new Node(x, y);
                node.value = tile;
                nodeMap[x, y] = node;
            }

        }
        start = FindNode(player);
        goal = FindNode(enemy);

        Debug.Log("Player is on" + start.posX + " " + start.posY);
        Debug.Log("Enemy is on" + goal.posX + " " + goal.posY);

        // Execute the A-Star algorithm.
        
        List<Node> nodePath = ExecuteAStar(start, goal);
        // Since the path starts from end towards beginning, we gotta reverse it.
        nodePath.Reverse();
        return nodePath;

    }

    private Node FindNode(GameObject obj)
    {
        // Circlecast to determine the tiles the invisible circle in the tank is touching

        Collider2D[] collidingObjects = Physics2D.OverlapCircleAll(obj.transform.position, 0.2f);
        foreach (Collider2D collidingObject in collidingObjects)
        {
            if (collidingObject.gameObject.GetComponent<NavTile>() != null)
            {
                // This is the tile that the object is on. 
                NavTile tile = collidingObject.gameObject.GetComponent<NavTile>();

                // Find the node which contains the tile.
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        Node node = nodeMap[x, y];
                        if (node.value == tile)
                        {
                            return node;
                        }
                    }
                }                
            }
        }
        return null;
    }

    private List<Node> ExecuteAStar(Node start, Node goal)
    {
        // This list holds potential best path nodes
        // that should be visited. Always starts with origin.
        List<Node> openList = new List<Node>() { start };

        // This list holds nodes that have already been visited.
        List<Node> closedList = new List<Node>();

        // Initialize the start node.
        // Remember f = g + h
        start.g_cost = 0;  //at the beginning node cost is always zero!
        start.f_cost = start.g_cost + CalculateHeuristicValue(start, goal);

        // Main algorithm.
        while (openList.Count > 0) {
            // First of all get the node with
            // lowest estimated cost to reach the target
            Node current = openList[0];
            foreach (Node node in openList) { 
                if (node.f_cost < current.f_cost) {
                    current = node;
                }
            }
            // Check if the target has been reached.
            if (current == goal)
            {
                return BuildPath(goal);
            }

            // Make sure that the current node
            // will never be visited again.

            openList.Remove(current);
            closedList.Add(current);

            // Execute the algorithm in the current
            // node's neighbors
            List<Node> neighbors = GetNeighbourNodes(current);
            
            foreach (Node neighbor in neighbors)
            {
                {
                    // If the neighbor has already been visited,
                    // skip it.
                    if (closedList.Contains(neighbor))
                        continue;
                    
                    // If the neighbor is not in the open list,
                    // add it.
                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);

                    // Calculate the cost to move to the neighbor.
                    // and verify if this value is better than
                    // whatever is stored in the neighbor
                    int candidateG = current.g_cost + 1;


                    // If the neighbor is already in the open list,
                    // but the cost to move to it is higher than
                    // the current cost, skip it.
                    if (candidateG >= neighbor.g_cost)
                        // there is already a better path!
                        continue;

                    // If the neighbor is not in the open list,
                    // or the cost to move to it is lower than
                    // the current cost, update it.
                    else
                    {
                        neighbor.parent = current;
                        neighbor.g_cost = candidateG;
                        neighbor.f_cost = neighbor.g_cost + CalculateHeuristicValue(neighbor, goal);
                        // f = g + h (g is basic cost, h is heuristic value)
                    }
                }
            }
        }
        
        // If reached this point, it means that
        // there are no more nodes to search.
        // The algorithm could not find a good path.
        return new List<Node>();
    }

    private List<Node> GetNeighbourNodes(Node node)
    {
        List<Node> neighbours = new List<Node>();
        
        // Verify all possible neighbours.
        // Since we can only move horizontal and vertical
        // Check these 4 possibilities
        // If a node is blocked, it cannot be visited.
        

        // Left side
        if (node.posX -1 >= 0)
        {
            // Candidate because that node might be blocked
            Node candidate = nodeMap[node.posX - 1, node.posY];
            if (candidate.value.navigable)
            {
                neighbours.Add(candidate);
            }

        }

        // Right side
        if (node.posX + 1 <= mapWidth-1)
        {
            Node candidate = nodeMap[node.posX + 1, node.posY];
            if (candidate.value.navigable)
            {
                neighbours.Add(candidate);
            }
            

        }

        // Top side
        if (node.posY - 1 >= 0)
        {
            Node candidate = nodeMap[node.posX, node.posY - 1];
            if (candidate.value.navigable)
            {
                neighbours.Add(candidate);
            }

        }

        // Bottom side
        if (node.posY + 1 <= mapHeight - 1)
        {
            Node candidate = nodeMap[node.posX, node.posY + 1];
            if (candidate.value.navigable)
            {
                neighbours.Add(candidate);
            }

        }

        return neighbours;
    }

    // A simple estimate of the distance.
    // Uses the manhattan distance.
    private int CalculateHeuristicValue(Node node1, Node node2)
    {
        return Mathf.Abs(node1.posX - node2.posX) + Mathf.Abs(node1.posY - node2.posY);
    }

    // Once all nodes are populated,
    // just read every parent and build the path.
    private List <Node> BuildPath(Node node)
    {
        List<Node> path = new List<Node>();
        while (node.parent!= null)
        {            
            node = node.parent;
            path.Add(node);
        }
        return path;
    }
        
        
    
}