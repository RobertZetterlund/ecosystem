using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour, IConsumable
{
    private RangedDouble hunger = new RangedDouble(0, 0, 1);
    private RangedDouble thirst = new RangedDouble(0, 0, 1);
    double timeToDeathByHunger = 200;
    double timeToDeathByThirst = 200;
    private static double BITE_FACTOR = 0.2; // use to calculate how much you eat in one bite
    double lifespan = 2000;
    bool dead;
    private double energy = 1;
    private RangedDouble dietFactor; // 1 = carnivore, 0.5 = omnivore, 0 = herbivore
    protected EntityAction currentAction = EntityAction.Idle;
    public NavMeshAgent navMeshAgent;
    private FCMHandler fcmHandler;
    private float senseRadius;
    private ISensor[] sensors;
    private float lastFCMUpdate = 0;
    private string targetGametag = "";
    private bool isMale;
    private Species species;
    private RangedInt nChildren; // how many kids you will have
    private RangedDouble size;
    private RangedDouble maxSize;
    private RangedDouble infantFactor; // how big the child is in %
    private RangedDouble growthFactor; // how much you grow each tick
    private RangedDouble speed;

    private Transform currentTargetTransform;

    //Debugging
    UnityEngine.Color SphereGizmoColor = new UnityEngine.Color(1, 1, 0, 0.3f);
    public bool showFCMGizmo, showSenseRadiusGizmo = false;

    public void Init(Species species, double maxSize, double dietFactor, int nChildren, double infantFactor, double growthFactor, double speed)
    {
        this.species = species;
        this.dietFactor = new RangedDouble(dietFactor, 0, 1);
        this.maxSize = new RangedDouble(maxSize, 0);
        this.size = new RangedDouble(maxSize * infantFactor, 0, maxSize);
        this.nChildren = new RangedInt(nChildren, 1);
        this.infantFactor = new RangedDouble(infantFactor, 0, 1);
        this.growthFactor = new RangedDouble(growthFactor, 0, 1);
        this.speed = new RangedDouble(speed, 0);

        System.Random rand = new System.Random();
        isMale = rand.NextDouble() >= 0.5;


    }

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = gameObject.AddComponent(typeof(NavMeshAgent)) as NavMeshAgent;
        navMeshAgent.speed = 5;
        // calculate instead if possible
        navMeshAgent.baseOffset = OrganismFactory.GetOffset(species);

        CapsuleCollider c = gameObject.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
        c.height = 2;

        //MeshRenderer r = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        //r.material = Resources.Load("unity_builtin_extra/Default-Material", typeof(Material)) as Material; // not working

        gameObject.layer = 8;



        senseRadius = 7;

        fcmHandler = new RabbitFCMHandler();

        sensors = new ISensor[1];
        sensors[0] = new AreaSensor(senseRadius);
        StartCoroutine(GoToFood());

    }

    void Sense()
    {
        ArrayList sensedGameObjects = new ArrayList();
        foreach (ISensor sensor in sensors)
        {
            foreach (GameObject gameObject in sensor.Sense(transform))
            {
                sensedGameObjects.Add(gameObject);
                if (!targetGametag.Equals("") && gameObject.CompareTag(targetGametag))
                {
                    StopAllCoroutines();

                    // decide to go towards that gameObject

                    currentTargetTransform = gameObject.transform;

                    //FollowMyCurrentTarget(gameObject);


                    // break;

                    SetDestination(gameObject.transform.position);
                }
            }
        }

        fcmHandler.ProcessSensedObjects(this, sensedGameObjects);

    }

    void FollowMyCurrentTarget(GameObject gameObject)
    {
        while (currentTargetTransform != null ^ Vector3.Distance(currentTargetTransform.position, this.transform.position) < 5)
        {
            // move to that thing lol

            //If they are next to each other or the same position
            if (CloseEnoughToAct(currentTargetTransform.position, this.transform.position))
            {
                IConsumable target = (IConsumable)gameObject.GetType();
                Act(target);
            }
        }
    }

    private bool CloseEnoughToAct(Vector3 position1, Vector3 position2)
    {
        return Vector3.Distance(currentTargetTransform.position, this.transform.position) < 1; //we probabbly need to update this number later on
    }

    protected void Act(IConsumable currentTarget)
    {
        switch (currentAction)
        {
            case EntityAction.GoingToFood:
                Eat(currentTarget);
                break;
            case EntityAction.GoingToWater:
                Drink(currentTarget);
                break;
            case EntityAction.SearchingForMate:
                Animal mate = (Animal)currentTarget;
                Reproduce(mate);
                break;
        }
    }

    private void Drink(IConsumable target)
    {
        currentAction = EntityAction.Drinking;
        Consume(target);

    }

    private void Eat(IConsumable target)
    {
        currentAction = EntityAction.Eating;
        Consume(target);
    }

    // Update is called once per frame
    void Update()
    {
        // growth = hunger drain and size gain while growing
        double growth = 0;
        //increases hunger and thirst over time
        if (size.GetValue() < maxSize.GetValue()) // if not fully grown
        {
            growth = maxSize.GetValue() * growthFactor.GetValue();
            size.Add(growth);
        }
        hunger.Add(Time.deltaTime * 1 / timeToDeathByHunger * ((size.GetValue() + growth) * speed.GetValue() + senseRadius));
        thirst.Add(Time.deltaTime * 1 / timeToDeathByThirst);

        //age the animal
        energy -= Time.deltaTime * 1 / lifespan;

        Sense();
        if ((Time.time - lastFCMUpdate) > 1)
        {
            lastFCMUpdate = Time.time;
            fcmHandler.CalculateFCM();
        }

        chooseNextAction();

        //check if the animal is dead
        isDead();

    }



    public void isDead()
    {
        if (hunger.GetValue() >= 1)
        {
            Die(CauseOfDeath.Hunger);
        }
        else if (thirst.GetValue() >= 1)
        {
            Die(CauseOfDeath.Thirst);
        }
        else if (energy <= 0)
        {
            Die(CauseOfDeath.Age);
        }
    }

    public void Die(CauseOfDeath cause)
    {
        if (!dead)
        {
            dead = true;
            //Something.log(cause);
            //Environment.RegisterDeath (this);
            //Destroy (gameObject);
        }

    }

    public void chooseNextAction()
    {
        currentAction = fcmHandler.GetAction();
        if (EntityAction.Idle == currentAction || EntityAction.Resting == currentAction) // && Maybe mate nearby or maybe theyre always searching
        {
            findMate();
        }

        // Get current action
        bool eating = currentAction == EntityAction.Eating;
        bool drinking = currentAction == EntityAction.Drinking;
        //bool reproducing = currentAction == EntityAction.Reproducing; //Dont think we should allow animals to stop having sex. Once started they will finish


        // More hungry than thirsty
        if (hunger.GetValue() >= thirst.GetValue() || (eating && !isCriticallyThirsty()))
        {
            findFood();
        }
        // More thirsty than hungry
        else if (thirst.GetValue() > hunger.GetValue() || (drinking && !isCriticallyHungry()))
        {
            findWater();
        }



        //doAction();
        // a method that makes the animal eat, drink, or reproduce
    }

    private void findFood()
    {
        //some shit here
        //currentAction = EntityAction.GoingToFood;
    }

    private void findWater()
    {
        //some shit here
        //currentAction = EntityAction.GoingToWater;
    }

    private void findMate()
    {
        //Some shit here
        //currentAction = EntityAction.SearchingForMate;
    }

    public bool isCriticallyThirsty()
    {
        return thirst.GetValue() < 0.1; //change these values when we know more or avoid hardcoded values
    }

    public bool isCriticallyHungry()
    {
        return hunger.GetValue() < 0.1; //change these values when we know more or avoid hardcoded values
    }

    public void Reproduce(Animal mate)
    {
        if (size.GetValue() < maxSize.GetValue())
        {
            // if still a child or wounded
            return;
        }

        if (!(isMale ^ mate.isMale))
        {
            // if same sex
            return;
        }

        if (hunger.GetValue() < 0.3 && thirst.GetValue() < 0.6)
        {
            if (energy > 0.4)
            {


                //make #nChildren children
                for (int i = 0; i < nChildren.GetValue(); i++)
                {
                    double maxSize = ReproductionUtility.ReproduceRangedDouble(this.maxSize.Duplicate(), mate.maxSize.Duplicate()).GetValue();

                    // deplete hunger for each child born
                    // stop when your hunger would run out
                    if (hunger.Add(maxSize * this.infantFactor.GetValue()) != maxSize * this.infantFactor.GetValue())
                    {
                        return;
                    }

                    double dietFactor = ReproductionUtility.ReproduceRangedDouble(this.dietFactor.Duplicate(), mate.dietFactor.Duplicate()).GetValue();
                    int nChildren = ReproductionUtility.ReproduceRangedInt(this.nChildren.Duplicate(), mate.nChildren.Duplicate()).GetValue();
                    double infantFactor = ReproductionUtility.ReproduceRangedDouble(this.infantFactor.Duplicate(), mate.infantFactor.Duplicate()).GetValue();
                    double growthFactor = ReproductionUtility.ReproduceRangedDouble(this.growthFactor.Duplicate(), mate.growthFactor.Duplicate()).GetValue();
                    double speed = ReproductionUtility.ReproduceRangedDouble(this.speed.Duplicate(), mate.speed.Duplicate()).GetValue();

                    Vector3 mother;
                    if (isMale)
                    {
                        mother = mate.transform.position;
                    }
                    else
                    {
                        mother = transform.position;
                    }

                    OrganismFactory.CreateAnimal(species, maxSize, dietFactor, nChildren, infantFactor, growthFactor, speed, mother);
                }
            }
            //code here for sex
            currentAction = EntityAction.Idle; // Set action to idle when done
        }
    }

    // let this animal attempt to take a bite from the given consumable
    private void Consume(IConsumable consumable)
    {
        // do eating calculations
        double biteSize = size.GetValue() * BITE_FACTOR;
        ConsumptionType type = consumable.GetConsumptionType();

        swallow(consumable.Consume(biteSize), type);
    }



    // eat this animal
    public double Consume(double amount)
    {
        return size.Add(-amount);
    }


    // swallow the food/water that this animal ate
    private void swallow(double amount, ConsumptionType type)
    {
        amount /= size.GetValue(); // balance according to size. (note that amount will be higher if your size is bigger)
        // increment energy / hunger / thirst
        switch (type)
        {
            case ConsumptionType.Water:
                thirst.Add(-amount);
                break;
            case ConsumptionType.Animal:
                hunger.Add(-amount * dietFactor.GetValue());
                break;
            case ConsumptionType.Plant:
                hunger.Add(-amount * (1 - dietFactor.GetValue()));
                break;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }


    public float GetSenseRadius()
    {
        return senseRadius;
    }

    //Draws a sphere corresponding to its sense radius
    void OnDrawGizmos()
    {
        if (showFCMGizmo)
        {
            Vector3 textOffset = new Vector3(-3, 2, 0);
            Handles.Label(transform.position + textOffset, currentAction.ToString());
            textOffset = new Vector3(1, 2, 0);
            if (fcmHandler != null)
                Handles.Label(transform.position + textOffset, fcmHandler.GetFCMData());
        }

        if (showSenseRadiusGizmo)
        {
            Gizmos.color = SphereGizmoColor;
            Gizmos.DrawSphere(transform.position, senseRadius);
        }


    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return navMeshAgent;
    }

    public IEnumerator GoToFood()
    {
        yield return Search("Plant");
    }

    public IEnumerator GoToWater()
    {
        yield return Search("Water");
    }

    public IEnumerator GoToPartner()
    {
        throw new NotImplementedException();
    }

    public IEnumerator ChaseAnimal(Animal animal)
    {
        throw new NotImplementedException();
    }

    public IEnumerator EscapeAnimal(Animal animal)
    {
        throw new NotImplementedException();
    }

    IEnumerator Walk()
    {
        Vector3 pos = ChooseNewDestination();
        SetDestination(pos);
        yield return new WaitForSeconds(0);
    }

    public IEnumerator Search(string gametag)
    {
        targetGametag = gametag;
        while (true)
        {
            yield return new WaitForSeconds(1);
            yield return Walk();
        }
    }


    /**
     * Makes the animal walk to a position 10 steps in front of the animal in a direction that is in the bounderies of an angle
     * of -40 to +40 of the direction that the animal is facing.
     */
    private Vector3 ChooseNewDestination()
    {
        Vector3 dir = transform.forward;
        float angle = Vector3.SignedAngle(dir, Vector3.forward, Vector3.up);
        float angle1 = angle - 40;
        float angle2 = angle + 40;
        float new_angle = UnityEngine.Random.Range(angle1, angle2);
        Vector3 new_directon = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * new_angle), 0, Mathf.Cos(Mathf.Deg2Rad * new_angle));

        Vector3 new_pos = transform.position + new_directon * 10;
        return new_pos;
    }
    public double GetAmount()
    {
        return size.GetValue();
    }

    public ConsumptionType GetConsumptionType()
    {
        return ConsumptionType.Animal;

    }

}
