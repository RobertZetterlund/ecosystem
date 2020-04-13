using System;
using System.Collections.Generic;
using UnityEngine;

public class ComponentNavigator
{

    public static Vector3[] rabbitMeshVerts;
    public static Vector3[] foxMeshVerts;
    public static Vector3[] treeMeshVerts;

    public static void LoadData(List<GameObject> puddleList){

        GameObject entity;
        Vector3[] baseVerts;
        Transform renderObjectTransform;

        // The rabbit has it's rotationa and scale values on a different child than the mesh
        entity = (GameObject)Resources.Load("testR");
        baseVerts = entity.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices;
        for(int i = 0; i < baseVerts.Length; i++) {
            if(i/38 != 0){ continue; }
            baseVerts[i] = entity.transform.GetChild(0).localPosition + Quaternion.Euler(entity.transform.GetChild(0).rotation.eulerAngles) * Vector3.Scale( baseVerts[i], entity.transform.GetChild(0).localScale);
        }
        rabbitMeshVerts = new Vector3[baseVerts.Length];
        baseVerts.CopyTo(rabbitMeshVerts, 0);
        

        // The fox has it's values on the child with the mesh
        entity = (GameObject)Resources.Load("testF");
        renderObjectTransform = entity.transform.GetChild(0).GetChild(0);
        baseVerts = renderObjectTransform.GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices;
        for(int i = 0; i < baseVerts.Length; i++)
        {
            if(i/25 != 0) { continue; }
            baseVerts[i] = new Vector3(0, 2.1f, 0) + renderObjectTransform.parent.GetChild(0).position + Quaternion.Euler(renderObjectTransform.rotation.eulerAngles) * Vector3.Scale( baseVerts[i], renderObjectTransform.lossyScale);
        }
        foxMeshVerts = new Vector3[baseVerts.Length];
        baseVerts.CopyTo(foxMeshVerts, 0);
        

        // The tree is like the fox and the sappling has the same mesh
        entity = (GameObject)Resources.Load("Tree");
        baseVerts = entity.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.vertices;
        for(int i = 0; i < baseVerts.Length; i++) {
            if(i/14 != 0) { continue; }
            baseVerts[i] = entity.transform.GetChild(0).localPosition + Quaternion.Euler( entity.transform.GetChild(0).rotation.eulerAngles) * Vector3.Scale(baseVerts[i], entity.transform.GetChild(0).localScale);
        }
        treeMeshVerts = new Vector3[baseVerts.Length];
        baseVerts.CopyTo(treeMeshVerts, 0);

        foreach(GameObject waterpuddle in puddleList)
        {
            ((Water)GetEntity(waterpuddle)).SetVerts(waterpuddle.GetComponent<MeshFilter>().sharedMesh.vertices);
        }

    }


    private static Vector3[] GetVerts(Species species, Vector3 position, Vector3 size, Vector3 rotation) {

        Vector3[] verts;
        switch(species) {
            case Species.Rabbit:
            verts = new Vector3[rabbitMeshVerts.Length];
            for(int i = 0; i < rabbitMeshVerts.Length; i++)
                verts[i] = Quaternion.Euler(rotation) * Vector3.Scale(rabbitMeshVerts[i], size) + position;
            break;
            case Species.Fox:
            verts = new Vector3[foxMeshVerts.Length];
            for(int i = 0; i < foxMeshVerts.Length; i++)
                verts[i] = Quaternion.Euler(rotation) * Vector3.Scale(foxMeshVerts[i], size) + position;
            break;
            case Species.Plant:
            verts = new Vector3[treeMeshVerts.Length];
            for(int i = 0; i < treeMeshVerts.Length; i++)
                verts[i] = Quaternion.Euler(rotation) * Vector3.Scale(treeMeshVerts[i], size) + position;
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
            return ((Water)entity).GetVerts();
        }
        return GetVerts(entity.GetSpecies(), entity.gameObject.transform.position, entity.gameObject.transform.localScale, entity.gameObject.transform.rotation.eulerAngles);
    }


    public static Vector3[] GetVerts(Collider collider) {

        Species species = GetSpecies(collider.gameObject);
        
        return GetVerts(GetEntity(collider.gameObject));

    }

    public static Species GetSpecies(GameObject gameObj) {
        gameObj = GoToHighestObject(gameObj);
        foreach(Species species in Enum.GetValues(typeof(Species))) {
            if(gameObj.tag.Equals(species.ToString())) {
                return species;
            }
        }

        return Species.Undefined;
    }

    public static Entity GetEntity(GameObject gameObj) {

        gameObj = GoToHighestObject(gameObj);
        return (Entity)gameObj.GetComponent(gameObj.tag.ToString());
    }

    public static GameObject GoToHighestObject(GameObject gameObj)
    {
        while(!object.ReferenceEquals(gameObj.transform.parent, null)) {
            gameObj = gameObj.transform.parent.gameObject;
        }
        return gameObj;
    }

    public static Animator GetAnimator(GameObject gameObj)
    {
        return GoToHighestObject(gameObj).GetComponentInChildren<Animator>();

    }

    // Scans all vertices to find nearest
    public static Vector3 GetClosesVert(Vector3 fromPoint, GameObject gameObj) 
    {
        if(GetSpecies(gameObj) == Species.Water)
        {
            return GetClosesVert(fromPoint, GetVerts(GetEntity(gameObj)));
        }
        return gameObj.transform.position;
    }

    public static Vector3 GetClosesVert(Vector3 fromPoint, Vector3[] verts)
    {
        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;
        foreach(Vector3 vertex in verts)
        {
            Vector3 diff = fromPoint - vertex;
            float distSqr = diff.sqrMagnitude;
                if(distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                nearestVertex = vertex;
            }
        }
        return nearestVertex;
    }

}
