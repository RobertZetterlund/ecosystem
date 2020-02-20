using System;
using UnityEngine;

/**
 * interface for moving objects
 */
public interface IMovement
{
    void Move();

    void Stop();

    void SetDirection(Vector3 direction);

    Vector3 GetDirection();

    void SetSpeed(float speed);

    float GetSpeed();

    void SetTargetDestination(Vector3 destination);
}
