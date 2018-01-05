using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    private const float width = Constants.Width; 
    private const float height = Constants.Height; 
    private const float tileSize = Constants.TileSize; 

    // Camera Orthogonal Size Constants
    private const float maxSize = Constants.MaxSize; 
    private const float minSize = Constants.MinSize; 
    private const float initSize = Constants.InitCameraSize; 

	private const float zoomSensitivity = Constants.ZoomSensitivity; 
    private const float wheelZoomSensitivity = Constants.WheelZoomSensitivity; 
    private const float moveSensitivity = Constants.MoveSensitivity; 

    private bool zoomIn = false; 
    private bool zoomOut = false;
    private bool wheelDrag = false;  
    private Vector2 moveDir;
	private Camera c; 

    #region Event Functions

    private void Awake()
    {
		c = GetComponent<Camera> (); 
        c.orthographicSize = initSize; 
        transform.position = new Vector3(width/2 * tileSize, height/2 * tileSize, transform.position.z); 

        moveDir = new Vector2(0f, 0f); 
    }
		
    private void LateUpdate()
    {
        HandleZoom(); 
        HandleCameraMovement(); 
    }

    #endregion

    #region Input Handlers

    private void HandleZoom()
    {
        if (Input.GetKeyDown(KeyCode.Z)) { zoomIn = true; zoomOut = false; }
        if (Input.GetKeyDown(KeyCode.X)) { zoomIn = false; zoomOut = true; }
        if (Input.GetKeyUp(KeyCode.Z)) { zoomIn = false; } 
        if (Input.GetKeyUp(KeyCode.X)) { zoomOut = false; } 


        float zoom = 0f; 


        if (zoomIn) { zoom = -1f; }
        if (zoomOut) { zoom = 1f; }

        float wheelZoom = Input.GetAxis("Mouse ScrollWheel"); 
        c.orthographicSize += wheelZoomSensitivity * -wheelZoom;
        // Zooming priority given to keyboard
        if (zoom != 0) { c.orthographicSize += zoomSensitivity * zoom; } 


		float size = c.orthographicSize; 
		c.orthographicSize = Mathf.Clamp (size, minSize, maxSize); 
    }

    private void HandleCameraMovement()
    {

        HandleKeyboardInputs(); 
        MoveCamera(); 

        if (Input.GetMouseButtonDown(2)) { wheelDrag = true;  } 
        if (Input.GetMouseButtonUp(2))   { wheelDrag = false; } 
        if (wheelDrag) { MoveCameraWithDrag(); }
    }

    private void MoveCamera()
    {
       
        Vector3 pos = transform.position; 
        Vector3 newPos = new Vector3(pos.x + moveDir.x, pos.y + moveDir.y, pos.z); 
        Vector3 diff = newPos - pos; 

        transform.Translate(diff * Time.deltaTime * moveSensitivity); 
    }

    private void MoveCameraWithDrag()
    {
        Vector3 mp = c.ScreenToWorldPoint(Input.mousePosition); 

        if (!WithinBounds(mp)) { return; }

        Vector3 pos = transform.position; 
        Vector3 newPos = new Vector3(mp.x, mp.y, pos.z); 
        Vector3 diff = newPos - pos; 

        transform.Translate(diff * Time.deltaTime * moveSensitivity); 
    }

    private void HandleKeyboardInputs()
    {
        if (Input.GetKeyDown(KeyCode.W)) { moveDir += Vector2.up;    }
        if (Input.GetKeyDown(KeyCode.A)) { moveDir += Vector2.left;  }
        if (Input.GetKeyDown(KeyCode.S)) { moveDir += Vector2.down;  }
        if (Input.GetKeyDown(KeyCode.D)) { moveDir += Vector2.right; }

        if (Input.GetKeyUp(KeyCode.W))   { moveDir -= Vector2.up;    }
        if (Input.GetKeyUp(KeyCode.A))   { moveDir -= Vector2.left;  }
        if (Input.GetKeyUp(KeyCode.S))   { moveDir -= Vector2.down;  }
        if (Input.GetKeyUp(KeyCode.D))   { moveDir -= Vector2.right; }
    }
    #endregion

	#region Helpers

    private bool WithinBounds(Vector3 p)
    {
        return p.x > 0 && p.x < width * tileSize && p.y > 0 && p.y < height * tileSize; 
    }

	#endregion
}
