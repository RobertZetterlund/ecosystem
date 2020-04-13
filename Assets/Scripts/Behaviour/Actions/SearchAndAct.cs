using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class SearchAndAct : AbstractAction
{
    protected TickTimer searchTimer, approachTimer;
    protected IPosition targetPos;

    public SearchAndAct(Animal animal) : base(animal)
    {
        searchTimer = new TickTimer(1);
        approachTimer = new TickTimer(1f);
    }

    public override void Execute()
    {
        switch (state)
        {
            case ActionState.Searching:
                Search();
                break;
            case ActionState.Approaching:
                Approach();
                break;
            case ActionState.Acting:
                Act();
                break;
        }
    }

    protected virtual bool Act()
    {
        state = ActionState.Acting;
        if (animal.targetGameObject == null || !animal.CloseEnoughToAct(animal.targetGameObject))
        {
            //Found food does not exist, GO back to search phase
            Reset();
            return false;
        }
        return true;
    }
    protected virtual bool Search()
    {
        searchTimer.Tick();
        state = ActionState.Searching;
        if (animal.targetGameObject == null)
        {
            if (searchTimer.IsDone())
            {
                searchTimer.Reset();
                Roam();
            }
        }
        else
        {
            try
            {

                targetPos = PositionToApproach();
                return Approach();
            }
            catch (MissingReferenceException)
            {

            }
        }
        return true;
    }

    public bool Approach()
    {
        approachTimer.Tick();
        state = ActionState.Approaching;
        if (animal.targetGameObject == null)
        {
            //Approached Object does not exist anymore, go back to search phase
            Reset();
            return false;
        }


        if (!animal.CloseEnoughToAct(animal.targetGameObject))
        {
            if (approachTimer.IsDone())
            {
                try
                {
                    Vector3 pos = targetPos.GetPos();
                    animal.GoToStationaryPosition(pos);
                }
                catch (Exception)
                {  
                } 

                approachTimer.Reset();
            }
        }
        else
        {
            // To prevent the animal from not going further than necessary to perform its action.
            // I wanted to use the stop function of the NavMeshAgent but if one does use that one also
            // has to resume the movement when you want the animal to walk again, so I did it this way instead.
            animal.SetDestination(animal.transform.position);
            return Act();
        }
        return true;

    }

    protected virtual IPosition PositionToApproach()
    {
        return new DynamicPosition(animal.targetGameObject.transform);
    }


    public void Roam()
    {
        Vector3 pos = ChooseNewDestination();
        animal.GoToStationaryPosition(pos);
    }

    /**
     * Makes the animal walk to a position 10 steps in front of the animal in a direction that is in the bounderies of an angle
     * of -40 to +40 of the direction that the animal is facing.
     */
    private Vector3 ChooseNewDestination()
    {
        Vector3 dir = animal.transform.forward;
        float angle = Vector3.SignedAngle(dir, Vector3.forward, Vector3.up);
        float angle1 = angle - 40;
        float angle2 = angle + 40;
        float new_angle = UnityEngine.Random.Range(angle1, angle2);
        Vector3 new_directon = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * new_angle), 0, Mathf.Cos(Mathf.Deg2Rad * new_angle));

        Vector3 new_pos = animal.transform.position + new_directon * 30;
        return new_pos;
    }

    public override void Reset()
    {
        state = ActionState.Searching;
    }

    public override bool IsDone()
    {
        return state == ActionState.Done;
    }

}