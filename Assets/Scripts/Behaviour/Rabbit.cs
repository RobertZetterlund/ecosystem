using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Behaviour
{
    class Rabbit : Animal
    {
        private ActionState state = new ActionState(); //should be same state as animal?
        private GameObject targetGameObject; //do i need getters or something here??

        //override
        public IEnumerator GoToFood()
        {
            state = ActionState.GoingToFood; //cant access state or target gameobject so they are defined above in a bs way. needs to be fixed
            string gametag = ConsumptionType.Plant.ToString();
            yield return StartCoroutine(Search(gametag));
            yield return StartCoroutine(GoToStationaryConsumable(ConsumptionType.Plant, targetGameObject.transform.position));
            state = ActionState.Idle;
            currentAction = EntityAction.Idle;
        }
    }
}
