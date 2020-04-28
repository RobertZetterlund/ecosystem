//#define show_gizmos

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;


public abstract class Animal : Entity, IConsumable
{
	// traits that could be in fcm:
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
	protected double timeToDeathByThirst;
	protected double BiteFactor; // use to calculate how much you eat in one bite
	protected double AdultSizeFactor; // how big you have to be to mate
	protected double lifespan;
	bool dead;
	private bool immobalized;
	[SerializeField]
	public NavMeshAgent navMeshAgent;
	private FCMHandler fcmHandler;
	private string targetGametag = "";
	private ArrayList sensedGameObjects;
	private RangedDouble maxSize;
	private RangedDouble heatTimer; // how many ticks the heat should increase before maxing out
									// senses
	private TickTimer senseTimer, fcmTimer, searchTimer, overallTimer;
	private AbstractSensor[] sensors;
	private AbstractSensor touchSensor;
	private RangedDouble sightLength;
	private RangedDouble smellRadius;
	private float horisontalFOV = 120;
	private float verticalFOV = 90;
	public GameObject targetGameObject;
	private Transform currentTargetTransform;
	private Memory memory;
	private SenseProcessor senseProcessor;
	private double biggestSenseRadius = 0;

	private double currentSpeed = 0;

	// ui
	private StatusBars statusBars;
	private Component[] childRenderers;
	//Debugging
	public bool showFCMGizmo = true;
	public bool showSenseRadiusGizmo;
	public bool showSightGizmo = false;
	public bool showSmellGizmo = false;
	public bool showTouchGizmo = true;
	public bool showTargetDestinationGizmo = true;
	UnityEngine.Color SphereGizmoColor = new UnityEngine.Color(1, 1, 0, 0.3f);
	public Vector3 targetDestinationGizmo = new Vector3(0, 0, 0);
	// trait copy for easier logging etc
	private AnimalTraits traits;
	//animation
	private Vector3 lastPos;
	protected Animator animator;
	protected float runAnimationspeedFactor = 1f;

	//Fitnesss
	private float timeAtBirth;

	// Raycast debuging
	public bool drawRaycast;
	public bool allRaycastHits;

	public float cdt;
	public float overallCostFactor;

	private AbstractAction goToFoodAction, goToWaterAction, goToMateAction, idleAction, action, escapeAction;
	protected SimulationController simulation = SimulationController.Instance();

	public virtual void Init(AnimalTraits traits, double size, double thirst)
	{
		this.species = traits.species;
		this.dietFactor = traits.dietFactor;
		this.maxSize = traits.maxSize;
		this.size = new RangedDouble(0, 0, traits.maxSize.GetValue());
		this.size.Add(size);
		this.nChildren = traits.nChildren;
		this.speed = traits.speed;
		this.fcmHandler = traits.fcmHandler;
		isMale = rand.NextDouble() >= 0.5;
		this.heatTimer = traits.heatTimer;
		this.sightLength = traits.sightLength;
		this.smellRadius = traits.smellRadius;
		this.thirst.Add(thirst);

		this.traits = traits;
		senseProcessor = new SenseProcessor(this, traits.diet, traits.foes, traits.mates);

		biggestSenseRadius = Math.Max(sightLength.GetValue(), smellRadius.GetValue());
		// drar en riktigt cheeky här...

		if (!traits.diet[0].Equals("Plant"))
			goToFoodAction = new HuntAction<Animal>(this);
		else
			goToFoodAction = new GoToConsumable<Plant>(this);

		goToWaterAction = new GoToWater(this);
		goToMateAction = new GoToMate(this);
		idleAction = new IdleAction(this);
		action = idleAction;
		escapeAction = new EscapeAction(this);

		targetGameObject = null;
		gameObject.tag = species.ToString();

		timeAtBirth = Time.time;
		SimulationController.Instance().Register(this);
	}

