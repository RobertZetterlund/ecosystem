using UnityEngine;
using System;

public static class PerlinNoise
{


    public static float[,] GeneratePerlinNoise(int side, int resolution, int octaves, float lacunarity, float persistence, int seed)
    {
        return PerlinNoise.GeneratePerlinNoise(side, side, resolution, octaves, lacunarity, persistence, seed);
    }

    public static float[,] GeneratePerlinNoise(int x, int z, int resolution, int octaves, float lacunarity, float persistence, int seed)
    {    
        float xStep = (float)(x - 1) / (float)resolution;
        float zStep = (float)(z - 1) / (float)resolution;
        float[,] noiseMap = new float[resolution, resolution];
        System.Random rand = new System.Random(seed);
        float[] randOffsetX = new float[octaves];
        float[] randOffsetZ = new float[octaves];

        for(int oct = 0; oct < octaves; oct++)
        {
            randOffsetX[oct] = rand.Next(-10000, 10000);
            randOffsetZ[oct] = rand.Next(-10000, 10000);
        }

        float amplitude = 1f;
        float frequency = 1f;

        float maxValue = 0.5f;
        float minValue = 0.5f;

        for(int oct = 0; oct < octaves; oct++){
            
            for(int i = 0; i < resolution; i++){
                for(int j = 0; j < resolution; j++)
                {
                    noiseMap[i, j] += amplitude*(2*Mathf.PerlinNoise(i*xStep*frequency + randOffsetX[oct], j*zStep*frequency + randOffsetZ[oct]) - 1f);
                    if(noiseMap[i, j] > maxValue)
                    {
                        maxValue = noiseMap[i, j];
                    }else if(noiseMap[i, j] < minValue)
                    {
                        minValue = noiseMap[i, j];
                    }
                }
            }
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        float k = 1 / (maxValue - minValue);
        float m = 1 - maxValue * k;
        for(int i = 0; i < resolution; i++) {
            for(int j = 0; j < resolution; j++) {
                noiseMap[i, j] = k * noiseMap[i, j] + m;
            }
        }
        

        return noiseMap;
    }
}
