using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class WaterPuddle : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
