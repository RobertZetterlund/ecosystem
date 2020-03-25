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
    private ActionState state = new ActionState();
    private bool isMale;
    private RangedInt nChildren; // how many kids you will have
    private RangedDouble size;
    private RangedDouble heat = new RangedDouble(0, 0, 1); // aka fuq-o-meter
    private RangedDouble speed;
    // internal traits
    double timeToDeathByHunger = 600;
    double timeToDeathByThirst = 200;
    private static double BITE_FACTOR = 10; // use to calculate how much you eat in one bite
    double lifespan = 2000;
    bool dead;
    public NavMeshAgent navMeshAgent;
    private FCMHandler fcmHandler;
    private string targetGametag = "";
    private ArrayList sensedGameObjects;
    private Species species;
    private RangedDouble maxSize;
    private RangedDouble infantFactor; // how big the child is in %
    private RangedDouble growthFactor; // how much you grow each tick
    private RangedDouble heatTimer; // how many ticks the heat should increase before maxing out
    // senses
    private Timer senseTimer, fcmTimer;
    private float senseRadius;
	private AbstractSensor[] sensors;
    private AbstractSensor touchSensor;
    private float sightLength = 25;
    private float smellRadius = 7;
    private float horisontalFOV = 120;
    private float verticalFOV = 90;
    private GameObject targetGameObject;
    private Transform currentTargetTransform;
    // ui
    private StatusBars statusBars;
    private Component[] childRenderers;
    //Debugging
    public bool showFCMGizmo, showSenseRadiusGizmo, showSightGizmo, showSmellGizmo, showTargetDestinationGizmo = false;
    UnityEngine.Color SphereGizmoColor = new UnityEngine.Color(1, 1, 0, 0.3f);
    Vector3 targetDestinationGizmo = new Vector3(0, 0, 0);
    // trait copy for easier logging etc
    private AnimalTraits traits;
    private bool logNext = false;
    private Vector3 lastPos;
    private Animator animator;

    private Memory memory;
    private SenseProcessor senseProcessor;

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

        gameObject.tag = species.ToString();

        GameController.Register(species);
    }

    // Start is called before the first frame update
    void Start()
    {

        memory = new Memory();
        senseProcessor = new SenseProcessor(this);



        navMeshAgent = gameObject.AddComponent(typeof(NavMeshAgent)) as NavMeshAgent;
        navMeshAgent.speed = (float)speed.GetValue();
        // calculate instead if possible
        navMeshAgent.baseOffset = OrganismFactory.GetOffset(species);
        
        CapsuleCollider c = gameObject.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
        c.height = 2;

        //MeshRenderer r = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        //r.material = Resources.Load("unity_builtin_extra/Default-Material", typeof(Material)) as Material; // not working

        gameObject.layer = 8;
        lastPos = transform.position;
        animator = GetComponent<Animator>();

        //The concept of senseRadius does not make any sense anymore, but the variable is used still.
        senseRadius = 10;

        sensors = new AbstractSensor[2];
        sensors[0] = SensorFactory.SightSensor(sightLength, horisontalFOV, verticalFOV);
        sensors[1] = SensorFactory.SmellSensor(smellRadius);
        touchSensor = SensorFactory.TouchSensor(0.5f);

        senseTimer = new Timer(0.25f);
        fcmTimer = new Timer(0.25f);

        // update ui and visual traits
        UpdateSize();
        UnityEngine.Object prefab = Resources.Load("statusCanvas");
        GameObject canvas = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        statusBars = canvas.GetComponent(typeof(StatusBars)) as StatusBars;
        canvas.transform.SetParent(gameObject.transform, false);
        childRenderers = GetComponentsInChildren<Renderer>();
        UpdateStatusBars();
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

        if (senseTimer.IsDone())
        {
            Sense();
            senseTimer.Reset();
            senseTimer.Start();
        }
        if (fcmTimer.IsDone())
        {
            fcmHandler.CalculateFCM();
            fcmTimer.Reset();
            fcmTimer.Start();
        }

        UpdateStatusBars();
		if (logNext)
        {
            TraitLogger.Log(traits);
        }

        chooseNextAction();

        //check if the animal is dead
        if(GameController.animalCanDie)
            isDead();

        //Animation

        UpdateAnimation();

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
                if (this.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                {
                sensedGameObjects.Add(gameObject);

                }
            }
        }

        SensedEvent sE = senseProcessor.Process(sensedGameObjects);
        memory.WriteSensedEventToMemory(sE);

        IDictionary<string, int> impactMap = sE.GetWeightMap();

        fcmHandler.ProcessSensedObjects(this, sE);
    }

    void FollowMyCurrentTarget(GameObject gameObject)
    {
        while (currentTargetTransform != null ^ Vector3.Distance(currentTargetTransform.position, this.transform.position) < 15)
        {
            // move to that thing lol

            //If they are next to each other or the same position
            if (CloseEnoughToAct(currentTargetTransform.gameObject))
            {
                IConsumable target = (IConsumable)gameObject.GetType();
                Act(target);
            }
        }
    }

    private bool CloseEnoughToAct(GameObject gameObject)
    {
        return touchSensor.IsSensingObject(transform, gameObject);
    }

    /*private bool CloseEnoughToAct(Vector3 position1, Vector3 position2)
    {
        return Vector3.Distance(position1, position2) < 1; //we probabbly need to update this number later on
    }

    private bool CloseEnoughToAct(Collider collider1, Collider collider2)
    {
        return collider1.bounds.Intersects(collider2.bounds);
    }*/

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

    void LateUpdate()
    {
        logNext = TraitLogger.logNext;
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
            currentAction = newAction;
            //This has to reset somewhere
            targetGameObject = null;
            switch (currentAction)
            {
                case EntityAction.GoingToWater:
                    targetGameObject = memory.ReadWaterFromMemory();

                    StartCoroutine(GoToWater());
                    break;

                case EntityAction.GoingToFood:
                    targetGameObject = memory.ReadFoodFromMemory();

                    StartCoroutine(GoToFood());
                    break;
            }
        }
        else
        {
            if(!GameObjectExists(targetGameObject))
            {
                //Debug.Log("I dont see my target");
                //currentAction = EntityAction.Idle;
                //Choose the next bestTarget

            }

        }

    }

    private bool GameObjectExists(GameObject target)
    {
        if (sensedGameObjects == null)
            return false;

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
        amount /= (size.GetValue()); // balance according to size. (note that amount will be higher if your size is bigger)
        // increment energy / hunger / thirst
        switch (type)
        {
            case ConsumptionType.Water:
                thirst.Add(amount);
                break;
            case ConsumptionType.Animal:
                hunger.Add(amount * dietFactor.GetValue());
                break;
            case ConsumptionType.Plant:
                hunger.Add(amount * (1 - dietFactor.GetValue()));
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
 
            Vector3 textOffset = new Vector3(10, 2, 0);
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
        if(showTargetDestinationGizmo)
        {
            Gizmos.DrawLine(transform.position, targetDestinationGizmo);
        }

        Handles.Label(transform.position + new Vector3(0, 3, 0), state.ToString());


    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return navMeshAgent;
    }

    public IEnumerator GoToStationaryConsumable(ConsumptionType consumptionType)
    {
        string gametag = consumptionType.ToString();
        yield return StartCoroutine(Search(gametag));
        yield return StartCoroutine(Approach(targetGameObject));
        yield return StartCoroutine(EatConsumable(consumptionType));
    }

    public IEnumerator EatConsumable(ConsumptionType consumptionType)
    {
        IConsumable consumable = null;
        switch(consumptionType)
        {
            case ConsumptionType.Animal:
                throw new NotImplementedException();
                break;
            case ConsumptionType.Plant:
                consumable = targetGameObject.GetComponent<Plant>();
                break;
            case ConsumptionType.Water:
                consumable = targetGameObject.GetComponent<WaterPond>();
                break;
        }
        state = ActionState.Eating;
        for (int i = 0; i < 5; i++)
        {
            if (consumable == null || consumable.GetAmount() == 0)
                break;

            Eat(consumable); // take one bite
            yield return new WaitForSeconds(1);
        }
        yield return null;
    }

    public IEnumerator GoToFood()
    {
        state = ActionState.GoingToFood;
        yield return StartCoroutine(GoToStationaryConsumable(ConsumptionType.Plant));
        state = ActionState.Idle;
        currentAction = EntityAction.Idle;
    }

    public IEnumerator Approach(GameObject targetGameObject)
    {
        state = ActionState.Approaching;

        while(targetGameObject != null && !CloseEnoughToAct(targetGameObject))
        {
            yield return new WaitForSeconds(0.2f);
            if(targetGameObject != null)
                SetDestination(targetGameObject.transform.position);
        }
        // To prevent the animal from not going further than necessary to perform its action.
        // I wanted to use the stop function of the NavMeshAgent but if one does use that one also
        // has to resume the movement when you want the animal to walk again, so I did it this way instead.
        SetDestination(transform.position);
        yield return null;
    }

    public IEnumerator GoToWater()
    {
        state = ActionState.GoingToWater;
        yield return StartCoroutine(GoToStationaryConsumable(ConsumptionType.Water));
        state = ActionState.Idle;
        currentAction = EntityAction.Idle;
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
        NavMeshPath path = new NavMeshPath();
        bool canPath = navMeshAgent.CalculatePath(pos, path);

        if (path.status == NavMeshPathStatus.PathComplete && canPath)
        {
            targetDestinationGizmo = pos;
            SetDestination(pos);
        }
        else
        {
            NavMeshHit myNavHit;
            if (NavMesh.SamplePosition(pos, out myNavHit, 100, -1))
            {
                targetDestinationGizmo = myNavHit.position;
                SetDestination(myNavHit.position);
            }
        }

        yield return null;
    }

    public IEnumerator Search(string gametag)
    {
        state = ActionState.Searching;
        targetGametag = gametag;
        //Make it search before actually walking, since it otherwise might walk away from a plant
        //and then walk right back to it.
        Sense();
        senseTimer.Reset();
        senseTimer.Start();
        while (targetGameObject == null)
        {
            yield return StartCoroutine(Walk());
            yield return new WaitForSeconds(1);
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

    //Currently used for testing
    public FCMHandler GetFCMHandler()
    {
        return fcmHandler;
    }

    public void SetFCMHandler(FCMHandler fcmHandler)
    {
        this.fcmHandler = fcmHandler;
    }


    void UpdateAnimation()
    {
        
        Vector3 deltaV = new Vector3(transform.position.x - lastPos.x, transform.position.y - lastPos.y, transform.position.z - lastPos.z);
        float deltaPos = Vector3.Magnitude(deltaV);
        float rapidness = deltaPos / Time.deltaTime;

      
        if(rapidness > 0.1)
        {
            animator.SetBool("Run", true);
            animator.speed = 1.3f * rapidness;
        }else
        {
            animator.speed = 1f;
            animator.SetBool("Run", false);
        }
        lastPos = transform.position;

    }


}
