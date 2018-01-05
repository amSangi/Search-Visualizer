using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour, IComparable<Node>
{
    // Up - North, Right - East

	// Indices: 0 - North, 1 - East, 2 - South, 3 - West 
    //          4 - North East, 5 - South East, 6 - South West, 7 - North West
	public List<Node> Neighbours; 
    public float distance { get; set; } 
    public float pathCost { get; set; }
    public Node predecessor = null; 
    public bool passable; 
   

    private void Awake()
    {
		Neighbours = new List<Node> (8); 
		for (int i = 0; i < 8; ++i) { Neighbours.Add(null); }
    }

	// Load neighbour nodes
    public void LoadNeighbours()
    {
        float tileSize = Constants.TileSize; 
        Vector2 centre = transform.position + new Vector3(tileSize/2, tileSize/2, 0f); 


        // Non-diagonal neighbours
        Collider2D n = Physics2D.OverlapPoint(new Vector2(centre.x, centre.y + tileSize)); 
        Collider2D e = Physics2D.OverlapPoint(new Vector2(centre.x + tileSize, centre.y)); 
        Collider2D s = Physics2D.OverlapPoint(new Vector2(centre.x, centre.y - tileSize)); 
        Collider2D w = Physics2D.OverlapPoint(new Vector2(centre.x - tileSize, centre.y)); 

        // Diagonal Neighbours
        Collider2D ne = Physics2D.OverlapPoint(new Vector2(centre.x + tileSize, centre.y + tileSize)); 
        Collider2D se = Physics2D.OverlapPoint(new Vector2(centre.x + tileSize, centre.y - tileSize)); 
        Collider2D sw = Physics2D.OverlapPoint(new Vector2(centre.x - tileSize, centre.y - tileSize)); 
        Collider2D nw = Physics2D.OverlapPoint(new Vector2(centre.x - tileSize, centre.y + tileSize)); 

        if (n) Neighbours[0] = n.gameObject.GetComponent<Node>();  
        if (e) Neighbours[1] = e.gameObject.GetComponent<Node>();   
        if (s) Neighbours[2] = s.gameObject.GetComponent<Node>();   
        if (w) Neighbours[3] = w.gameObject.GetComponent<Node>(); 
        if (ne) Neighbours[4] = ne.gameObject.GetComponent<Node>(); 
        if (se) Neighbours[5] = se.gameObject.GetComponent<Node>(); 
        if (sw) Neighbours[6] = sw.gameObject.GetComponent<Node>(); 
        if (nw) Neighbours[7] = nw.gameObject.GetComponent<Node>(); 

    }
		
    public void UpdateNeighbours()
    {
        for (int i = 0; i < 8; i++)
        {
            Node n = Neighbours[i]; 
            if (n) { n.LoadNeighbours(); }
        }
    }

    public int CompareTo(Node n)
    {
        if (n == null) { return -1; }

        if (distance < n.distance) { return -1; }
        else if (distance > n.distance) { return 1; }
        else { return 0; }
    }




}
