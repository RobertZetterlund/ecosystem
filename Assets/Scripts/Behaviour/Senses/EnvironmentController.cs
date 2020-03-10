using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnvironmentController
{
    
    /**
     * returns all colliders that are fully or partly inside a sphere
     * defined by paramters center and radius
     */
     public static Collider[] CheckSurroundings(Vector3 center, float radius)
    {
        return Physics.OverlapSphere(center, radius);
    }
}
