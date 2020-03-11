using UnityEngine;
using System;
using System.Collections.Generic;

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
    public GameObject waterPrefab;
    public GameObject ground;
    public Material terrainMaterial;

    Texture2D texture;
    Color[] colors;

    List<List<Vector2>> waterList;
    List<GameObject> puddleList = new List<GameObject>();

    GenerateMesh generator;
    ColorIndexer colorIndex;

    private void Awake() 
    {
        
        UpdateMap();
    }

    private void Start()
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
        terrainMaterial.SetTexture("_MainTex", texture);
        texture.SetPixels(colors);
        texture.Apply();
    }


    public void GenerateMesh(){
        
        generator = gameObject.GetComponent<GenerateMesh>();
        Mesh mesh = generator.MakeMesh(resolution, resolution, heightMap, texture,animCurve, amplifier);

        ground.GetComponent<MeshCollider>().sharedMesh = mesh;
        ground.GetComponent<MeshFilter>().sharedMesh = mesh;
        ground.GetComponent<MeshRenderer>().material = terrainMaterial;

        WaterGenerator waterGen = new WaterGenerator();
        /*waterList = waterGen.GenerateWater(resolution, resolution, heightMap, 0.25f);

        int puddlesNow = 0;
        int puddlesBefore = puddleList.Count - 1;

        foreach(List<Vector2> cluster in waterList)
        {
            if(puddlesNow <= puddlesBefore)
            {
                AddWater(cluster, puddleList[puddlesNow]);

            }else{

                puddleList.Add(Instantiate(waterPrefab));
                AddWater(cluster, puddleList[puddlesNow]);

            }
            puddlesNow++;
        }

        for(int i = puddlesBefore; i > puddlesNow; i--)
        {
            DestroyImmediate(puddleList[i]);
            puddleList.RemoveAt(i);
        }
        */

    }


    public void AddWater(List<Vector2> cluster, GameObject puddle)
    {
        float maxX = float.MinValue;
        float maxZ = float.MinValue;

        float minX = float.MaxValue;
        float minZ = float.MaxValue;

        foreach(Vector2 vec in cluster) {
            if(vec.x > maxX) {
                maxX = vec.x;
            } else if(vec.x < minX) {
                minX = vec.x;
            }

            if(vec.y > maxZ) {
                maxZ = vec.y;
            } else if(vec.y < minZ) {
                minZ = vec.y;
            }
        }

        float meanX = (maxX + minX) / 2.0f;
        float meanZ = (maxZ + minZ) / 2.0f;
        float maxR = float.MinValue;

        foreach(Vector2 vec in cluster) {
            if( Mathf.Sqrt( (vec.x - meanX)* (vec.x - meanX)  + (vec.y - meanZ) * (vec.y - meanZ) ) > maxR ) {
                maxR = Mathf.Sqrt((vec.x - meanX) * (vec.x - meanX) + (vec.y - meanZ) * (vec.y - meanZ));
            }
        }

        
        puddle.transform.position = new Vector3(meanX, amplifier*animCurve.Evaluate(0.25f), meanZ);
        puddle.transform.localScale =  new Vector3(maxR, 1f, maxR);

    }

    public void GenerateMap(){
        


        Material mat = new Material(Shader.Find("Unlit/Texture"));
        mat.SetFloat("_Glossiness", 0f);
        

        mat.SetTexture("_MainTex", texture);

        testObject.GetComponent<MeshRenderer>().material = mat;
    }

}
