using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchManager : MonoBehaviour {

    public GameObject pathTile; 
    public GameObject visitedTile; 

    private Node start; 
    private Node end;
    private Transform visited;

    private int pathLength = 0; 
    private int nodeCount = 4; 
    private const float tileSize = Constants.TileSize; 
    // Frontiers: 
    private Queue<Node> bfsFrontier;
    private Queue<Node> bbfsFrontier;
    private PriorityQueue<Node> informedFrontier; 

    // Context-Preserving Accumulators 
    private List<Node> explored; 
    private List<Node> explored2;   // bidirectional 
    private List<Node> todo; 
    private List<Node> todo2;       // bidirectional

    public void Init()
    {
        start = GameObject.FindGameObjectWithTag("StartNode").GetComponent<Node>(); 
        end   = GameObject.FindGameObjectWithTag("EndNode").GetComponent<Node>(); 
        visited = new GameObject("Visited").transform; 

        bfsFrontier = new Queue<Node>(); 
        bbfsFrontier = new Queue<Node>(); 
        informedFrontier = new PriorityQueue<Node>(); 

        explored = new List<Node>(); 
        explored2 = new List<Node>(); 
        todo = new List<Node>(); 
        todo2 = new List<Node>(); 
    }

    #region Search Algorithms
    public bool BFS() 
    {
        bfsFrontier.Enqueue(start);

        while (bfsFrontier.Count != 0)
        {
            Node n = bfsFrontier.Dequeue(); 

            if (n == end)
            {
                DisplaySolutionPath(n); 
                return true; 
            }

            VisitNode(n); 
            explored.Add(n); 

            List<Node> neighbours = n.Neighbours;  
            // Generate neighbours
            for (int i = 0; i < nodeCount; ++i)
            {
                Node child = neighbours[i]; 

                if (!(child && child.passable)) continue;

                if (!(todo.Contains(child) || explored.Contains(child)))
                {

                    child.predecessor = n; 

                    if (child == end)
                    {
                        DisplaySolutionPath(child); 
                        return true; 
                    }

                    bfsFrontier.Enqueue(child); 
                    todo.Add(child); 
                }
            }
        }
        return false; 
    }

    public bool BBFS()
    {
		pathLength = -1;

        bfsFrontier.Enqueue(start); 
        bbfsFrontier.Enqueue(end); 

        while (bfsFrontier.Count != 0 && bbfsFrontier.Count != 0)
        {
            Node n = bfsFrontier.Dequeue(); 
            Node n2 = bbfsFrontier.Dequeue(); 

            if (bbfsFrontier.Contains(n))
            {
                DisplayBSolutionPath(n); 
                DisplayBSolutionPath(n2); 
            }

            if (bfsFrontier.Contains(n2))
            {
                DisplayBSolutionPath(n); 
                DisplayBSolutionPath(n2); 
            }

            VisitNode(n); 
            VisitNode(n2); 
            explored.Add(n); 
            explored2.Add(n2); 

            List<Node> neighbours = n.Neighbours;  
            // Generate neighbours
            for (int i = 0; i < nodeCount; ++i)
            {
                Node child = neighbours[i]; 

                if (!(child && child.passable)) continue;

                if (!(todo.Contains(child) || explored.Contains(child)))
                {

                    if (bbfsFrontier.Contains(child))
                    {
                        DisplayBSolutionPath(child); 
                        DisplayBSolutionPath(n); 
                        return true; 
                    }

                    child.predecessor = n;     
                    bfsFrontier.Enqueue(child); 
                    todo.Add(child); 
                }
            }

            neighbours = n2.Neighbours;  
            // Generate neighbours
            for (int i = 0; i < nodeCount; ++i)
            {
                Node child = neighbours[i]; 

                if (!(child && child.passable)) continue;

                if (!(todo2.Contains(child) || explored2.Contains(child)))
                {

                    if (bfsFrontier.Contains(child))
                    {
                        DisplayBSolutionPath(child); 
                        DisplayBSolutionPath(n2); 
                        return true; 
                    }
                    child.predecessor = n2; 
                    bbfsFrontier.Enqueue(child);
                    todo2.Add(child); 
                }
            }
        }
        return false; 
    }

    public bool BestFirstSearch(int h)
    {
        Heuristic distHeuristic = ManhattanDistance; 
        if (h == 1) { distHeuristic = EuclideanDistance; }

        start.distance = distHeuristic(start); 
        informedFrontier.Enqueue(start); 

        while (informedFrontier.Count() != 0)
        {
            Node n = informedFrontier.Dequeue(); 

            if (n == end)
            {
                DisplaySolutionPath(n); 
                return true; 
            }

            VisitNode(n); 
            explored.Add(n); 

            List<Node> neighbours = n.Neighbours;  
            // Generate neighbours
            for (int i = 0; i < nodeCount; ++i)
            {
                Node child = neighbours[i]; 

                if (!(child && child.passable)) continue;

                if (!(todo.Contains(child) || explored.Contains(child)))
                {

                    child.predecessor = n;
                    child.distance = distHeuristic(child); 

                    if (child == end)
                    {
                        DisplaySolutionPath(child); 
                        return true; 
                    }

                    informedFrontier.Enqueue(child); 
                    todo.Add(child); 
                }
            }
        }
        return false;  
    }

    public bool AStar(int h)
    {
        Heuristic distHeuristic = ManhattanDistance; 
        if (h == 1) { distHeuristic = EuclideanDistance; }

        start.distance = distHeuristic(start); 
        informedFrontier.Enqueue(start); 
        while (informedFrontier.Count() != 0)
        {
            Node n = informedFrontier.Dequeue(); 

            if (n == end)
            {
                DisplaySolutionPath(n); 
                return true; 
            }

            VisitNode(n); 
            explored.Add(n); 

            List<Node> neighbours = n.Neighbours;  
            // Generate neighbours
            for (int i = 0; i < nodeCount; ++i)
            {
                Node child = neighbours[i]; 

                if (!(child && child.passable)) continue;

                if (!(todo.Contains(child) || explored.Contains(child)))
                {

                    child.predecessor = n;
                    child.pathCost = n.pathCost + 1f; 
                    child.distance = distHeuristic(child) + child.pathCost; 

                    if (child == end)
                    {
                        DisplaySolutionPath(child); 
                        return true; 
                    }

                    informedFrontier.Enqueue(child); 
                    todo.Add(child); 
                }
            }
        }
        return false; 
    }

    #endregion

    #region Helpers
    public void Reset()
    {
		pathLength = 0; 
        nodeCount = 4; 


        // Frontiers
        bfsFrontier.Clear(); 
        bbfsFrontier.Clear(); 
        informedFrontier.Clear(); 

        // Accumulators
        explored.Clear(); 
        explored2.Clear(); 
        todo.Clear(); 
        todo2.Clear(); 

        // Visited/Path Tiles
        foreach (Transform t in visited) { Destroy(t.gameObject); } 
    }

    public int GetExploredCount()
    {
        return explored.Count + explored2.Count; 
    }

	public int GetPathLength()
	{
		return pathLength; 
	}

    public void SetNodeCount(int d)
    {
        nodeCount = 4 + (d*4); 
    }

    private void DisplaySolutionPath(Node n)
    {
        while (n != start)
        {
			pathLength++; 
            Instantiate(pathTile, n.transform.position, Quaternion.identity, visited); 
            n = n.predecessor; 
        }
    }

    private void DisplayBSolutionPath(Node n)
    {
        start.predecessor = null; 
        end.predecessor = null; 
        while (n != null)
        {
			pathLength++; 
            Instantiate(pathTile, n.transform.position, Quaternion.identity, visited); 
            n = n.predecessor; 
        }
    }

    private void VisitNode(Node n)
    {
        Instantiate(visitedTile, n.transform.position, Quaternion.identity, visited); 
    }

    #endregion

    #region Heuristics
    public delegate float Heuristic(Node n); 

    public float EuclideanDistance(Node n)
    {
        return Vector3.Distance(n.transform.position, end.transform.position); 
    }

    public float ManhattanDistance(Node n)
    {
        Vector2 p1 = n.transform.position; 
        Vector2 p2 = end.transform.position; 
        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y); 
    }
    #endregion
}
