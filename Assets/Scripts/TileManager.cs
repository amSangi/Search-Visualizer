using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileManager : MonoBehaviour {

    // Constants
    private const float Height = Constants.Height; 
    private const float Width = Constants.Width; 
    private const float TileSize = Constants.TileSize; 

    // Prefabs
    public GameObject bgTile; 
    public GameObject wallTile; 
    public GameObject cursorTile; 
	public GameObject startTile; 
	public GameObject endTile; 

    private Transform cursor; 
    private Transform bgTiles; 
    private Transform wallTiles;

    // Input Handler Members
    private bool addDrag = false; 
    private bool removeDrag = false; 
    private Transform selectedNode = null; 

	#region Constraints
	///  Constraints: 
	/// 	- Wall tiles can only be placed on bg tiles
	/// 	- Wall tiles can be removed and replaced with bg tiles 
	/// 	- Start/End tiles can be moved anywhere except for ontop of eachother 
	/// 	- No tiles can ever overlap (i.e. no tiles ever have the same x/y coordinates)
	#endregion

    #region Event Functions
    private void Awake()
    {
        bgTiles = new GameObject("BG_Tiles").transform; 
        wallTiles = new GameObject("Wall_Tiles").transform; 
        InitBG ();
        InitCursor(); 
		InitStartEnd ();
    }
		

    private void Update()
    {
        Collider2D c = GetColliderAtMP(); 
        if (!c) return; 

        int layer = c.gameObject.layer; 

		LeftClickHandler (c, layer); 
		RightClickHandler (c, layer); 
        MoveHandler(c, layer); 
    }

    #endregion

	#region Handlers
    private void LeftClickHandler(Collider2D c, int layer)
    {
		// Left click pressed
		if (Input.GetMouseButtonDown(0) || addDrag)
		{
            // Handle Wall tile placement
			if (layer == bgTile.layer) 
			{
				SwapTiles (wallTile, wallTiles, c); 
				addDrag = true; 
                removeDrag = false; 
			}

            if (layer == wallTile.layer) { addDrag = true; }

            // Handle Node Selection
            if (!addDrag && (layer == startTile.layer || layer == endTile.layer)) { selectedNode = c.transform; }
		}


		// Left click released
		if (Input.GetMouseButtonUp(0))
		{
            // Handle node placement
            bool validPos = layer != startTile.layer && layer != endTile.layer; 
            if (selectedNode && validPos) { PlaceSelectNode(c, layer); }
			selectedNode = null; 
			addDrag = false; 
		}
    }

    private void RightClickHandler(Collider2D c, int layer)
    {
		// Right click pressed
		if (removeDrag || Input.GetMouseButtonDown(1))
		{
            if (layer == wallTile.layer) { SwapTiles(bgTile, bgTiles, c); }
            removeDrag = true;
            addDrag = false;  
            selectedNode = null; 
		}

		// Right click released
		if (Input.GetMouseButtonUp(1)) { removeDrag = false; }
    }

    private void MoveHandler(Collider2D c, int layer)
    {

		// Move Selected Node 
		if (selectedNode) { PlaceSelectNode (c, layer); }
        
		// Move Cursor
		cursor.position = c.transform.position; 

    }

	private void PlaceSelectNode(Collider2D c, int layer)
	{
        if (layer == startTile.layer || layer == endTile.layer) return; 

		Collider2D selectedCol = selectedNode.GetComponent<Collider2D> (); 
		selectedCol.enabled = false; 

		// Place bgTile at selectedNode position
		PlaceTile (bgTile, bgTiles, selectedNode.position); 

		// Move Select Node
		selectedNode.transform.position = c.transform.position; 

		// Delete c tile
		Destroy (c.gameObject); 
		selectedCol.enabled = true; 

		// Update/Load Neighbours
		Node n = selectedNode.gameObject.GetComponent<Node> (); 
		n.LoadNeighbours (); 
		n.UpdateNeighbours (); 
		
	}
	#endregion

    #region Initializations

    private void InitBG() 
    {

        for (int i = 0; i < Width; ++i)
        {
            for (int j = 0; j < Height; ++j)
            {
                Instantiate(bgTile, new Vector2(i*TileSize, j*TileSize), Quaternion.identity, bgTiles);  
            }
        }

        foreach(Transform t in bgTiles)
        {
            Node n = t.GetComponent<Node> (); 
            n.LoadNeighbours (); 
        }
    }

    private void InitCursor()
    {
		cursor = (Instantiate(cursorTile, new Vector2(1f, 1f) * TileSize, Quaternion.identity) as GameObject).transform; 
		cursor.gameObject.name = "Cursor"; 

        Collider2D c = GetColliderAtMP(); 

        if (c && c.gameObject.layer == bgTile.layer) { cursor.position = c.transform.position; }

    }

	private void InitStartEnd()
	{
		Vector2 centreMap = new Vector2 (Width / 2f * TileSize, Height / 2f * TileSize); 
        Vector2 initDist = new Vector2 (3f * TileSize, 0f); 

        Collider2D c = Physics2D.OverlapCircle(centreMap - initDist, TileSize/2f); 
        Collider2D c2 = Physics2D.OverlapCircle(centreMap + initDist, TileSize/2f);       

        SwapTiles(startTile, null, c); 
        SwapTiles(endTile, null, c2); 
        GameObject.FindGameObjectWithTag("StartNode").name = "Start"; 
        GameObject.FindGameObjectWithTag("EndNode").name = "End"; 
	}

    #endregion

    #region Helpers
    // Return the collider at current mouse position
    private Collider2D GetColliderAtMP()
    {
        Vector2 p = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        return Physics2D.OverlapPoint(p); 
    }

    // Swap old tile with new tile and update/load the neighbours
    private void SwapTiles(GameObject newTile, Transform parent, Collider2D oldTileCollider)
    {
        oldTileCollider.enabled = false; 
        Node n = oldTileCollider.gameObject.GetComponent<Node>(); 

        GameObject go = Instantiate(newTile, oldTileCollider.transform.position, Quaternion.identity, parent) as GameObject; 
        
        go.GetComponent<Node>().Neighbours = n.Neighbours; 
        n.UpdateNeighbours(); 
        Destroy(oldTileCollider.gameObject); 
    }

	// Assume: Pos does not contian a tile 
	// Place given tile at given pos and load/update neighbours
	private void PlaceTile(GameObject tile, Transform parent, Vector2 pos)
	{
		GameObject go = Instantiate (tile, pos, Quaternion.identity, parent) as GameObject; 
		Node n = go.GetComponent<Node> (); 
		n.LoadNeighbours (); 
		n.UpdateNeighbours (); 
	}

    #endregion 

}