	// Start is called before the first frame update
	protected virtual void Start()
	{
		cdt = simulation.settings.cdt;
		memory = new Memory();

		navMeshAgent = gameObject.AddComponent(typeof(NavMeshAgent)) as NavMeshAgent;
		SyncGameSpeed();
		navMeshAgent.angularSpeed = 10000;
		navMeshAgent.acceleration = 10000;
		// calculate instead if possible
		navMeshAgent.baseOffset = OrganismFactory.GetOffset(species);

		gameObject.layer = 8;
		lastPos = transform.position;


		sensors = new AbstractSensor[2];
		sensors[0] = SensorFactory.SmellSensor((float)smellRadius.GetValue());
		sensors[1] = SensorFactory.SightSensor((float)sightLength.GetValue(), horisontalFOV, verticalFOV);
		touchSensor = SensorFactory.TouchSensor(1);

		senseTimer = new TickTimer(1f);
		fcmTimer = new TickTimer(0.5f);
		// update ui and visual traits
		UnityEngine.Object prefab = Resources.Load("statusCanvas");
		GameObject canvas = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
		statusBars = canvas.GetComponent(typeof(StatusBars)) as StatusBars;
		childRenderers = GetComponentsInChildren<Renderer>();
		statusBars.Init((float)AdultSizeFactor);
		//UpdateStatusBars();

		animator = ComponentNavigator.GetAnimator(this.gameObject);
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		//check if the animal is dead
		if (isDead())
			return;

		currentSpeed = navMeshAgent.velocity.magnitude;
		DepleteSize();
		UpdateSize();

		// update thirst
		thirst.Add(cdt / timeToDeathByThirst);

		//Punishment for overeating
		if(size.GetValue() / maxSize.GetValue() > 0.98 || thirst.GetValue() < 0.02)
		{
			//energy -= cdt / lifespan;
		}
		energy -= cdt / lifespan;

		if (immobalized)
		{
			// return before executing actions and moving etc...
			return;
		}

		heat.Add(cdt / heatTimer.GetValue());


		// can only mate if in heat and fully grown
		isFertile = heat.GetValue() == 1 && size.GetValue() / maxSize.GetValue() >= AdultSizeFactor;

		senseTimer.Tick();
		if (senseTimer.IsDone())
		{
			Sense();
			senseTimer.Reset();
		}
		fcmTimer.Tick();
		if (fcmTimer.IsDone())
		{
			fcmHandler.ProcessAnimal(thirst.GetValue(), energy, dietFactor.GetValue(),
				isMale, nChildren.GetValue(), size.GetValue(), speed.GetValue(), isFertile, maxSize.GetValue());
			fcmHandler.CalculateFCM();
			fcmTimer.Reset();
		}

		ChooseNextAction();
		if (currentAction != EntityAction.Idle)
			action.Execute();

		Move();
		
	}



	void Move()
	{
		NavMeshAgent agent = navMeshAgent;
		if (Time.timeScale > 1.0f && agent.hasPath)
		{
			NavMeshHit hit;
			float maxAgentTravelDistance = Time.deltaTime * agent.speed;

			//If at the end of path, stop agent.
			if (
				agent.SamplePathPosition(NavMesh.AllAreas, maxAgentTravelDistance, out hit) ||
				agent.remainingDistance <= agent.stoppingDistance
			)
			{
				agent.SetDestination(transform.position);
			}
			//Else, move the actor and manually update the agent pos
			else
			{
				transform.position = hit.position;
				agent.nextPosition = transform.position;
			}
		}
	}
	/*
		void Move()
		{
			var oldPos = transform.position;
			for (var f = 0f; f < 1.0f; f += Time.fixedDeltaTime)
			{
				navMeshAgent.transform.position = Vector3.Lerp(oldPos, navMeshAgent.nextPosition, f);
			}
		}
		*/

