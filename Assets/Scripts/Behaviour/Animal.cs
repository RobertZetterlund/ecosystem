using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts;

public class Animal : MonoBehaviour, IConsumable
{
    // traits that could be in fcm:
    private RangedDouble hunger = new RangedDouble(0, 0, 1);
    private RangedDouble thirst = new RangedDouble(0, 0, 1);
    private double energy = 1;
    private RangedDouble dietFactor; // 1 = carnivore, 0.5 = omnivore, 0 = herbivore
    protected EntityAction currentAction = EntityAction.Idle;
    private bool isMale;
    private RangedInt nChildren; // how many kids you will have
    private RangedDouble size;
    private RangedDouble heat = new RangedDouble(0, 0, 1); // aka fuq-o-meter
    private RangedDouble speed;
    // internal traits
    double timeToDeathByHunger = 200;
    double timeToDeathByThirst = 200;
    private static double BITE_FACTOR = 0.2; // use to calculate how much you eat in one bite
    double lifespan = 2000;
    bool dead;
    public NavMeshAgent navMeshAgent;
    private FCMHandler fcmHandler;
    private float lastFCMUpdate = 0;
    private string targetGametag = "";
    private ArrayList sensedGameObjects;
    private Species species;
    private RangedDouble maxSize;
    private RangedDouble infantFactor; // how big the child is in %
    private RangedDouble growthFactor; // how much you grow each tick
    private RangedDouble heatTimer; // how many ticks the heat should increase before maxing out
    // senses
    private float senseRadius;
	private AbstractSensor[] sensors;
    private float sightLength = 25;
    private float smellRadius = 7;
    private float horisontalFOV = 90;
    private float verticalFOV = 45;
    private GameObject targetGameObject;
    private Transform currentTargetTransform;
    // ui
    private StatusBars statusBars;
    private Component[] childRenderers;
    //Debugging
    public bool showFCMGizmo, showSenseRadiusGizmo, showSightGizmo, showSmellGizmo = false;
    UnityEngine.Color SphereGizmoColor = new UnityEngine.Color(1, 1, 0, 0.3f);
    // trait copy for easier logging etc
    AnimalTraits traits;

    public void Init(AnimalTraits traits)
    {
        this.species = traits.species;
        this.dietFactor = new RangedDouble(traits.dietFactor, 0, 1);
        this.maxSize = new RangedDouble(traits.maxSize, 0);
        this.size = new RangedDouble(traits.maxSize * traits.infantFactor, 0, traits.maxSize);
        this.nChildren = new RangedInt(traits.nChildren, 1);
        this.infantFactor = new RangedDouble(traits.infantFactor, 0, 1);
        this.growthFactor = new RangedDouble(traits.growthFactor, 0, 1);
        this.speed = new RangedDouble(traits.speed, 0);
        this.fcmHandler = traits.fcmHandler;
        System.Random rand = new System.Random();
        isMale = rand.NextDouble() >= 0.5;
        this.heatTimer = new RangedDouble(traits.heatTimer, 1);

        this.traits = traits;

        targetGameObject = null;

        GameController.Register(species);
    }

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = gameObject.AddComponent(typeof(NavMeshAgent)) as NavMeshAgent;
        navMeshAgent.speed = (float)speed.GetValue();
        // calculate instead if possible
        navMeshAgent.baseOffset = OrganismFactory.GetOffset(species);
        
        CapsuleCollider c = gameObject.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
        c.height = 2;

        //MeshRenderer r = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        //r.material = Resources.Load("unity_builtin_extra/Default-Material", typeof(Material)) as Material; // not working

        gameObject.layer = 8;

        //The concept of senseRadius does not make any sense anymore, but the variable is used still.
        senseRadius = 10;

        sensors = new AbstractSensor[2];
        sensors[0] = SensorFactory.SightSensor(sightLength, horisontalFOV, verticalFOV);
        sensors[1] = SensorFactory.SmellSensor(smellRadius);

