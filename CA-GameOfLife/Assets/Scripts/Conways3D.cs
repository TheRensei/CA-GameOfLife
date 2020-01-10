using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conways3D : MonoBehaviour
{
    public int gridSizeX, gridSizeY, gridSizeZ;


    public Cell3D[,,] cells = new Cell3D[50, 50, 50];
    public int[,,] states = new int[50, 50, 50];

    [Space]
    public GameObject cube;
    public Material tile;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid(gridSizeX, gridSizeX, gridSizeZ);

        RandomizeGrid();

        InvokeRepeating("Upd", 0.1f, 0.3f);
    }

    ///<summary>
    /// Update the state of each cell on a grid
    ///</summary>
    void Upd()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeX; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    int state = cells[x, y, z].state;
                    int result = state;

                    int count = GetLivingNeighbours(x, y, z);

                    if (state == 1 && count < 5)
                    {
                        result = 0;
                    }
                    if (state == 1 && (count == 5 || count == 7))
                    {
                        result = 1;
                    }
                    if (state == 1 && count > 6)
                    {
                        result = 0;
                    }
                    if (state == 0 && count == 6)
                    {
                        result = 1;
                    }

                    //CREATE AN ARRAY AND COPY OVER THE WHOLE THING AND THEN APPLY THE RESULTS HERE
                    states[x, y, z] = result;
                }
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeX; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    int state = states[x, y, z];
                    cells[x, y, z].SetState(state);
                }
            }
        }
    }

    ///<summary>
    /// Randomly fill the grid by changing the state of cells
    ///</summary>
    void RandomizeGrid()
    {
        for (int x = 3; x < gridSizeX-3; x++)
        {
            for (int y = 3; y < gridSizeY-3; y++)
            {
                for (int z = 3; z < gridSizeZ-3; z++)
                {
                    int state = Random.Range(0,2);
                    cells[x, y, z].SetState(state);
                }
            }
        }
    }

    ///<summary>
    /// Create grid of given size and fill it with cell objects
    ///</summary>
    /// <param name="width">Width of the grid</param>
    /// <param name="height">Height of the grid</param>
    /// <param name="depth">Depth of the grid</param>
    void CreateGrid(int width, int height, int depth)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    GameObject temp = Instantiate(cube);
                    temp.name = "x: " + x + " y: " + y + " z: " + z;
                    temp.AddComponent<Cell3D>().CreateCell(new int[] { x, y, z}, tile);
                    cells[x, y, z] = temp.GetComponent<Cell3D>();
                }
            }
        }
    }

    ///<summary>
    /// Returns the count of the neighbours that are alive around the current cell
    ///</summary>	
    /// <param name="x">x coordinate of the current cell</param>
    /// <param name="y">y coordinate of the current cell</param>
    /// <param name="z">z coordinate of the current cell</param>
    int GetLivingNeighbours(int x, int y, int z)
    {
        int count = 0;

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                for (int k = -1; k < 2; k++)
                {
                    //The way of wrapping the grid on X and Y
                    //Based on The Coding Train video about the CA
                    int col = (x + i + gridSizeX) % gridSizeX;
                    int row = (y + j + gridSizeY) % gridSizeY;
                    int rd = (z + k + gridSizeZ) % gridSizeZ;

                    //If the cell is alive add 1, if it's dead the state is 0 so nothing happens
                    count += cells[col, row, rd].state;
                }
            }
        }

        //Remove the current cell from the count
        count -= cells[x, y, z].state;

       return count;
    }
}