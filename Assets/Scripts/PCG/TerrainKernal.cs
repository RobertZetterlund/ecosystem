using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp.Interfaces;
using DelaunatorSharp.Models;

public class TerrainKernal : MonoBehaviour
{

    
    public int side;
    public int resolution;
    [Range(1, 10)]
    public int octaves;
    public float lacunarity;
    [Range(0f, 1f)]
    public float persistance;
    public GameObject previewPlane;
    float[,] heightMap;
    public bool autoUpdate;
    public float amplifier;
    public AnimationCurve animCurve;
    public int seed;
    public GameObject waterPrefab;
    public GameObject ground;
    public Material terrainMaterial;
    public Material waterTempMaterial;

    Texture2D texture;
    Color[] colors;

    List<List<Vector2>> waterList;
    [SerializeField]
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
        waterList = waterGen.GenerateWater(resolution, resolution, heightMap, 0.25f);
        if (puddleList.Count != 0)
        {
            foreach (GameObject obj in puddleList)
            {
                
                DestroyImmediate(obj);
            }
            puddleList = new List<GameObject>();
        }

        foreach (List<Vector2> cluster in waterList)
        {
            GameObject obj = new GameObject();
            puddleList.Add(obj);
            AddWater(cluster, obj);

         
        }

        


    }

    public float[,] GetHeightMap()
    {
        return heightMap;
    }

    /*
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

    }*/

    public void AddWater(List<Vector2> lVertices2D, GameObject newObject)
    {
       

        lVertices2D.OrderBy(p => p.y).ThenBy(p => p.x);
            

        Vector2[] vertices2D = lVertices2D.ToArray();
        List<IPoint> points = new List<IPoint>();

        foreach (Vector2 vec in vertices2D)
        {
            points.Add(new Point(vec.x, vec.y));
        }
        DelaunatorSharp.Delaunator del = new DelaunatorSharp.Delaunator(points);


        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, amplifier * animCurve.Evaluate(0.25f), vertices2D[i].y);
        }

        int[] indices = del.Triangles;
        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;


        newObject.AddComponent(typeof(MeshRenderer));
        var rend = newObject.GetComponent<MeshRenderer>();
        Material[] mat = new Material[1];
        mat[0] = waterTempMaterial;
        rend.materials = mat;
        MeshFilter filter = newObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshCollider meshCollider = newObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        meshCollider.sharedMesh = msh;
        filter.mesh = msh;

        // Add WaterPond script to object

        newObject.AddComponent(typeof(WaterPond));

        
    }
    public void GenerateMap(){
        


        Material mat = new Material(Shader.Find("Unlit/Texture"));
        mat.SetFloat("_Glossiness", 0f);
        

        mat.SetTexture("_MainTex", texture);

        previewPlane.GetComponent<MeshRenderer>().material = mat;
    }

    private void OnApplicationQuit()
    {
        Debug.Log("hej");
        foreach (GameObject obj in puddleList)
        {

            DestroyImmediate(obj);
            puddleList = new List<GameObject>();
        }
    }

}
