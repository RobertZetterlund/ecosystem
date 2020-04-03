using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



class Fox : Animal
{
    public override void Init(AnimalTraits traits)
    {
        base.Init(traits);
    }

    //Override
    public override IEnumerator GoToFood()
    {
        state = ActionState.GoingToFood;
        string gametag = ConsumptionType.Animal.ToString();
        
        //Vector3 pos = new Vector3(0, 0, 0);
        bool retry;
        do
        {
            yield return StartCoroutine(Search(gametag));
            try
            {
                retry = false;
            }
            catch (MissingReferenceException)
            {
                retry = true;
            }
        } while (retry);
        yield return StartCoroutine(ChaseAnimal(targetGameObject));
        state = ActionState.Idle;
        currentAction = EntityAction.Idle;
    }

    public IEnumerator ChaseAnimal(GameObject animal)
    {
        
        state = ActionState.Approaching;

        while (animal != null && !CloseEnoughToAct(animal))
        {
            yield return new WaitForSeconds(0.2f);
            if (animal != null)
            {

                SetDestination(animal.transform.position);

            }

        }
        // To prevent the animal from not going further than necessary to perform its action.
        // I wanted to use the stop function of the NavMeshAgent but if one does use that one also
        // has to resume the movement when you want the animal to walk again, so I did it this way instead.
        SetDestination(transform.position);
        yield return null;
        if(animal != null)
        {
            yield return StartCoroutine(GoToStationaryConsumable(ConsumptionType.Animal, animal.transform.position));
        }
        state = ActionState.Idle;
        currentAction = EntityAction.Idle;

    }
}