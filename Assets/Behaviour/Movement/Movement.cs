using System;
using UnityEngine;

public abstract class Movement : IMovement
{
    //the direction that the subclass is moving in
    protected Vector3 direction = new Vector3(0f, 0f, 0f);

    protected Vector3 targetDestination = new Vector3(0f, 0f, 0f);

    protected float speed = 0f;

    public abstract void Move();

    public virtual void Stop()
    {
        speed = 0;
    }

    public Vector3 GetDirection()
    {
        return direction;
    }

    public void SetDirection(Vector3 direction)
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
        throw new NotImplementedException();
    }
}

