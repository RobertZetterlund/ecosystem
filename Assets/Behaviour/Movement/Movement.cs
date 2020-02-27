using System;
using UnityEngine;

public abstract class Movement : IMovement
{
    //the direction that the subclass is moving in
    protected Vector3 direction = new Vector3(0f, 0f, 0f);

    protected Vector3 targetDestination = new Vector3(0f, 0f, 0f);

    private Vector3 acceptance = new Vector3(0.2f, 0.2f, 0.2f);

    protected Transform transform;

    protected float speed = 0f;

    protected float baseSpeed = 2;

    public virtual void Move()
    {
        if(HasReachedDestination())
        {
            speed = 0;
        }
    }

    public virtual void Stop()
    {
        speed = 0;
    }

    public Vector3 GetDirection()
    {
        return direction;
    }

    private void SetDirection(Vector3 direction)
    {
        //Normalize the direction
        direction = direction.normalized;
        this.direction = direction;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetTargetDestination(Vector3 destination)
    {
        this.targetDestination = destination;
        SetDirection(destination - transform.position);
        SetSpeed(baseSpeed);
    }

    public bool HasReachedDestination()
    {
        Debug.Log((targetDestination - transform.position).magnitude);
        return (targetDestination - transform.position).magnitude < acceptance.magnitude;
    }
}

