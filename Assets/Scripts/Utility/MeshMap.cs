using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMap
{

    public static Vector3[] rabbitMeshVerts;
    public static Vector3[] foxMeshVerts;
    public static Vector3[] treeMeshVerts;

    public static void LoadData(){

        GameObject entity;
        Vector3[] baseVerts;

        // The rabbit has it's rotationa and scale values on a different child than the mesh
        entity = (GameObject)Resources.Load("testR");
        baseVerts = entity.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices;
        for(int i = 0; i < baseVerts.Length; i++) {
            baseVerts[i] = entity.transform.GetChild(0).localPosition + Vector3.Scale(Quaternion.Euler(entity.transform.GetChild(0).rotation.eulerAngles) * baseVerts[i], entity.transform.GetChild(0).localScale);
        }
        rabbitMeshVerts = new Vector3[baseVerts.Length];
        baseVerts.CopyTo(rabbitMeshVerts, 0);
        

        // The fox has it's values on the child with the mesh
        entity = (GameObject)Resources.Load("testF");
        baseVerts = entity.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices;
        for(int i = 0; i < baseVerts.Length; i++)
        {
            baseVerts[i] = entity.transform.GetChild(0).localPosition + Vector3.Scale(Quaternion.Euler(entity.transform.GetChild(0).rotation.eulerAngles) * baseVerts[i], entity.transform.GetChild(0).localScale);
        }
        foxMeshVerts = new Vector3[baseVerts.Length];
        baseVerts.CopyTo(foxMeshVerts, 0);
        

        // The tree is like the fox and the sappling has the same mesh
        entity = (GameObject)Resources.Load("Tree");
        baseVerts = entity.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.vertices;
        for(int i = 0; i < baseVerts.Length; i++) {
            baseVerts[i] = entity.transform.GetChild(0).localPosition + Vector3.Scale(Quaternion.Euler(entity.transform.GetChild(0).rotation.eulerAngles) * baseVerts[i], entity.transform.GetChild(0).localScale);
        }
        treeMeshVerts = new Vector3[baseVerts.Length];
        baseVerts.CopyTo(treeMeshVerts, 0);

    }


    private static Vector3[] GetVerts(Species species, Vector3 position, float size) {

        Vector3[] verts;
        switch(species) {
            case Species.Rabbit:
            verts = new Vector3[rabbitMeshVerts.Length];
            for(int i = 0; i < rabbitMeshVerts.Length; i++)
                verts[i] = rabbitMeshVerts[i] * size + position;
            break;
            case Species.Fox:
            verts = new Vector3[foxMeshVerts.Length];
            for(int i = 0; i < foxMeshVerts.Length; i++)
                verts[i] = foxMeshVerts[i] * size + position;
            break;
            case Species.Plant:
            verts = new Vector3[treeMeshVerts.Length];
            for(int i = 0; i < treeMeshVerts.Length; i++)
                verts[i] = treeMeshVerts[i] * size + position;
            break;
            default:
            verts = new Vector3[0];
            break;
        }

        return verts;

    }

    public static Vector3[] GetVerts(Entity entity)
    {
        if(entity.GetSpecies().Equals(Species.Water))
        {
            return ((WaterPond)entity).GetVerts();
        }
        return GetVerts(entity.GetSpecies(), entity.gameObject.transform.position, entity.GetSize());
    }


    public static Vector3[] GetVerts(Collider collider) {

        Species species = Entity.GetSpecies(collider.gameObject);
        
        return GetVerts(Entity.GetEntity(collider.gameObject));

    }

}
