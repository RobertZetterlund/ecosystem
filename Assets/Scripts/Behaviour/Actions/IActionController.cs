
using System.Collections;

interface IActionController
{
    IEnumerator GoToFood();

    IEnumerator GoToWater();

    IEnumerator GoToPartner();

    IEnumerator ChaseAnimal(Animal animal);

    IEnumerator EscapeAnimal(Animal animal);
}

