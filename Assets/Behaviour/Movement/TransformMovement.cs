using System;
using UnityEngine;


/**
 * Moves an object by using its transform
 */
public class TransformMovement : Movement
{

    public TransformMovement(Transform transform)
    {
        this.transform = transform;
    }
    public override void Move()
    {
        base.Move();
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
