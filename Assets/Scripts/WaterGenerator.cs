using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterGenerator : MonoBehaviour
{

	bool[,] waterMap;
	List<List<Vector2>> clusterList;
	int clusterIndex;

	public void GenerateWater(int x, int z, float[,] heightMap, float waterHeight)
	{
		clusterIndex = 0;
		waterMap = new bool[x, z];
		clusterList = new List<List<Vector2>>();

		for(int i = 0; i < x; i++)
		{
			for(int j = 0; j < z; j++)
			{
				if(heightMap[i, j] > waterHeight)
				{
					waterMap[i, j] = false;
				}else
				{
					waterMap[i, j] = true;
				}
			}
		}

		for(int i = 0; i < x; i++) {
			for(int j = 0; j < z; j++) {
				
				if(waterMap[i, j])
				{
					
					FindCluster(i, j);
					clusterIndex++;
				}
				
			}
		}

	}

	private void FindCluster(int i, int j)
	{
		waterMap[i, j] = false;
		clusterList[clusterIndex].Add(new Vector2(i, j));
		if(waterMap[i + 1, j])
		{
			FindCluster(i + 1, j);
		}
		
	}
	
}
