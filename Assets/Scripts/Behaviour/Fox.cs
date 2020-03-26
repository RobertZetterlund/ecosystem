using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Behaviour
{
    class Fox : Animal
    {
        private ActionState state = new ActionState(); //should be same state as animal?
        private GameObject targetGameObject; //do i need getters or something here??

        //Override
        public IEnumerator GoToFood()
        {
            state = ActionState.GoingToFood; //cant access state or target gameobject so they are defined above in a bs way. needs to be fixed
            string gametag = ConsumptionType.Plant.ToString();
            yield return StartCoroutine(Search(gametag));
            IConsumable target = (IConsumable)targetGameObject.GetType();
            Animal prey = (Animal)target;
            yield return StartCoroutine(ChaseAnimal(prey));
            state = ActionState.Idle;
            currentAction = EntityAction.Idle;
        }
    }
}