        // update ui and visual traits
        UpdateSize();
        UnityEngine.Object prefab = Resources.Load("statusCanvas");
        GameObject canvas = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        statusBars = canvas.GetComponent(typeof(StatusBars)) as StatusBars;
        canvas.transform.parent = gameObject.transform;
        childRenderers = GetComponentsInChildren<Renderer>();
        UpdateStatusBars();
    }

    void Sense()
    {
        sensedGameObjects = new ArrayList();
        // This could be used for a more comfortable way of handling sensed events.
        // Although I'm not 100% sure thats its working correctly atm. Not sure if we will need it either.
        //ArrayList sensedObjectEvents = new ArrayList();
        foreach (AbstractSensor sensor in sensors)
        {
            foreach (GameObject gameObject in sensor.Sense(transform))
            {
                //SensedObjectEvent sensedObjectEvent = new SensedObjectEvent(this, gameObject, sensor.sensorType);
                //sensedObjectEvents.Add(sensedObjectEvent);
                sensedGameObjects.Add(gameObject);
                if (!targetGametag.Equals("") && gameObject.CompareTag(targetGametag))
                {
                    targetGameObject = gameObject;
                    //StopAllCoroutines();

                    // decide to go towards that gameObject

                    //currentTargetTransform = gameObject.transform;

                    //FollowMyCurrentTarget(gameObject);


                    // break;

                    //SetDestination(gameObject.transform.position);
                }
            }
        }

        fcmHandler.ProcessSensedObjects(this, sensedGameObjects);

    }

    void FollowMyCurrentTarget(GameObject gameObject)
    {
        while (currentTargetTransform != null ^ Vector3.Distance(currentTargetTransform.position, this.transform.position) < 15)
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
        return Vector3.Distance(position1, position2) < 5; //we probabbly need to update this number later on
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
        Consume(target);
    }

    private void Eat(IConsumable target)
    {
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
            UpdateSize();
        }
        hunger.Add(Time.deltaTime * 1 / timeToDeathByHunger * ((size.GetValue() + growth) * speed.GetValue() + senseRadius));
        thirst.Add(Time.deltaTime * 1 / timeToDeathByThirst);

        //age the animal
        energy -= Time.deltaTime * 1 / lifespan;

        heat.Add(1 / heatTimer.GetValue());

        Sense();
        if ((Time.time - lastFCMUpdate) > 1)
        {
            lastFCMUpdate = Time.time;
            fcmHandler.CalculateFCM();
        }

        UpdateStatusBars();
        TraitLogger.Log(traits);

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
            GameController.Unregister(traits);
            Destroy(gameObject);
        }

    }

    public void chooseNextAction()
    {
        CheckCurrentAction(fcmHandler.GetAction());
        //currentAction = fcmHandler.GetAction();


        //doAction();
        // a method that makes the animal eat, drink, or reproduce
    }

    private void CheckCurrentAction(EntityAction newAction)
    {
        
        if(currentAction != newAction)
        {
            StopAllCoroutines();
            currentAction = fcmHandler.GetAction();
            switch (currentAction)
            {
                case EntityAction.GoingToWater:
                    StartCoroutine(GoToWater());
                    break;

                case EntityAction.GoingToFood:
                    StartCoroutine(GoToFood());
                    break;
            }
        }
        else
        {
            if(!GameObjectExists(targetGameObject))
            {
                //Debug.Log("I dont see my target");
                currentAction = EntityAction.Idle;
                //Choose the next bestTarget

            }

        }

    }

    private bool GameObjectExists(GameObject target)
    {
        foreach(GameObject gameObject in sensedGameObjects)
        {
            if (gameObject.Equals(target))
            {
                return true;
            }
        }
        return false;
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
        // reduce heat (assume you did the fucko but even if the animals were incompatible biologically)
        heat.Add(-1);

        if (size.GetValue() < maxSize.GetValue())
        {
            // if still a child or wounded
            return;
        }

        if (species != mate.species)
        {
            // if different species
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
                    double heatTimer = ReproductionUtility.ReproduceRangedDouble(this.heatTimer.Duplicate(), mate.heatTimer.Duplicate()).GetValue();
                    FCMHandler fcmHandler = this.fcmHandler.Reproduce(mate.fcmHandler);

                    AnimalTraits child = new AnimalTraits(species, maxSize, dietFactor, nChildren, infantFactor, growthFactor, speed, heatTimer, fcmHandler);

                    Vector3 mother;
                    if (isMale)
                    {
                        mother = mate.transform.position;
                    }
                    else
                    {
                        mother = transform.position;
                    }

                    OrganismFactory.CreateAnimal(child, mother);
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

        /*if (showSenseRadiusGizmo)
        {
            Gizmos.color = SphereGizmoColor;
            Gizmos.DrawSphere(transform.position, senseRadius);
        }*/

        if(showSightGizmo)
        {
            float hFOV = horisontalFOV;
            //float vFOV = verticalFOV;
            var pos1 = transform.position + Quaternion.AngleAxis(hFOV / 2, transform.up) * transform.forward * sightLength;
            //var pos2 = transform.position + Quaternion.AngleAxis(vFOV / 2, transform.right) * transform.forward * radius;
            var pos2 = transform.position + Quaternion.AngleAxis(-hFOV / 2, transform.up) * transform.forward * sightLength;
            //var pos4 = transform.position + Quaternion.AngleAxis(-vFOV / 2, transform.right) * transform.forward * radius;

            Gizmos.DrawLine(transform.position, pos1);
            Gizmos.DrawLine(transform.position, pos2);

            var prev = pos2;
            int start = (int)(-hFOV / 2);
            int end = (int)(hFOV / 2);
            for (int i = start; i <= end; i += 10)
            {
                var newpos = transform.position + Quaternion.AngleAxis(i, transform.up) * transform.forward * sightLength;
                Gizmos.DrawLine(prev, newpos);
                prev = newpos;
            }
        }
        if (showSmellGizmo)
        {
            var pos1 = transform.position + Quaternion.AngleAxis(0, transform.up) * transform.forward * smellRadius;

            var prev = pos1;
            for (int i = 20; i <= 360; i += 20)
            {
                var newpos = transform.position + Quaternion.AngleAxis(i, transform.up) * transform.forward * smellRadius;
                Gizmos.DrawLine(prev, newpos);
                prev = newpos;
            }
        }


    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return navMeshAgent;
    }

    public IEnumerator GoToFood()
    {
        yield return Search("Plant");
        IConsumable consumable = targetGameObject.GetComponent<MyTestPlant>();
        yield return Approach(targetGameObject);
        for(int i = 0; i < 5; i++)
        {
            Eat(consumable); // take one bite
            new WaitForSeconds(1);
        }
        
        currentAction = EntityAction.Idle;
        yield return null;
   
    }

    public IEnumerator Approach(GameObject targetGameObject)
    {
        while(!CloseEnoughToAct(transform.position, targetGameObject.transform.position))
        {
            yield return new WaitForSeconds(0.1f);
            SetDestination(targetGameObject.transform.position);
        }
        yield return null;
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

        //SetDestination(pos);
        //yield return null;

        NavMeshPath path = new NavMeshPath();
        bool canPath = navMeshAgent.CalculatePath(pos, path);
      
        if (path.status == NavMeshPathStatus.PathComplete && canPath)
        {
            SetDestination(pos);
        }
        else
        {
            transform.Rotate(Vector3.up, 40);
            Walk();
        }
        
        yield return new WaitForSeconds(0);
    }

    public IEnumerator Search(string gametag)
    {
        targetGametag = gametag;
        while (targetGameObject == null)
        {
            yield return new WaitForSeconds(1);
            yield return Walk();
        }
        yield return null;

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

    private void UpdateSize()
    {
        Vector3 scale = gameObject.transform.localScale;
        scale = scale * (float)size.GetValue();
    }

    // update position and value of status bars
    private void UpdateStatusBars()
    {
        statusBars.UpdateStatus((float)hunger.GetValue(), (float)thirst.GetValue(), (float)energy);

        Renderer rend = (Renderer)childRenderers[0]; // take the first one
        Vector3 center = rend.bounds.center;
        float radius = rend.bounds.extents.magnitude;
        statusBars.gameObject.transform.position = center + new Vector3(0, radius, 0);
    }

}
