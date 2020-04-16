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
			}
		}
		else
		{
			state = ActionState.Idle;
		}

	}

	public Vector3 EscapeAnimal(Vector3 targetPos)
	{
		Vector3 myPos = animal.gameObject.transform.position;
		Vector3 dir = myPos - targetPos;
		float angle = Vector3.SignedAngle(dir, Vector3.forward, Vector3.up);
		Vector3 new_directon = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * angle), 0, Mathf.Cos(Mathf.Deg2Rad * angle));

		Vector3 myNewPos = myPos + new_directon * 10;

		return myNewPos;
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
