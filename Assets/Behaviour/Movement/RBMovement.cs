using System;
using UnityEngine;

/**
 * Moves an object by using a rigid body
 */
public class RBMovement : Movement
{
    private Rigidbody rb;

    public RBMovement(Rigidbody rb)
    {
        this.rb = rb;
        transform = rb.transform;
    }
    public override void Move()
    {
        base.Move();
        // The step size is equal to speed times frame time.
        float singleStep = 8 * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        //Vector3 newDirection = Vector3.RotateTowards(rb.transform.forward, direction, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        //Debug.DrawRay(rb.transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        //rb.MoveRotation(Quaternion.LookRotation(newDirection));
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
    }
}