	void Update()
	{
		if (simulation.settings.gameSpeed <= 1)
		{
			UpdateStatusBars();
			UpdateAnimation();
		}
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

	public bool CloseEnoughToAct(GameObject gameObject)
	{
		return touchSensor.IsSensingObject(transform, gameObject);
	}

	public void Eat(IConsumable target)
	{
		Consume(target);
	}


	public bool isDead()
	{
		if (thirst.GetValue() >= 1)
		{
			Die(CauseOfDeath.Thirst);
		}
		else if (energy <= 0)
		{
			//Debug.Log("Dead by age");
			Die(CauseOfDeath.Age);
		}
		else if (size.GetValue() == 0)
		{
			Die(CauseOfDeath.Hunger); // or eaten
		}
		else
		{
			return false;
		}
		return true;
	}

	public void Die(CauseOfDeath cause)
	{
		Debug.Log("Dead by " + cause);
		if (!dead)
		{
			//Debug.Log("Death by: " + cause.ToString() + "   Time alive: " + GetTimeAlive());
			dead = true;
			StopAllCoroutines();
			SimulationController.Instance().Unregister(this, cause);
			statusBars.Destroy();
			Destroy(gameObject);
		}

	}

	public void ChooseNextAction()
	{
		EntityAction newAction = fcmHandler.GetAction();
		if (currentAction != newAction)
		{
			currentAction = newAction;
			//This has to reset somewhere
			targetGameObject = null;
			switch (currentAction)
			{
				case EntityAction.GoingToWater:
					targetGameObject = memory.ReadWaterFromMemory();
					action = goToWaterAction;
					break;
				case EntityAction.GoingToFood:
					targetGameObject = memory.ReadFoodFromMemory();
					action = goToFoodAction;
					break;
				case EntityAction.SearchingForMate:
					targetGameObject = memory.ReadMateFromMemory();
					action = goToMateAction;
					break;
				case EntityAction.Escaping:
					targetGameObject = memory.ReadFoeFromMemory();
					action = escapeAction;
					break;
				default:
					currentAction = EntityAction.Idle;
					action = idleAction;
					break;
			}
			action.Reset();
		}
	}

	public bool isCriticallyThirsty()
	{
		return thirst.GetValue() < 0.1; //change these values when we know more or avoid hardcoded values
	}


	public bool isCriticallyHungry()
	{
		return size.GetValue() / maxSize.GetValue() < 0.1; //change these values when we know more or avoid hardcoded values
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
			// mak babi
			//state = ActionState.Reproducing;
			if (true || (size.GetValue() / maxSize.GetValue() < 0.3 && thirst.GetValue() < 0.6))
			{
				if (true || energy > 0.4)
				{
					mate.heat.Add(-1);
					mate.isFertile = false;
					heat.Add(-1);
					isFertile = false;
					//make #nChildren children
					Animal mother = isMale ? mate : this;

					double nbrChildren = mother.nChildren.GetValue();
					double oddsOfExtraChild = nbrChildren - Math.Truncate(nbrChildren);
					nbrChildren = MathUtility.RandomChance(oddsOfExtraChild) ? Math.Truncate(nbrChildren) + 1 : Math.Truncate(nbrChildren);

					// calculate size for each parent and child
					double individualSize = (size.GetValue() + mate.size.GetValue()) / (2 + nbrChildren);
					size.SetValue(0);
					mate.size.SetValue(0);
					size.Add(individualSize);
					mate.size.Add(individualSize);

					// calculate thirst for each parent and child
					double individualThirst = 1 - ((1 - thirst.GetValue()) + (1 - mate.thirst.GetValue())) / (2 + nbrChildren);
					thirst.SetValue(0);
					mate.thirst.SetValue(0);
					thirst.Add(individualThirst);
					mate.thirst.Add(individualThirst);

					for (int i = 0; i < nbrChildren; i++)
					{
						AnimalTraits child = ReproductionUtility.ReproduceAnimal(traits, mate.traits);
						Animal childAnimal = OrganismFactory.CreateAnimal(child, mother.transform.position, individualSize, individualThirst);
						SimulationController.Instance().RegisterBirth(species);
					}
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
	[MethodImpl(MethodImplOptions.Synchronized)]
	private void Consume(IConsumable consumable)
	{
		// do eating calculations
		if (consumable != null)
		{
			ConsumptionType type = consumable.GetConsumptionType();

			// determine how much you CAN eat 
			double biteSize = maxSize.GetValue() * BiteFactor;
			double effectiveAmount = 0;

			switch (type)
			{
				case ConsumptionType.Water:
					effectiveAmount = biteSize;
					break;
				case ConsumptionType.Animal:
					effectiveAmount = biteSize * dietFactor.GetValue();
					break;
				case ConsumptionType.Plant:
					effectiveAmount = biteSize * (1 - dietFactor.GetValue());
					break;
			}

			// determine how much you can fit in your belly
			double maxAmountPossible = maxSize.GetValue() - size.GetValue();
			if ( maxAmountPossible - effectiveAmount < 0)
				effectiveAmount = maxAmountPossible;

			// determine how much THE TARGET has
			double amount = consumable.Consume(effectiveAmount);

			// swallow
			switch (type)
			{
				case ConsumptionType.Water:
					thirst.Add(amount);
					break;
				case ConsumptionType.Animal:
					size.Add(effectiveAmount);
					break;
				case ConsumptionType.Plant:
					size.Add(effectiveAmount);
					break;
			}
		}

	}

	// eat this animal
	public double Consume(double amount)
	{
		/*
		Immobalize();
		double eaten = size.Add(-amount);
		return eaten;*/
		double eaten = size.Add(-1000000);
		return eaten;
	}

	// swallow the food/water that this animal ate

	public void GoToStationaryPosition(Vector3 pos)
	{
		NavMeshPath path = new NavMeshPath();
		bool canPath = false;
		try
		{
			canPath = navMeshAgent.CalculatePath(pos, path);
		}
		catch (Exception)
		{
		}


		if (path.status == NavMeshPathStatus.PathComplete && canPath)
		{
			SetDestination(pos);
		}
		else
		{
			NavMeshHit myNavHit;
			if (NavMesh.SamplePosition(pos, out myNavHit, 100, -1))
			{
				SetDestination(myNavHit.position);
			}
		}
	}

	public void SetDestination(Vector3 pos)
	{
		NavMeshHit myNavHit;
		if (NavMesh.SamplePosition(pos, out myNavHit, 100, NavMesh.AllAreas))
		{
			//navMeshAgent.ResetPath();

			navMeshAgent.SetDestination(myNavHit.position);
			targetDestinationGizmo = myNavHit.position;
		}
	}


#if show_gizmos
	void OnDrawGizmos()
	{
		if (showTouchGizmo)
		{
			var pos11 = transform.position + Quaternion.AngleAxis(0, transform.up) * transform.forward * touchSensor.GetRadius();

			var prev1 = pos11;
			for (int i = 20; i <= 360; i += 20)
			{
				var newpos1 = transform.position + Quaternion.AngleAxis(i, transform.up) * transform.forward * touchSensor.GetRadius();
				Gizmos.DrawLine(prev1, newpos1);
				prev1 = newpos1;
			}

		}

		if (drawRaycast)
		{



			foreach (Vector3 vec in ((AreaSensor)sensors[0]).pointList)
			{
				Gizmos.color = UnityEngine.Color.gray;
				Gizmos.DrawSphere(vec, 0.05f);
				Gizmos.color = UnityEngine.Color.white;
				Gizmos.DrawLine(gameObject.transform.position, vec);
			}

			foreach (Vector3 vec in ((AreaSensor)sensors[0]).wrongHitList)
			{
				Gizmos.color = UnityEngine.Color.red;
				Gizmos.DrawLine(gameObject.transform.position, vec);
			}
			foreach (Vector3 vec in ((AreaSensor)sensors[0]).rightHitList)
			{
				Gizmos.color = UnityEngine.Color.blue;
				Gizmos.DrawLine(gameObject.transform.position, vec);

			}

			if (allRaycastHits)
			{
				foreach (Vector3 vec in ((AreaSensor)sensors[0]).hitList)
				{
					Gizmos.color = UnityEngine.Color.green;
					Gizmos.DrawLine(gameObject.transform.position, vec);
				}
			}

			Gizmos.color = UnityEngine.Color.white;
		}

		/*
		if (showSenseRadiusGizmo)
		{
			Gizmos.color = SphereGizmoColor;
			Gizmos.DrawSphere(transform.position, senseRadius);
		}
		*/
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
		Handles.Label(transform.position + new Vector3(0, 3, 0), currentAction.ToString() + "   " + action.GetState());
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

#endif

	public NavMeshAgent GetNavMeshAgent()
	{
		return navMeshAgent;
	}

	private static void Water_Sort(Vector3[] arr, int left, int right, Vector3 pos)
	{
		if (left < right)
		{
			int pivot = Partition(arr, left, right, pos);
			if (pivot > 1)
			{
				Water_Sort(arr, left, pivot - 1, pos);
			}
			if (pivot + 1 < right)
			{
				Water_Sort(arr, pivot + 1, right, pos);
			}
		}

	}

	private static int Partition(Vector3[] arr, int left, int right, Vector3 pos)
	{
		Vector3 pivot = arr[left];
		while (true)
		{
			while ((pos - arr[left]).sqrMagnitude < (pos - pivot).sqrMagnitude)
			{
				left++;
			}
			while ((pos - arr[right]).sqrMagnitude > (pos - pivot).sqrMagnitude)
			{
				right--;
			}
			if (left < right)
			{
				if (arr[left] == arr[right]) return right;

				Vector3 temp = arr[left];
				arr[left] = arr[right];
				arr[right] = temp;
			}
			else
			{
				return right;
			}
		}
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
		float sizeRadius = 0;
		float baseRadius = navMeshAgent.radius * 1.1f;
		if (gameObject != null && (float)size.GetValue() > 0.01)
		{
			try
			{
				sizeRadius = (float)Math.Pow(size.GetValue(), 1f / 3f);
				gameObject.transform.localScale = OrganismFactory.GetOriginalScale(species) * (float)size.GetValue();
				//navMeshAgent.radius = 0.5f * radiusF;
			}
			catch (Exception)
			{

			}

		}
		float radius = (sizeRadius > baseRadius) ? sizeRadius : baseRadius;
		touchSensor.SetRadius(radius);
	}

	// update position and value of status bars
	private void UpdateStatusBars()
	{
		statusBars.UpdateStatus((float)(size.GetValue() / maxSize.GetValue()), (float)thirst.GetValue(), (float)energy, (float)heat.GetValue());
		statusBars.gameObject.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
		// set position of status bars
		Renderer rend = (Renderer)childRenderers[0]; // take the first one
		Vector3 center = rend.bounds.center;
		float radius = rend.bounds.extents.magnitude;
		Vector3[] corners = new Vector3[4];
		statusBars.gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
		float height = corners[1].y - corners[0].y;
		statusBars.gameObject.transform.position = center + new Vector3(0, radius / 2, 0) + new Vector3(0f, height / 2, 0f);
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

	private void DepleteSize()
	{
		double sizeCost = Math.Pow(size.GetValue(), 2f / 3f); // surface area heat radiation
		double speedCost = currentSpeed * size.GetValue(); // mass * speed
		double smellCost = smellRadius.GetValue() * 2; // times 2 because it is more op than sight
		double sightCost = sightLength.GetValue() * horisontalFOV / 360;
		// each cost is divided by some arbitrary constant to balance it

		sizeCost /= 120;
		speedCost /= 3200;
		smellCost /= 5000;
		sightCost /= 5000;

		// deplete size based on traits.
		double depletion = overallCostFactor * cdt * (sizeCost + speedCost + smellCost + sightCost);
		//Debug.Log(" size0 " + size.GetValue() + " depletion "+ depletion + " size " + Time.deltaTime * sizeCost + " speed " + Time.deltaTime * speedCost + " smell " + Time.deltaTime * smellCost + " sight " + Time.deltaTime * sightCost );
		size.Add(-depletion);

	}

	public AnimalTraits GetTraits()
	{
		return traits;
	}

	public float GetTimeAlive()
	{
		return (Time.time - timeAtBirth) * Time.timeScale;
	}

	public void Kill()
	{

	}

	// return true if first call
	protected virtual bool Immobalize()
	{
		if (!immobalized)
		{
			SetDestination(transform.position);
			isFertile = false;
			immobalized = true;
			return true;
		}
		return false;
	}

	public double GetSpeed()
	{
		return currentSpeed;
	}

	public double GetMaxSpeed()
	{
		return speed.GetValue();
	}

	public double GetSenseRadius()
	{
		return biggestSenseRadius;
	}

	public void SyncGameSpeed()
	{
		navMeshAgent.speed = (float)(speed.GetValue() * simulation.settings.gameSpeed);
	}

}
