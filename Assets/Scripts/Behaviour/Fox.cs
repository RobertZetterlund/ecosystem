using System;
using System.Collections;

class Fox : Animal
{
    public override void Init(AnimalTraits traits)
    {
        base.Init(traits);
    }

    //Override
    public override IEnumerator GoToFood()
    {
        state = ActionState.GoingToFood; //cant access state or target gameobject so they are defined above in a bs way. needs to be fixed
        string gametag = ConsumptionType.Animal.ToString();
        yield return StartCoroutine(Search(gametag));        
        yield return StartCoroutine(ChaseAnimal(targetGameObject));
        state = ActionState.Idle;
        currentAction = EntityAction.Idle;
    }
}