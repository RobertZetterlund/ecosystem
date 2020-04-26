using System;
using UnityEngine;
using UnityEngine.AI;

public class EscapeAction : AbstractAction
{
	protected TickTimer escapeTimer;
	protected Vector3 targetPos = new Vector3(0, 0, 0);

	public EscapeAction(Animal animal) : base(animal)
	{
		escapeTimer = new TickTimer(1);
	}

	public override void Execute()
	{
		Escape(); // only one action
	}

	protected virtual void Escape()
	{
		escapeTimer.Tick();
		if (escapeTimer.IsDone() && animal.targetGameObject != null) // roam if nothing to escape from
		{
			escapeTimer.Reset();
			state = ActionState.Escaping;
			try
			{
				targetPos = animal.targetGameObject.transform.position;
				animal.GoToStationaryPosition(EscapeAnimal(animal.targetGameObject.transform.position));
			}
			catch (MissingReferenceException)
			{
				// target died
				Reset();
			}
		}
		else
		{
			state = ActionState.Idle;
		}

	}

	public Vector3 EscapeAnimal(Vector3 targetPos)
	{

        // creates three "good" positions, sampling all of them, the path hit that is the furthest away from the animal is
        // currently the best option, hence selected. That means that rabbit will not run into wall.
        //animal.navMeshAgent.SamplePathPosition


		Vector3 myPos = animal.gameObject.transform.position;
		Vector3 dir = myPos - targetPos;
		float base_angle = Vector3.SignedAngle(dir, Vector3.forward, Vector3.up);
		float left_angle = base_angle - 100;
		float right_angle = base_angle + 100;
		Vector3 base_direction = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * base_angle), 0, Mathf.Cos(Mathf.Deg2Rad * base_angle));
		Vector3 left_direction = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * left_angle), 0, Mathf.Cos(Mathf.Deg2Rad * left_angle));
		Vector3 right_direction = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * right_angle), 0, Mathf.Cos(Mathf.Deg2Rad * right_angle));

		Vector3 base_pos = myPos + base_direction * 10;
		Vector3 left_pos = myPos + left_direction * 5;
		Vector3 right_pos = myPos + right_direction * 5;

		NavMesh.SamplePosition(base_pos, out NavMeshHit base_hit, 100, NavMesh.AllAreas);
		NavMesh.SamplePosition(left_pos, out NavMeshHit left_hit, 100, NavMesh.AllAreas);
		NavMesh.SamplePosition(right_pos, out NavMeshHit right_hit, 100, NavMesh.AllAreas);


		float base_dist = Vector3.Distance(base_hit.position, myPos);
		float left_dist = Vector3.Distance(left_hit.position, myPos);
		float right_dist = Vector3.Distance(right_hit.position, myPos);

        if(base_dist >= left_dist && base_dist >= right_dist)
        {
			return base_hit.position;
        }else if(left_dist >= right_dist)
        {
			return left_hit.position;
        }else
        {
			return right_hit.position;
        }
	}

	public override void Reset()
	{
		state = ActionState.Escaping;
	}

	public override bool IsDone()
	{
		return state == ActionState.Done;
	}

}
