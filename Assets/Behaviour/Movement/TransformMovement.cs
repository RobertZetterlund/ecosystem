using System;
using UnityEngine;


/**
 * Moves an object by using its transform
 */
public class TransformMovement : Movement
{
    private Transform transform;

    public TransformMovement(Transform transform)
    {
        this.transform = transform;
    }
    public override void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
