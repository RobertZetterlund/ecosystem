using System.Collections;
using UnityEngine;

class Fox : Animal
{
    public override void Init(AnimalTraits traits)
    {
        base.Init(traits);
    }
    protected override void Start()
    {
        runAnimationspeedFactor = 1f;
        base.Start();
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


    public override IEnumerator EatConsumable(ConsumptionType consumptionType)
    {
        IConsumable consumable = null;
        switch (consumptionType)
        {
            case ConsumptionType.Animal:
                consumable = targetGameObject.GetComponent<Animal>();
                break;
            case ConsumptionType.Plant:
                consumable = targetGameObject.GetComponent<Plant>();
                break;
            case ConsumptionType.Water:
                consumable = targetGameObject.GetComponent<Water>();
                break;
        }
        state = ActionState.Eating;
        // foxes eat once
        for (int i = 0; i < 1; i++)
        {
            yield return new WaitForSeconds(1);
            if (consumable == null || consumable.GetAmount() == 0)
                break;
            Eat(consumable); // take one bite

        }
        state = ActionState.Idle;
        yield return null;
    }

}