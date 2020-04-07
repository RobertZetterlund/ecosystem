using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public abstract class Animal : Entity, IConsumable
{
    // traits that could be in fcm:
    private RangedDouble hunger = new RangedDouble(1, 0, 1);
    private RangedDouble thirst = new RangedDouble(0, 0, 1);
    private double energy = 1;
    private RangedDouble dietFactor; // 1 = carnivore, 0.5 = omnivore, 0 = herbivore
    public bool isMale;
    private RangedDouble nChildren; // how many kids you will have
    private RangedDouble speed;
    public bool isFertile;
    // internal traits
    private static System.Random rand = MathUtility.random;
    protected EntityAction currentAction = EntityAction.Idle;
    protected ActionState state = new ActionState();
    private RangedDouble heat = new RangedDouble(0, 0, 1); // aka fuq-o-meter
    double bellySize = 700; // basically how slow hunger depletes.
    double timeToDeathByThirst = 100;
    private static double BITE_FACTOR = 50; // use to calculate how much you eat in one bite
    double lifespan = 200;
    bool dead;
    public NavMeshAgent navMeshAgent;
    private FCMHandler fcmHandler;
    private string targetGametag = "";
    private ArrayList sensedGameObjects;
    private RangedDouble maxSize;
    private RangedDouble infantFactor; // how big the child is in %
    private RangedDouble growthFactor; // how much you grow each tick
    private RangedDouble heatTimer; // how many ticks the heat should increase before maxing out
    // senses
    private Timer senseTimer, fcmTimer;
    private AbstractSensor[] sensors;
    private AbstractSensor touchSensor;
    private RangedDouble sightLength;
    private RangedDouble smellRadius;
    private float horisontalFOV = 120;
    private float verticalFOV = 90;
    protected GameObject targetGameObject;
    private Transform currentTargetTransform;
    private Memory memory;
    private SenseProcessor senseProcessor;

    // ui
    private StatusBars statusBars;
    private Component[] childRenderers;
    //Debugging
    public bool showFCMGizmo = true;
    public bool showSenseRadiusGizmo;
    public bool showSightGizmo = false;
    public bool showSmellGizmo = false;
    public bool showTargetDestinationGizmo = false;
    UnityEngine.Color SphereGizmoColor = new UnityEngine.Color(1, 1, 0, 0.3f);
    Vector3 targetDestinationGizmo = new Vector3(0, 0, 0);
    // trait copy for easier logging etc
    private AnimalTraits traits;
    private bool logNext = false;
    //animation
    private Vector3 lastPos;
    protected Animator animator;
    protected float runAnimationspeedFactor = 1f;

    //Fitnesss
    private float timeAtBirth;

    // Raycast debuging
    public bool drawRaycast;
    public bool allRaycastHits;

    public virtual void Init(AnimalTraits traits)
    {
        this.species = traits.species;
        this.dietFactor = traits.dietFactor;
        this.maxSize = traits.maxSize;
        this.size = new RangedDouble(traits.maxSize.GetValue()*traits.infantFactor.GetValue(), 0, traits.maxSize.GetValue());
        this.nChildren = traits.nChildren;
        this.infantFactor = traits.infantFactor;
        this.growthFactor = traits.growthFactor;
        this.speed = traits.speed;
        this.fcmHandler = traits.fcmHandler;
        isMale = rand.NextDouble() >= 0.5;
        this.heatTimer = traits.heatTimer;
        this.sightLength = traits.sightLength;
        this.smellRadius = traits.smellRadius;

        this.traits = traits;
        senseProcessor = new SenseProcessor(this, traits.diet, traits.foes, traits.mates);

        targetGameObject = null;
        gameObject.tag = species.ToString();

        timeAtBirth = Time.time;
        SimulationController.Instance().Register(this);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {

        memory = new Memory();

        navMeshAgent = gameObject.AddComponent(typeof(NavMeshAgent)) as NavMeshAgent;
        navMeshAgent.speed = (float)speed.GetValue();
        // calculate instead if possible
        navMeshAgent.baseOffset = OrganismFactory.GetOffset(species);

        gameObject.layer = 8;
        lastPos = transform.position;
        

        sensors = new AbstractSensor[2];
        sensors[0] = SensorFactory.SightSensor((float)sightLength.GetValue(), horisontalFOV, verticalFOV);
        sensors[1] = SensorFactory.SmellSensor((float)smellRadius.GetValue());
        touchSensor = SensorFactory.TouchSensor(1);

        senseTimer = new Timer(0.25f);
        senseTimer.Start();
        fcmTimer = new Timer(0.25f);
        fcmTimer.Start();

        // update ui and visual traits
        UnityEngine.Object prefab = Resources.Load("statusCanvas");
        GameObject canvas = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        statusBars = canvas.GetComponent(typeof(StatusBars)) as StatusBars;
        childRenderers = GetComponentsInChildren<Renderer>();
        UpdateSize();
        UpdateStatusBars();


        animator = ComponentNavigator.GetAnimator(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        DepleteHungerAndSize();


        // update thirst
        thirst.Add(Time.deltaTime * 1 / timeToDeathByThirst);

        //age the animal
        energy -= Time.deltaTime * 1 / lifespan;

        heat.Add(Time.deltaTime / heatTimer.GetValue());

        // can only mate if in heat and fully grown
        isFertile = heat.GetValue() == 1 && size.GetValue() == maxSize.GetValue();


        if (senseTimer.IsDone())
        {
            Sense();
            senseTimer.Reset();
            senseTimer.Start();
        }
        if (fcmTimer.IsDone())
        {
            fcmHandler.ProcessAnimal(hunger.GetValue(), thirst.GetValue(), energy, dietFactor.GetValue(), 
                isMale, nChildren.GetValue(), size.GetValue(), speed.GetValue(), isFertile);
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
        isDead();

        //Animation

        UpdateAnimation();

    }

    void Sense()
    {
        sensedGameObjects = new ArrayList();
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
        
            targetGameObject = memory.GetTargObj(currentAction);
        
            IDictionary<string, int> impactMap = sE.GetWeightMap();


        fcmHandler.ProcessSensedObjects(this, sE);
    }

    protected bool CloseEnoughToAct(GameObject gameObject)
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

    protected bool Act(IConsumable currentTarget)
    {
        switch (currentAction)
        {
            case EntityAction.GoingToFood:
                Eat(currentTarget);
                return true;
                break;
            case EntityAction.GoingToWater:
                Drink(currentTarget);
                return true;
                break;
            case EntityAction.SearchingForMate:
                Animal mate = (Animal)currentTarget;
                return Reproduce(mate);
                break;
            default:
                return false;
        }
    }
    private void Drink(IConsumable target)
    {
        Consume(target);
    }

    public void Eat(IConsumable target)
    {
        Consume(target);
    }

    void LateUpdate()
    {
        logNext = TraitLogger.logNext;
    }

    public void isDead()
    {
        // wont die whe nhunger runs out rn, only when size runs out
        if (false && hunger.GetValue() >= 1)
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
        else if (size.GetValue() == 0)
        {
            Die(CauseOfDeath.Eaten);
        }
    }

    public void Die(CauseOfDeath cause)
    {
        if (!dead)
        {
            Debug.Log("Death by: " + cause.ToString());
            dead = true;
            StopAllCoroutines();
            SimulationController.Instance().Unregister(this);
            statusBars.Destroy();
            Destroy(gameObject);
        }

    }

    public void chooseNextAction()
    {
        CheckCurrentAction(fcmHandler.GetAction());
    }

    private void CheckCurrentAction(EntityAction newAction)
    {
        if (currentAction != newAction)
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
                case EntityAction.Escaping:
                    targetGameObject = memory.ReadFoeFromMemory();
                    StartCoroutine(Escape());
                    break;
                default:
                    currentAction = EntityAction.Idle;
                    break;

                case EntityAction.SearchingForMate:
                    StartCoroutine(GoToMate());
                    break;
            }
        }
        else
        {
            if (!GameObjectExists(targetGameObject))
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

        foreach (GameObject gameObject in sensedGameObjects)
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

    public bool Reproduce(Animal mate)
    {
        try
        {
            // checked if can mate
            if (species != mate.species || // if different species
                !(isMale ^ mate.isMale) || // if same sex
                !isFertile || !mate.isFertile) // if not fertile
            {
                //currentAction = EntityAction.Idle; // Set action to idle when done
                return false;
            }
            Debug.Log("mating");
            // mak babi
            state = ActionState.Reproducing;
            if (true || (hunger.GetValue() < 0.3 && thirst.GetValue() < 0.6))
            {
                if (true || energy > 0.4)
                {
                    mate.heat.Add(-1);
                    mate.isFertile = false;
                    heat.Add(-1);
                    isFertile = false;
                    //make #nChildren children
                    Animal mother = isMale ? mate : this;

                    double children = mother.nChildren.GetValue();
                    double oddsOfExtraChild = children - Math.Truncate(children);
                    children = MathUtility.RandomChance(oddsOfExtraChild) ? children + 1 : children;
                    for (int i = 0; i < children; i++)
                    {
                        double maxSize = ReproductionUtility.ReproduceRangedDouble(this.maxSize.Duplicate(), mate.maxSize.Duplicate()).GetValue();

                        // deplete hunger for each child born
                        // stop when your hunger would run out
                        // if: hunger.Add(maxSize * this.infantFactor.GetValue()) != maxSize * this.infantFactor.GetValue()
                        double sizeRemoved = mother.size.Add(-maxSize * mother.infantFactor.GetValue());
                        if (sizeRemoved != -maxSize * mother.infantFactor.GetValue())
                        {
                            mother.size.Add(-sizeRemoved); // restore because child wasnt born.
                            return false;
                        }
                        double dietFactor = ReproductionUtility.ReproduceRangedDouble(this.dietFactor.Duplicate(), mate.dietFactor.Duplicate()).GetValue();
                        double nChildren = ReproductionUtility.ReproduceRangedDouble(this.nChildren.Duplicate(), mate.nChildren.Duplicate()).GetValue();
                        double infantFactor = ReproductionUtility.ReproduceRangedDouble(this.infantFactor.Duplicate(), mate.infantFactor.Duplicate()).GetValue();
                        double growthFactor = ReproductionUtility.ReproduceRangedDouble(this.growthFactor.Duplicate(), mate.growthFactor.Duplicate()).GetValue();
                        double speed = ReproductionUtility.ReproduceRangedDouble(this.speed.Duplicate(), mate.speed.Duplicate()).GetValue();
                        double heatTimer = ReproductionUtility.ReproduceRangedDouble(this.heatTimer.Duplicate(), mate.heatTimer.Duplicate()).GetValue();
                        FCMHandler fcmHandler = this.fcmHandler.Reproduce(mate.fcmHandler);


                        AnimalTraits child = new AnimalTraits(species, maxSize, dietFactor, nChildren, infantFactor, growthFactor, speed, heatTimer, sightLength.GetValue(), smellRadius.GetValue(), fcmHandler, traits.diet, traits.foes, traits.mates);

                        //child.fcmHandler = new RabbitFCMHandler(FCMFactory.RabbitFCM());
                        OrganismFactory.CreateAnimal(child, mother.transform.position);
                    }
                    mother.UpdateSize();
                }
            }
        }
        catch (MissingReferenceException)
        {
            // mate died lol
        }
        //currentAction = EntityAction.Idle; // Set action to idle when done
        return true;
    }

    // let this animal attempt to take a bite from the given consumable
    private void Consume(IConsumable consumable)
    {
        Debug.LogWarning("eating");
        // do eating calculations
        if (consumable != null)
        {
            double biteSize = Time.deltaTime * size.GetValue() * BITE_FACTOR;
            ConsumptionType type = consumable.GetConsumptionType();
            swallow(consumable.Consume(biteSize), type);
        }

    }

    // eat this animal
    public double Consume(double amount)
    {
        double eaten = size.Add(-amount);
        UpdateSize();
        return eaten;
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

    public void SetDestination(Vector3 pos)
    {
        NavMeshHit myNavHit;
        if (NavMesh.SamplePosition(pos, out myNavHit, 100, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(myNavHit.position);
        }
    }


    //Draws a sphere corresponding to its sense radius
    void OnDrawGizmos()
    {

        if(drawRaycast) {
            
            
            
            foreach(Vector3 vec in ((AreaSensor)sensors[0]).pointList) {
                Gizmos.color = UnityEngine.Color.gray;
                Gizmos.DrawSphere(vec, 0.05f);
                Gizmos.color = UnityEngine.Color.white;
                Gizmos.DrawLine(gameObject.transform.position, vec);
            }

            foreach(Vector3 vec in ((AreaSensor)sensors[0]).wrongHitList) {
                Gizmos.color = UnityEngine.Color.red;
                Gizmos.DrawLine(gameObject.transform.position, vec);
            }
            foreach(Vector3 vec in ((AreaSensor)sensors[0]).rightHitList) {
                Gizmos.color = UnityEngine.Color.blue;
                Gizmos.DrawLine(gameObject.transform.position, vec);

            }

            if(allRaycastHits) {
                foreach(Vector3 vec in ((AreaSensor)sensors[0]).hitList) {
                    Gizmos.color = UnityEngine.Color.green;
                    Gizmos.DrawLine(gameObject.transform.position, vec);
                }
            }

            Gizmos.color = UnityEngine.Color.white;
        }

        /*if (showSenseRadiusGizmo)
        {
            Gizmos.color = SphereGizmoColor;
            Gizmos.DrawSphere(transform.position, senseRadius);
        }*/

        if (showSightGizmo)
        {
            float hFOV = horisontalFOV;
            //float vFOV = verticalFOV;
            var pos1 = transform.position + Quaternion.AngleAxis(hFOV / 2, transform.up) * transform.forward * (float)sightLength.GetValue();
            //var pos2 = transform.position + Quaternion.AngleAxis(vFOV / 2, transform.right) * transform.forward * radius;
            var pos2 = transform.position + Quaternion.AngleAxis(-hFOV / 2, transform.up) * transform.forward * (float)sightLength.GetValue();
            //var pos4 = transform.position + Quaternion.AngleAxis(-vFOV / 2, transform.right) * transform.forward * radius;

            Gizmos.DrawLine(transform.position, pos1);
            Gizmos.DrawLine(transform.position, pos2);

            var prev = pos2;
            int start = (int)(-hFOV / 2);
            int end = (int)(hFOV / 2);
            for (int i = start; i <= end; i += 10)
            {
                var newpos = transform.position + Quaternion.AngleAxis(i, transform.up) * transform.forward * (float)sightLength.GetValue();
                Gizmos.DrawLine(prev, newpos);
                prev = newpos;
            }
        }
        if (showSmellGizmo)
        {
            var pos1 = transform.position + Quaternion.AngleAxis(0, transform.up) * transform.forward * (float)smellRadius.GetValue();

            var prev = pos1;
            for (int i = 20; i <= 360; i += 20)
            {
                var newpos = transform.position + Quaternion.AngleAxis(i, transform.up) * transform.forward * (float)smellRadius.GetValue();
                Gizmos.DrawLine(prev, newpos);
                prev = newpos;
            }
        }
        if (showTargetDestinationGizmo)
        {
            Gizmos.DrawLine(transform.position, targetDestinationGizmo);
        }
        Handles.Label(transform.position + new Vector3(0, 3, 0), currentAction.ToString() + "   " + state.ToString());
    }
    void OnDrawGizmosSelected()
    {
        if (showFCMGizmo)
        {
            Vector3 textOffset = new Vector3(-3, 2, 0);
            Handles.Label(transform.position + textOffset, currentAction.ToString());
            textOffset = new Vector3(1, 2, 0);
            if (fcmHandler != null)
                Handles.Label(transform.position + textOffset, fcmHandler.GetFCMData());
        }
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return navMeshAgent;
    }

    public IEnumerator GoToStationaryConsumable(ConsumptionType consumptionType, Vector3 position)
    {
        yield return StartCoroutine(Approach(targetGameObject, position));
        if(!(targetGameObject == null)) { 
        yield return StartCoroutine(EatConsumable(consumptionType));
        }
    }

    public virtual IEnumerator EatConsumable(ConsumptionType consumptionType)
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
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1);
            if (consumable == null || consumable.GetAmount() == 0)
                break;
            Eat(consumable); // take one bite
            
        }
        state = ActionState.Idle;
        yield return null;
    }

    public virtual IEnumerator GoToFood()
    {
        state = ActionState.GoingToFood;
        string gametag = ConsumptionType.Plant.ToString();
        Vector3 pos = new Vector3(0,0,0);
        bool retry;
        do
        {
            yield return StartCoroutine(Search(gametag));
            try
            {
                pos = targetGameObject.transform.position;
                retry = false;
            }
            catch (MissingReferenceException)
            {
                retry = true;
            }
        } while (retry);
        yield return StartCoroutine(GoToStationaryConsumable(ConsumptionType.Plant, pos));
        state = ActionState.Idle;
        currentAction = EntityAction.Idle;
    }

    public virtual IEnumerator Approach(GameObject targetGameObject, Vector3 position)
    {
        
        state = ActionState.Approaching;

        while (targetGameObject != null && !CloseEnoughToAct(targetGameObject))
        {
            yield return new WaitForSeconds(0.2f);
            if (targetGameObject != null)
            {
               
                SetDestination(position);

            }
                
        }
        // To prevent the animal from not going further than necessary to perform its action.
        // I wanted to use the stop function of the NavMeshAgent but if one does use that one also
        // has to resume the movement when you want the animal to walk again, so I did it this way instead.
        SetDestination(transform.position);
        yield return null;
    }

    public IEnumerator GoToMate()
    {
        Animal mate = null;
        bool retry = true;
        do
        {
            yield return StartCoroutine(SearchAndApproachMate());
            try
            {
                mate = targetGameObject.GetComponent<Animal>();
                
                // if mate wasnt fertile, search for new
                if (Act(mate))
                {
                    retry = false;
                }
            }
            catch (MissingReferenceException)
            {
                // if mate died, search for new
                retry = true;
            }
        } while (retry);

        yield return new WaitForSeconds(1);
        currentAction = EntityAction.Idle;
        yield return null;
    }

    // search and approach moving mate
    public IEnumerator SearchAndApproachMate()
    {
        // search
        yield return StartCoroutine(Search(species.ToString()));

        // check if valid mate
        while (targetGameObject != null && !CloseEnoughToAct(targetGameObject))
        {
            yield return new WaitForSeconds(0.2f);
            if (targetGameObject != null)
            {
                // approach
                state = ActionState.Approaching;
                SetDestination(targetGameObject.transform.position);
            }
        }
        // done 
        SetDestination(transform.position);
        yield return null;
    }

    public IEnumerator GoToWater()
    {
        state = ActionState.GoingToWater;
        string gametag = ConsumptionType.Water.ToString();
        yield return StartCoroutine(Search(gametag));
    
        MeshFilter mesh = (MeshFilter) targetGameObject.GetComponent(typeof(MeshFilter));
        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;
        // scan all vertices to find nearest
        foreach (Vector3 vertex in mesh.sharedMesh.vertices)
        {
            Vector3 diff = transform.position - vertex;
            float distSqr = diff.sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                nearestVertex = vertex;
            }
        }
        yield return StartCoroutine(GoToStationaryConsumable(ConsumptionType.Water, nearestVertex));

        state = ActionState.Idle;
        currentAction = EntityAction.Idle;
    }

    /*public IEnumerator GoToPartner()
    {
        yield return StartCoroutine(GoToMate());
    }*/

    

    public Vector3 EscapeAnimal(Vector3 targetPos)
    {
        Vector3 dir = transform.position - targetPos;
        float angle = Vector3.SignedAngle(dir, Vector3.forward, Vector3.up);
        Vector3 new_directon = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * angle), 0, Mathf.Cos(Mathf.Deg2Rad * angle));

        
        Vector3 myNewPos = transform.position + new_directon*10;
        
        
        return myNewPos;
        

    }

    private IEnumerator Escape()
    {
        state = ActionState.Escaping;
        while (targetGameObject != null)
        {

            GoToStationaryPosition(EscapeAnimal(targetGameObject.transform.position));
            yield return new WaitForSeconds(1);
        }
        
        state = ActionState.Idle;
        currentAction = EntityAction.Idle;
        yield return null;

    }

    private void GoToStationaryPosition(Vector3 pos)
    {
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
    }

    public void Roam()
    {
        Vector3 pos = ChooseNewDestination();
        GoToStationaryPosition(pos);
    }

    public IEnumerator Search(string gametag)
    {
        Debug.Log(gametag);
        state = ActionState.Searching;
        targetGametag = gametag;
        //Make it search before actually walking, since it otherwise might walk away from a plant
        //and then walk right back to it.
        Sense();
        senseTimer.Reset();
        senseTimer.Start();
        while (targetGameObject == null)
        {
            Roam();
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
        gameObject.transform.localScale = OrganismFactory.GetOriginalScale(species) * (float)size.GetValue();

        Renderer rend = (Renderer)childRenderers[0];
        float radius = rend.bounds.extents.magnitude;
        touchSensor.setRadius(1 + radius * 1.1f);
        //statusBars.transform.localScale = StatusBars.scale;
    }

    // update position and value of status bars
    private void UpdateStatusBars()
    {
        statusBars.UpdateStatus((float)hunger.GetValue(), (float)thirst.GetValue(), (float)energy, (float)heat.GetValue());
        statusBars.gameObject.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
        // set position of status bars
        Renderer rend = (Renderer)childRenderers[0]; // take the first one
        Vector3 center = rend.bounds.center;
        float radius = rend.bounds.extents.magnitude;
        Vector3[] corners = new Vector3[4];
        statusBars.gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
        float height = corners[1].y - corners[0].y;
        statusBars.gameObject.transform.position = center + new Vector3(0, radius/2, 0) + new Vector3(0f,height/2,0f);
        statusBars.transform.localScale = StatusBars.scale;
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


        if (rapidness > 0.1)
        {
            animator.SetBool("Run", true);
            animator.speed = runAnimationspeedFactor * rapidness;
        }
        else
        {
            animator.speed = 1f;
            animator.SetBool("Run", false);
        }
        lastPos = transform.position;

    }

    /*
     * Deplete hunger and size depending on traits
     * First hunger will be depleted, if hunger ran out, deplete size also
     * Else, increase size and deplete hunger further
     */
    private void DepleteHungerAndSize()
    {
        double sizeToHungerFactor = bellySize * 1/1000;
        double constantHunger = maxSize.GetValue() / 20;
        double smellCost = smellRadius.GetValue() * horisontalFOV / 360 / 20 * 2; // 2 cuz op
        double sightCost = sightLength.GetValue() / 20;

        // calculate size growth
        double growth = 0;
        if (size.GetValue() < maxSize.GetValue()) // if not fully grown
        {
            growth = Time.deltaTime * size.GetValue() * growthFactor.GetValue(); // used to be maxSize
        }
        // deplete hunger based on traits.
        double depletion = Time.deltaTime / bellySize * ((size.GetValue() + constantHunger) * speed.GetValue() + smellCost + sightCost);
        double depleted = hunger.Add(depletion);
        // if hunger ran out, deplete size also
        if (depletion != depleted)
        {
            size.Add((depleted - depletion)/sizeToHungerFactor);
        }
        else // else, increase size according to grwoth until hunger runs out
        {
            size.Add(hunger.Add(growth/sizeToHungerFactor)*sizeToHungerFactor); // grow until hunger runs out
        }
        UpdateSize();
    }

    public AnimalTraits GetTraits()
    {
        return traits;
    }

    public float GetTimeAlive()
    {
        return Time.time - timeAtBirth;
    }

}
