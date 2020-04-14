using UnityEngine;
using System.Collections.Generic;


public class WaterGenerator
{


	public List<List<Vector2>> GenerateWater(int x, int z, float[,] heightMap, float waterHeight)
	{
		bool[,] waterMap;
		List<List<Vector2>> clusterList;
		bool[,] visited;
		waterMap = new bool[x, z];
		clusterList = new List<List<Vector2>>();
		visited = new bool[x, z];

		for (int i = 0; i < x; i++)
		{
			for (int j = 0; j < z; j++)
			{
				if (heightMap[i, j] > waterHeight)
				{
					waterMap[i, j] = false;
				}
				else
				{
					waterMap[i, j] = true;
				}
			}
		}

		clusterList = countIslands(waterMap, x, z);


		return clusterList;

	}

	private bool isSafe(bool[,] M, int row,
					   int col, bool[,] visited, int xSize, int ySize)
	{
		// row number is in range, 
		// column number is in range 
		// and value is 1 and not 
		// yet visited 
		return (row >= 0) && (row < xSize) && (col >= 0) && (col < ySize) && (M[row, col] && !visited[row, col]);
	}

	// A utility function to do 
	// DFS for a 2D boolean matrix. 
	// It only considers the 8 
	// neighbors as adjacent vertices 
	private void DFS(bool[,] M, int row,
					int col, bool[,] visited, int xSize, int ySize, List<Vector2> clusterList)
	{
		// These arrays are used to 
		// get row and column numbers 
		// of 8 neighbors of a given cell 
		int[] rowNbr = new int[] { -1, -1, -1, 0,
								   0, 1, 1, 1 };
		int[] colNbr = new int[] { -1, 0, 1, -1,
								   1, -1, 0, 1 };

		// Mark this cell 
		// as visited 
		visited[row, col] = true;
		clusterList.Add(new Vector2(row, col));
		// Recur for all 
		// connected neighbours 
		for (int k = 0; k < 8; ++k)
		{
			if (isSafe(M, row + rowNbr[k], col + colNbr[k], visited, xSize, ySize))
			{
				DFS(M, row + rowNbr[k],
					col + colNbr[k], visited, xSize, ySize, clusterList);
			}
		}
	}

	// The main function that 
	// returns count of islands 
	// in a given boolean 2D matrix 
	private List<List<Vector2>> countIslands(bool[,] M, int xSize, int ySize)
	{
		// Make a bool array to 
		// mark visited cells. 
		// Initially all cells 
		// are unvisited 
		bool[,] visited = new bool[xSize, ySize];

		// Initialize count as 0 and 
		// travese through the all 
		// cells of given matrix 

		List<List<Vector2>> clusterList1 = new List<List<Vector2>>();
		for (int i = 0; i < xSize; ++i)
		{
			for (int j = 0; j < ySize; ++j)
			{
				if (M[i, j] && !visited[i, j])
				{
					List<Vector2> clusterList = new List<Vector2>();
					// If a cell with value 1 is not 
					// visited yet, then new island 
					// found, Visit all cells in this 
					// island and increment island count 
					DFS(M, i, j, visited, xSize, ySize, clusterList);
					// I Remove smaller clusters
					if (clusterList.Count > 50)
						clusterList1.Add(clusterList);

				}
			}

		}
		/*
        int min = 10000;
        foreach (List<Vector2> list in clusterList1)
        {
            if (list.Count < min)
                min = list.Count;
        }
        Debug.Log(min);
        */
		return clusterList1;
	}

}
