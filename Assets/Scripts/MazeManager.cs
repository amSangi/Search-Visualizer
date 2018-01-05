using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class MazeManager : MonoBehaviour {

    public GameObject TileManagerGO; 
    public GameObject SearchManagerGO; 

    private SearchManager sm; 

    // UI
    public ToggleGroup searchToggles; 
    public ToggleGroup distanceToggles; 
    public ToggleGroup diagonalToggles; 

    public Button searchButton; 
    public Button resetButton; 

    public Text timeText; 
	public Text pathLengthText; 
    public Text nodesExploredText; 
    public Text solutionText; 

    public void Start()
    {
        (Instantiate(TileManagerGO) as GameObject).name = "TileManager"; 
        (Instantiate(SearchManagerGO) as GameObject).name = "SearchManager"; 

        sm = SearchManagerGO.GetComponent<SearchManager>(); 
        sm.Init(); 

        searchButton.onClick.AddListener(Search); 
        resetButton.onClick.AddListener(Reset); 

        DisplayStats(0f, 0, 0, false); 
    }

    public void Search()
    {
        int search = GetToggleValue(searchToggles); 
        int heuristic = GetToggleValue(distanceToggles); 
        int diagonal = GetToggleValue(diagonalToggles); 
        bool solution = false;

        sm.Reset(); 
        sm.SetNodeCount(diagonal); 
        float ti = Time.realtimeSinceStartup; 
        switch(search)
            {
                case 0:
                    solution = sm.BFS(); 
                    break; 
                case 1: 
                    solution = sm.BBFS(); 
                    break;
                case 2:
                    solution = sm.BestFirstSearch(heuristic); 
                    break;
                case 3:
                    solution = sm.AStar(heuristic); 
                    break; 
            }

        float searchTime = (Time.realtimeSinceStartup - ti) * 1000; 
		int pathLength = sm.GetPathLength (); 
        int visitedCount = sm.GetExploredCount(); 
        DisplayStats(searchTime, pathLength, visitedCount, solution); 
    }

	private void DisplayStats(float time, int pathLength, int nodesExplored, bool soln) 
    {

        timeText.text			 	 	= "Time: " + time.ToString() + " ms"; 
		pathLengthText.text			 	= "Path Length: " + pathLength.ToString (); 
        nodesExploredText.text 		 	= "Nodes Explored: " + nodesExplored.ToString(); 
        if (soln) { solutionText.text 	= "Solution found"; }
        else { solutionText.text 		= "No solution found"; }
    }

    private int GetToggleValue(ToggleGroup tg)
    {
        IEnumerator e = tg.ActiveToggles().GetEnumerator(); 
        e.MoveNext(); 
        Toggle t = (Toggle)e.Current;

        return int.Parse(t.name); 
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

}
