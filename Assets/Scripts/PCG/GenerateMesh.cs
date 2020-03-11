using UnityEngine;
using System.Collections.Generic;

public class GenerateMesh : MonoBehaviour
{

	ColorIndexer colorIndex;
	Vector3[] vectorMap;
	int[] triangleMap;
	public Mesh meshMap;
	Vector2[] uvs;
	

	public Mesh MakeMesh(int x, int z, float[,] heightMap, Texture texture, AnimationCurve animCurve, float amplifier)
	{

		

		vectorMap = new Vector3[x * z];
		triangleMap = new int[ 6 * (x-1) * (z-1) ];
		uvs = new Vector2[x * z];
		
		colorIndex = gameObject.GetComponent<ColorIndexer>();

		int index = 0;
		for(int i = 0; i < x; i++){
			for(int j = 0; j < z; j++){

				vectorMap[i * x + j] = new Vector3(i, animCurve.Evaluate(heightMap[i, j]) * amplifier, j);
				uvs[i * x + j] = new Vector2((float)j / z, (float)i /x);

				if(i != 0 && j != 0){
					triangleMap[index + 0] = i * x + j - 1;
					triangleMap[index + 1] = i * x + j - x - 1;
					triangleMap[index + 2] = i * x + j - x;
					triangleMap[index + 3] = i * x + j - x;
					triangleMap[index + 4] = i * x + j;
					triangleMap[index + 5] = i * x + j - 1;
					index += 6;
				}

				

			}
		}

		


		meshMap.vertices = vectorMap;
		meshMap.triangles = triangleMap;
		meshMap.uv = uvs;
		meshMap.RecalculateNormals();


		return meshMap;
		
	}

}
 