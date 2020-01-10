using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conways : MonoBehaviour
{
    public int gridSizeX, gridSizeY;
    public bool randomizeAtStart = false;
    public bool start = true;
    [Range(0.00001f, 3f)] public float updateRate  = 0.1f;

    [Space]
    public Sprite tile;

    private Cell[,] cells = new Cell[192, 108];
    private int[,] states = new int[192, 108];
    private int[,] rule = new int[192, 108];


    // Start is called before the first frame update
    void Start()
    {
        //Create grid and fill it with cells
        CreateGrid(gridSizeX, gridSizeY);

        if(randomizeAtStart)
        {
            RandomizeGrid();
        }


        //Should the simulation start immediately and update every 0.1 sec
        if (start)
        {
            InvokeRepeating("UpdateStates", 0.1f, updateRate);
        }
    }

    private void Update()
    {
        //Check for left click to switch the state of the tile clicked on
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f))
            {
                hit.collider.gameObject.GetComponent<Cell>().SwitchState();
            }
        }

        //Check if space pressed to pause and unpause the update
        if (Input.GetKeyDown(KeyCode.Space))
        {
            start = start ? false : true;
            if (start)
            {
                InvokeRepeating("UpdateStates", 0.1f, updateRate);
            }
            else
            {
                CancelInvoke("UpdateStates");
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //Apply the new mode per each cell
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    cells[x, y].SetMode(0);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            //Apply the new mode per each cell
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    cells[x, y].SetMode(1);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //Apply the new mode per each cell
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    cells[x, y].SetMode(2);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            //Apply the new mode per each cell
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    cells[x, y].SetMode(3);
                }
            }
        }
    }


    ///<summary>
    /// Randomly fill the grid by changing the state of cells
    ///</summary>
    void RandomizeGrid()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //0 or 1
                cells[x, y].SetState(Random.Range(0, 2), 0);
            }
        }
    }

    ///<summary>
    /// Update the state of each cell on a grid
    ///</summary>
    void UpdateStates()
    {
        //Loop through all cells in a grid
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //Get current state of the cell
                int state = cells[x, y].state;

                //Initial result of the update is the previous state of the cell
                int result = state;

                //Initial rule used is the rule that has been used previously
                int rl = cells[x, y].currentRl;

                //Get the living neighbour count of current cell
                int count = GetLivingNeighbours(x, y);

                //Check the rules and apply the correct state based on the result
                if (state == 1 && count < 2)
                {
                    result = 0;
                    rl = 0;
                }
                if (state == 1 && (count == 2 || count == 3))
                {
                    result = 1;
                    rl = 1;
                }
                if (state == 1 && count > 3)
                {
                    result = 0;
                    rl = 2;
                }
                if (state == 0 && count == 3)
                {
                    result = 1;
                    rl = 3;
                }

                //Replace the above rules with this for cave generations
                //Inspired by the Sebastian League video about cave gen
                //if (count > 4)
                //{
                //    result = 1;
                //}
                //else if (count < 4)
                //{
                //    result = 0;
                //}

                //CREATE AN ARRAY AND COPY OVER THE WHOLE THING AND THEN APPLY THE RESULTS LATER
                states[x, y] = result;
                rule[x, y] = rl;
            }
        }

        //Apply the results of rule check for every cell
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                cells[x, y].SetState(states[x, y], rule[x, y]);
            }
        }
    }

    ///<summary>
    /// Create grid of given size and fill it with cell objects
    ///</summary>
    /// <param name="width">Width of the grid</param>
    /// <param name="height">Height of the grid</param>
    void CreateGrid(int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Create new gameobject with given name
                GameObject temp = new GameObject("x: " + x + " y: " + y);
                //Add the cell component to the gameobjec
                temp.AddComponent<Cell>().CreateCell(new int[] { x, y }, tile);
                //add the reference to the cell to the array
                cells[x, y] = temp.GetComponent<Cell>();
            }
        }
    }

    ///<summary>
    /// Returns the count of the neighbours that are alive around the current cell
    ///</summary>	
    /// <param name="x">x coordinate of the current cell</param>
    /// <param name="y">y coordinate of the current cell</param>
    int GetLivingNeighbours(int x, int y)
    {
        int count = 0;

        //
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                //The way of wrapping the grid on X and Y
                //Based on The Coding Train video about the CA
                int col = (x + i + gridSizeX) % gridSizeX;
                int row = (y + j + gridSizeY) % gridSizeY;

                //If the cell is alive add 1, if it's dead the state is 0 so nothing happens
                count += cells[col, row].state;
            }
        }
        //Remove the current cell from the count
        count -= cells[x, y].state;

        return count;
    }
}

//Cool Patterns When counting neighbourd incorrectly + random values at specific areas as example
/*
//put this in GetLivingNeighbours() instead of what is already there
	int col = (x + i + gridSizeX) % gridSizeX;
	int row = (y + i + gridSizeY) % gridSizeY;

//Put either 1 or 2 in the Start() to replace the one that is already there 

//1
    for (int x = 20; x < gridSizeX-20; x++)
    {
        for (int y = 20; y < gridSizeY-20; y++)
        {
            cells[x, y].SetState(Random.Range(-1,1) >= 0 ? 1: 0);
        }
    }

//2
    for (int x = 25; x < gridSizeX; x++)
    {
        for (int y = 25; y < gridSizeY; y++)
        {
            cells[x, y].SetState(Random.Range(-1,1) >= 0 ? 1: 0);
        }
    }
	
 */
