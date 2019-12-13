using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ITriggerListener, IDamageable, IDamageDealer
{
	[Header("Player")]
	[SerializeField] public bool allowMovementInput = true;

	[Header("Rotation")]
	[SerializeField] private float angleStep = 5f;      //The angles that are rotated with each step

	[Header("Jumping")]
	[SerializeField] private float cometJumpRadius = 10;    //Max radius at which the player can jump to other comet
	[SerializeField] private float jumpingSpeed = 10;       //The speed (in units/sec) at which the player jumps from comet to comet

	[Header("Health")]
	[SerializeField] private int maxHealth = 3;
	[SerializeField] private int damageOnContact = 1;	

	[Header("Ship Repairing")]
	[SerializeField] private RequiredComponent[] requiredComponents;

	//Events
	public event System.Action playerDeathEvent;
	public event System.Action playerBoardedShipEvent;
	public event System.Action playerReachedExitEvent;

	private float lastAngleStep = 0f;           //When was the last angle delta added
	private float angleStepCD = 0.05f;          //Time in secs that must be waited before rotating again
	private bool bIsRotatingCW, bIsRotatingCCW;
	public int hp { get; private set; }
	public bool canBeDamaged = true;

	//Accessors
	public int MaxHealth { get { return maxHealth; } }
	public RequiredComponent[] RequiredComponents { get { return requiredComponents; } }
	public Dictionary<EShipComponent, int> ObtainedComps { get { return obtainedComps; } }
	public Dictionary<EShipComponent, int> NeededComps { get { return neededComps; } }

	private IRideable riddenObj;
	private Transform targetJumpingPosition;
	[HideInInspector] public EPlayerState playerState;
	private HashSet<Comet> nearbyComets;
	private Comet nearestComet;
	private Vector3 initialPosOffset;
	private Quaternion initialRotOffset;
	private LineRenderer nearestCometLine;
	private Dictionary<EShipComponent, int> obtainedComps;
	private Dictionary<EShipComponent, int> neededComps;
	private HashSet<IDamageDealer> cannotBeDamagedBy;


	#region MonoBehaviourMethods

	private void Awake() 
	{
		hp = maxHealth;
		playerState = EPlayerState.OnComet;

		nearbyComets = new HashSet<Comet>();
		neededComps = new Dictionary<EShipComponent, int>();
		obtainedComps = new Dictionary<EShipComponent, int>();

		cannotBeDamagedBy = new HashSet<IDamageDealer>();

		initialPosOffset = transform.localPosition;
		initialRotOffset = transform.localRotation;
	}

	// Start is called before the first frame update
	void Start()
	{
		riddenObj = GetComponentInParent<IRideable>();
		riddenObj.GetsRidden(this);
		
		nearestCometLine = GetComponent<LineRenderer>();

		CircleCollider2D DetectionTrigger = GetComponentInChildren<Trigger2DRelay>()?.triggerCollider as CircleCollider2D;
		DetectionTrigger.radius = cometJumpRadius;

		//Ship repaired init

		for (int i = 0; i < requiredComponents.Length; i++)
		{
			neededComps.Add(requiredComponents[i].component, requiredComponents[i].amount);
			obtainedComps.Add(requiredComponents[i].component, 0);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (allowMovementInput && playerState != EPlayerState.Jumping)
		{
			MovementInputUpdate();
		}

		//Update nearest comet
		CometTrackingUpdate();

		DrawNearestComet();
	}

	private void OnDrawGizmosSelected()
	{
		//Draw jump radius when selected
		if (playerState == EPlayerState.OnComet)
		{
			Gizmos.color = new Color(1, .92f, .016f, .8f);
			Gizmos.DrawWireSphere(transform.position, cometJumpRadius);
		}
	}


	public void OnCollisionEnter2D(Collision2D other)
	{
		if (playerState == EPlayerState.Dead)
			return;

		//Only deal damage when on comet or spaceship
		IDamageable damageableObj = other.gameObject.GetComponent<IDamageable>();
		if (damageableObj != null)
		{
			//Ignore the comet we are riding
			if (playerState == EPlayerState.OnComet && other.transform.root == transform.root)
				return;

			DealDamage(damageOnContact, damageableObj);
		}

		IPickable pickup = other.gameObject.GetComponent<IPickable>();
		if (pickup != null)
			pickup.Pickup(this);
	}

	#endregion


	#region CometJumping

	private void CometTrackingUpdate()
	{
		List<Comet> foundNull = new List<Comet>();

		nearestComet = null;

		//Iterate through comets to find closest
		float minSqrDist = float.MaxValue;
		HashSet<Comet>.Enumerator em = nearbyComets.GetEnumerator();

		while (em.MoveNext())
		{
			if (!em.Current)
			{
				foundNull.Add(em.Current);
				continue;
			}

			float sqrDist = (em.Current.transform.position - transform.position).sqrMagnitude;

			if (sqrDist < minSqrDist)
			{
				minSqrDist = sqrDist;
				nearestComet = em.Current;
			}
		}

		foreach (Comet nullRef in foundNull)
			nearbyComets.Remove(nullRef);
	}

	private void DrawNearestComet()
	{
		if (playerState != EPlayerState.OnComet || !nearestComet)
			nearestCometLine.enabled = false;
		else
		{
			nearestCometLine.enabled = true;
			nearestCometLine.SetPositions(new Vector3[]{transform.position, nearestComet.transform.position});
		}
	}

	public void JumpToNearestCommet()
	{
		if (playerState != EPlayerState.OnComet || !nearestComet)
			return;

		//Add ridden comet as nearby comet
		nearbyComets.Add(riddenObj as Comet);

		//Dettach from comet
		riddenObj.StopBeingRidden();
		riddenObj = null;
		transform.parent = null;

		//print("Jumping from " + gameObject.name + " to "+ nearestComet.name +",PS: " + playerState + ", nearByC: " + nearbyComets.ToString());

		//Set state to jumping and start jumping process
		playerState = EPlayerState.Jumping;
		
		StartCoroutine(JumpingRoutine(nearestComet));
	}

	private IEnumerator JumpingRoutine(Comet cometTarget)
	{
		//Remove new ridden comet from nearby comets
		nearbyComets.Remove(cometTarget);

		Transform target = cometTarget.transform;

		riddenObj = cometTarget;
		riddenObj.GetsRidden(this);

		while (Vector3.SqrMagnitude(transform.position - target.position) > .5f)
		{
			transform.position = Vector3.MoveTowards(transform.position, target.position, jumpingSpeed * Time.deltaTime);
			yield return null;
		}

		//New comet reached, update player state
		playerState = EPlayerState.OnComet;
		transform.SetParent(target);

		//Apply offsets
		transform.localPosition = initialPosOffset;
		transform.localRotation = initialRotOffset;
	}

	#endregion

	#region Movement
	private void MovementInputUpdate()
	{
		float action1Input = Input.GetAxisRaw("Action1");
		float action2Input = Input.GetAxisRaw("Action2");

		//Action 1
		if (action1Input > 0.2f)
		{
			//If in a comet (jump)
			if (playerState == EPlayerState.OnComet)
			{
				JumpToNearestCommet();
			}
			//if in spaceship, rotate ccw
			else if (playerState == EPlayerState.OnSpaceShip)
			{
				DoRotationStep(-angleStep);
				bIsRotatingCCW = true;
			}
		}
		else if (bIsRotatingCCW)
		{
			riddenObj?.CancelRotation();
			bIsRotatingCCW = false;
		}

		//Action 2 Do clockwise rotation
		if (action2Input > 0.2f)
		{
			DoRotationStep(angleStep);
			bIsRotatingCW = true;
		}
		else if (bIsRotatingCW)
		{
			riddenObj?.CancelRotation();
			bIsRotatingCW = false;
		}
	}

	private void DoRotationStep(float value)
	{
		if (Time.time > lastAngleStep + angleStepCD)
		{
			lastAngleStep = Time.time;
			riddenObj?.Rotate(angleStep);
		}
	}

	public void BoardShip(Ship ship)
	{
		if (playerBoardedShipEvent != null)
			playerBoardedShipEvent();

		riddenObj.StopBeingRidden();

		riddenObj = ship;
		riddenObj.GetsRidden(this);
		transform.SetParent(ship.transform, true);
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;

		playerState = EPlayerState.OnSpaceShip;

		//Remove player's collider and sprite renderer
		Destroy(GetComponent<Collider2D>());
		Destroy(GetComponent<SpriteRenderer>());
	}

	public void Escape()
	{
		if (playerReachedExitEvent != null)
			playerReachedExitEvent();
	}

	#endregion


	#region ShipRepairing

	private IEnumerator DropGrabbedComponents(EShipComponent component, int amount)
	{
		GameObject[] droppedComps = new GameObject[amount];

		//Get the prefab template
		GameObject template = null;
		for (int i = 0; i < requiredComponents.Length; i++)
		{
			if (requiredComponents[i].component == component)
			{
				template = requiredComponents[i].prefab;
				break;
			}
		}

		//Instantiate necessary components
		for (int i = 0; i < amount; i++)
		{
			float rad = Random.Range(0, Mathf.PI * 2);
			Quaternion rot = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)))); 

			droppedComps[i] = Instantiate(template, transform.position, rot);

			//Push object away
			Rigidbody2D rb = droppedComps[i].GetComponent<Rigidbody2D>();
			rb.AddForce(droppedComps[i].transform.right * 7.5f, ForceMode2D.Impulse);
			rb.gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");
			rb.drag = .5f;
		}

		//Animate scale increase
		float scale = 0f;
		while (scale < 1f)
		{
			foreach (GameObject obj in droppedComps)
			{
				obj.transform.localScale = new Vector3(scale, scale);
			}

			scale = Mathf.Clamp01(scale + (1/1.5f * Time.deltaTime));

			yield return null;
		}

		//Reenabled collisions
		foreach (GameObject go in droppedComps)
		{
			go.layer = LayerMask.NameToLayer("Default");
		}
	}

	public int TotalObtainedComps()
	{
		Dictionary<EShipComponent, int>.Enumerator e = obtainedComps.GetEnumerator();

		int total = 0;
		while (e.MoveNext())
		{
			total += e.Current.Value;
		}

		return total;
	}

	public int TotalNeededComps()
	{
		Dictionary<EShipComponent, int>.Enumerator e = neededComps.GetEnumerator();

		int total = 0;
		while (e.MoveNext())
		{
			total += e.Current.Value;
		}

		return total;
	}

	private bool RemoveFractionOfObtainedComps()
	{
		int compsToRemove = TotalObtainedComps() / (maxHealth - 1);

		if (compsToRemove == 0)
			return false;

		int leftToRemove = compsToRemove;
		for (int i = 0; i < requiredComponents.Length && leftToRemove > 0; i++)
		{
			int removeFromComp = leftToRemove / (requiredComponents.Length - i);

			if (removeFromComp == 0)
				removeFromComp = leftToRemove % (requiredComponents.Length - i);

			EShipComponent comp = requiredComponents[i].component;
			removeFromComp = removeFromComp > obtainedComps[comp] ? obtainedComps[comp] : removeFromComp;
			obtainedComps[comp] -= removeFromComp;

			leftToRemove -= removeFromComp;

			StartCoroutine(DropGrabbedComponents(comp, removeFromComp));
		}

		return true;
	}


	//Adds 1 to the type of spaceship component. Returns true if it was added and false if player is already maxed out.
	public bool AddSpaceshipComponent(EShipComponent type)
	{
		if (obtainedComps[type] == neededComps[type])
			return false;

		obtainedComps[type] = obtainedComps[type] + 1;

		return true;
	}


	#endregion


	#region TRIGGER_LISTENER

	public void OnObjectEnteredTrigger(Trigger2DRelay triggerObj, Collider2D other)
	{

		//Add collided object if it is a comet that is not already considered
		Comet movingObj = other.gameObject.GetComponent<Comet>();
		if (movingObj && !nearbyComets.Contains(movingObj))
			nearbyComets.Add(movingObj);
	}

	public void OnObjectExitedTrigger(Trigger2DRelay triggerObj, Collider2D other)
	{
		nearbyComets.Remove(other.GetComponent<Comet>());

		if (nearbyComets.Count == 0)
			nearestComet = null;
	}

	public void OnObjectStayedTrigger(Trigger2DRelay triggerObj, Collider2D other)
	{

	}
	#endregion

	#region DAMAGEABLE/DAMAGEDEALER

	public bool TakeDamage(int amount, IDamageDealer instigator)
	{
		//If invulnerable or dead or cannot be damaged by that entity, return
		if (!canBeDamaged || playerState == EPlayerState.Dead || cannotBeDamagedBy.Contains(instigator))
			return false;

		//If cannot drop grabbed components, take damage instead
		if (!RemoveFractionOfObtainedComps())
		{
			hp = Mathf.Clamp(hp - amount, 0, maxHealth);

			if (hp == 0)
			{
				Die();
				return true;
			}
		}

		//TODO: Play hurt animation

		return true;
	}

	public void Die()
	{
		playerState = EPlayerState.Dead;

		if (playerDeathEvent != null)
			playerDeathEvent();

		allowMovementInput = false;

		riddenObj.StopBeingRidden();

		//TODO: Play Death Animation
	}

	public void DealDamage(int amount, IDamageable entity)
	{
		entity.TakeDamage(damageOnContact, this);
	}

	public bool HealDamage(int amount)
	{
		if (hp == maxHealth)
			return false;

		hp = Mathf.Clamp(hp + amount, 0, maxHealth);

		return true;
	}

	public void AddTemporalInvunerability(IDamageDealer forEntity, float duration)
	{
		cannotBeDamagedBy.Add(forEntity);
		StartCoroutine(RemoveTempInvDispatch(forEntity, duration));
	}

	public void RemoveTemporalInvunerability(IDamageDealer forEntity)
	{
		cannotBeDamagedBy.Remove(forEntity);
	}

	IEnumerator RemoveTempInvDispatch(IDamageDealer forEntity, float duration)
	{
		yield return new WaitForSeconds(duration);
		RemoveTemporalInvunerability(forEntity);
	}

	#endregion
}

[System.Serializable]
public class RequiredComponent
{
	public EShipComponent component;
	public int amount;
	public GameObject prefab;
}
