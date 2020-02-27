using UnityEngine;
using System;


public class TerrainKernal : MonoBehaviour
{

    
    public int side;
    public int resolution;
    [Range(1, 10)]
    public int octaves;
    public float lacunarity;
    [Range(0f, 1f)]
    public float persistance;
    public GameObject testObject;
    float[,] heightMap;
    public bool autoUpdate;
    public float amplifier;
    public AnimationCurve animCurve;
    public int seed;

    Texture2D texture;
    Color[] colors;
    
    GenerateMesh generator;
    ColorIndexer colorIndex;

    private void Awake() 
    {
        
        UpdateMap();
    }

    public void UpdateMap()
    {

        if(side > 300){side = 300;}else if(side <= 0){side = 1;}
        if(octaves > 10){octaves = 10;}else if(octaves <= 0){octaves = 1;}
        if(resolution > 256){resolution = 256;}else if(resolution < 1){resolution = 1;}
        if(lacunarity > 30) { lacunarity = 30; } else if(lacunarity < 1) { lacunarity = 1; }
        

        heightMap = PerlinNoise.GeneratePerlinNoise(side, side, resolution, octaves, lacunarity, persistance, seed);
        colorIndex = gameObject.GetComponent<ColorIndexer>();
        colors = new Color[resolution*resolution];
        texture = new Texture2D(resolution, resolution);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        for(int i = 0; i < resolution; i++) {
            for(int j = 0; j < resolution; j++) {

                colors[i * resolution + j] = colorIndex.GetColor(heightMap[i, j]);

            }
        }

        texture.SetPixels(colors);
        texture.Apply();
    }


    public void GenerateMesh(){
        
        generator = gameObject.GetComponent<GenerateMesh>();
        generator.MakeMesh(resolution, resolution, heightMap, texture,animCurve, amplifier);
        

    }

    public void GenerateMap(){
        


        Material mat = new Material(Shader.Find("Unlit/Texture"));
        mat.SetFloat("_Glossiness", 0f);
        

        mat.SetTexture("_MainTex", texture);

        testObject.GetComponent<MeshRenderer>().material = mat;
    }

}
