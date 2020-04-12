using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class WaterPuddle : MonoBehaviour
{

    List<Tuple<Vector3, bool>> spotList = new List<Tuple<Vector3, bool>>();
    // Use this for initialization
    void Start()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        List<Tuple<Vector3, Vector3>> tmp = GetAllOutliningEdges(mesh.triangles, mesh.vertices);
        
        List<Vector3> added = new List<Vector3>();
        foreach (Tuple<Vector3, Vector3> tup in tmp) {
            if (!added.Contains(tup.Item1))
            {
                spotList.Add(new Tuple<Vector3, bool>(tup.Item1, false));
                added.Add(tup.Item1);
            }
            if (!added.Contains(tup.Item2))
            {
                spotList.Add(new Tuple<Vector3, bool>(tup.Item2, false));
                added.Add(tup.Item2);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector3 getAvailablePos(Vector3 pos)
    {
        Vector3 result = pos;
        float minDistanceSqr = Mathf.Infinity;
        int index = 0;
        foreach(Tuple<Vector3, bool> tup in spotList.ToList())
        {
            if(tup.Item2 == false)
            {
                Vector3 diff = pos - tup.Item1;
                float distSqr = diff.sqrMagnitude;
                if (distSqr < minDistanceSqr)
                {
                    minDistanceSqr = distSqr;
                    result = tup.Item1;
                    index = spotList.IndexOf(tup);
                }
            }
        }

        if (spotList.Contains(new Tuple<Vector3, bool>(result, true))) {
            getAvailablePos(pos);
        }
        else
        {
            spotList[index] = new Tuple<Vector3, bool>(result, true);
        }
        return result;
    }
    public void clearSpot(Vector3 pos)
    {
        foreach(Tuple<Vector3, bool> tup in spotList.ToList())
        {
            if(tup.Item1 == pos && tup.Item2 == true)
            {
                spotList[spotList.IndexOf(tup)] = new Tuple<Vector3, bool>(pos, false);
            }
        }
    }
    
   

    private List<Tuple<Vector3, Vector3>> GetAllOutliningEdges(int[] triangles, Vector3[] vertices)
    {
        Dictionary<int, List<Tuple<Vector3, Vector3>>> table = new Dictionary<int, List<Tuple<Vector3, Vector3>>>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Tuple<Vector3, Vector3> edge1 = hashedEdge(vertices[triangles[i]], vertices[triangles[i + 1]]);
            Tuple<Vector3, Vector3> edge2 = hashedEdge(vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
            Tuple<Vector3, Vector3> edge3 = hashedEdge(vertices[triangles[i + 2]], vertices[triangles[i]]);
            if (table.ContainsKey(edge1.GetHashCode()))
            {
                table[edge1.GetHashCode()].Add(edge1);
            }
            else
            {
                List<Tuple<Vector3, Vector3>> tmp = new List<Tuple<Vector3, Vector3>>();
                tmp.Add(edge1);
                table.Add(edge1.GetHashCode(), tmp);
            }
            if (table.ContainsKey(edge2.GetHashCode()))
            {
                table[edge2.GetHashCode()].Add(edge2);
            }
            else
            {
                List<Tuple<Vector3, Vector3>> tmp = new List<Tuple<Vector3, Vector3>>();
                tmp.Add(edge2);
                table.Add(edge2.GetHashCode(), tmp);
            }
            if (table.ContainsKey(edge3.GetHashCode()))
            {
                table[edge3.GetHashCode()].Add(edge3);
            }
            else
            {
                List<Tuple<Vector3, Vector3>> tmp = new List<Tuple<Vector3, Vector3>>();
                tmp.Add(edge3);
                table.Add(edge3.GetHashCode(), tmp);
            }


        }
        List<Tuple<Vector3, Vector3>> result = new List<Tuple<Vector3, Vector3>>();
        foreach (int key in table.Keys)
        {

            if (table[key].Count == 1)
            {

                result.Add(table[key].First());
            }
        }
        return result;
    }
    private Tuple<Vector3, Vector3> hashedEdge(Vector3 p, Vector3 q)
    {
        Tuple<Vector3, Vector3> pResult = new Tuple<Vector3, Vector3>(p, q);
        Tuple<Vector3, Vector3> qResult = new Tuple<Vector3, Vector3>(q, p);
        if (p.x == q.x)
        {
            if (p.z < q.z)
            {
                return pResult;
            }
            else
            {
                return qResult;
            }
        }
        else
        {
            if (p.x < q.x)
            {
                return pResult;
            }
            else
            {
                return qResult;
            }
        }
    }

}
