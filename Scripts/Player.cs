using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ITriggerListener, IDamageable
{
	[Header("Player")]
	[SerializeField] public bool allowMovementInput = true;

	[Header("Rotation")]
	[SerializeField] private float angleStep = 5f;		//The angles that are rotated with each step
	
	[Header("Jumping")]
	[SerializeField] private float cometJumpRadius = 10;	//Max radius at which the player can jump to other comet
	[SerializeField] private float jumpingSpeed = 10;		//The speed (in units/sec) at which the player jumps from comet to comet

	[Header("Health")]
	[SerializeField] private int maxHealth = 3;
	[SerializeField] private int damageOnContact = 1;
	

	[Header("Ship Repairing")]
	[SerializeField] private RequiredComponent[] requiredComponents;
	

	private float lastAngleStep = 0f;			//When was the last angle delta added
	private float angleStepCD = 0.05f;			//Time in secs that must be waited before rotating again
	private bool bIsRotatingCW, bIsRotatingCCW;
	public int hp {get; private set;}
	public bool canBeDamaged = true;

	public int MaxHealth {get {return maxHealth;}}

	private ForwardMovementComponent movementComp; //Movement component from the ship or comet that the player is on
	private Transform targetJumpingPosition; 
	[HideInInspector] public EPlayerState playerState;
	private HashSet<Transform> nearbyComets;
	private Transform nearestComet;
	private Vector3 initialPosOffset;
	private Quaternion initialRotOffset;
	private Dictionary<EShipComponent, int> obtainedComps;
	private Dictionary<EShipComponent, int> neededComps;

    // Start is called before the first frame update
    void Start()
    {
        movementComp = GetComponentInParent<ForwardMovementComponent>();

		playerState = EPlayerState.OnComet;

		nearbyComets = new HashSet<Transform>();
		
		CircleCollider2D DetectionTrigger = GetComponentInChildren<Trigger2DRelay>()?.triggerCollider as CircleCollider2D;
		DetectionTrigger.radius = cometJumpRadius;

		initialPosOffset = transform.localPosition;
		initialRotOffset = transform.localRotation;

		hp = maxHealth;

		//Ship repaired init
		neededComps = new Dictionary<EShipComponent, int>();
		obtainedComps = new Dictionary<EShipComponent, int>();
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
    }

	private void CometTrackingUpdate()
	{
		if (nearbyComets.Count == 0)
		{
			nearestComet = null;
			return;
		}

		//Iterate through comets to find closest
		float minSqrDist = float.MaxValue;
		HashSet<Transform>.Enumerator em = nearbyComets.GetEnumerator(); 

		while (em.MoveNext()) { 
            float sqrDist = (em.Current.position - transform.position).sqrMagnitude;

			if (sqrDist < minSqrDist)
			{
				minSqrDist = sqrDist;
				nearestComet = em.Current;
			}
        } 
		
	}

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
			movementComp?.CancelRotation();
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
			movementComp?.CancelRotation();
			bIsRotatingCW = false;
		}
	}

	private void DoRotationStep(float value)
	{
		if (Time.time > lastAngleStep + angleStepCD)
		{
			lastAngleStep = Time.time;
			movementComp?.AddRotationAngles(angleStep);
		}
	}

	public void JumpToNearestCommet()
	{
		if (playerState != EPlayerState.OnComet || !nearestComet)
			return;

		movementComp?.CancelRotation();
		
		//Add ridden comet as nearby comet
		nearbyComets.Add(movementComp.transform);

		//Dettach from comet
		movementComp = null;
		transform.parent = null;

		//Set state to jumping and start jumping process
		playerState = EPlayerState.Jumping;
		StartCoroutine(JumpingRoutine(nearestComet));
	}

	private IEnumerator JumpingRoutine(Transform cometTarget)
	{
		//Remove new ridden comet from nearby comets
		nearbyComets.Remove(cometTarget);
		nearestComet = null;

		while (transform.position != cometTarget.position)
		{
			transform.position = Vector3.MoveTowards(transform.position, cometTarget.position, jumpingSpeed * Time.deltaTime);
			yield return null;
		}

		//New comet reached, update player state
		movementComp = cometTarget.GetComponent<ForwardMovementComponent>();
		playerState = EPlayerState.OnComet;
		transform.SetParent(cometTarget);

		//Apply offsets
		transform.localPosition = initialPosOffset;
		transform.localRotation = initialRotOffset;
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

	private bool RemoveFractionOfObtainedComps()
	{
		int compsToRemove = TotalObtainedComps() / maxHealth;
		
		if (compsToRemove == 0)
			return false;

		int leftToRemove = compsToRemove;
		for (int i = 0; i < requiredComponents.Length && leftToRemove > 0; i++)
		{
			int removeFromComp = leftToRemove / (requiredComponents.Length - i);

			EShipComponent comp = requiredComponents[i].component;
			removeFromComp = removeFromComp > obtainedComps[comp] ? obtainedComps[comp] : removeFromComp;
			obtainedComps[comp] -= removeFromComp;

			leftToRemove -= removeFromComp;

			StartCoroutine(DropGrabbedComponents(comp, removeFromComp));
		}

		return true;
	}

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
			float angle = (360f / amount) * i;
			Quaternion rot = Quaternion.AngleAxis(angle, Vector3.right);

			droppedComps[i] = Instantiate(template, transform.position, rot);
		}

		//Animate scale increase
		float scale = 0f;

		while (scale < 1f)
		{
			foreach (GameObject obj in droppedComps)
			{
				obj.transform.localScale = new Vector3(scale, scale);
			}

			scale = Mathf.Clamp01(scale + 1.5f * Time.deltaTime);

			yield return null;
		}
		
	}

	public int TotalObtainedComps()
	{
		Dictionary<EShipComponent, int>.Enumerator e = obtainedComps.GetEnumerator();

		int total = 0;
		while (e.MoveNext()) { 
            total += e.Current.Value;
        } 

		return total;
	}

	public void AddSpaceshipComponent(EShipComponent type)
	{
		obtainedComps[type] = Mathf.Clamp(obtainedComps[type] + 1, 0, neededComps[type]);
	}

	private void OnCollisionEnter2D(Collision2D other) {
		
		IDamageable damageableObj = other.gameObject.GetComponent<IDamageable>();

		if (damageableObj != null)
		{
			//Ignore the comet we are riding
			if (playerState == EPlayerState.OnComet && other.transform.root == transform.root)
				return;

			damageableObj.TakeDamage(damageOnContact);
		}

		IPickable pickup = other.gameObject.GetComponent<IPickable>();
		if (pickup != null)
			pickup.Pickup(this);
	}

#region TRIGGER_LISTENER

	public void OnObjectEnteredTrigger(Trigger2DRelay triggerObj, Collider2D other)
	{
		if (playerState != EPlayerState.OnComet)
			return;
			
		//Add collided object if it is a comet that is not already considered
		ForwardMovementComponent movingObj = other.gameObject.GetComponent<ForwardMovementComponent>();
		if (movingObj && movingObj.type == EMovementEntityType.Comet && !nearbyComets.Contains(movingObj.transform))
			nearbyComets.Add(movingObj.transform);
	}

	public void OnObjectExitedTrigger(Trigger2DRelay triggerObj, Collider2D other)
	{
		nearbyComets.Remove(other.transform);

		if (nearbyComets.Count == 0)
			nearestComet = null;
	}

	public void OnObjectStayedTrigger(Trigger2DRelay triggerObj, Collider2D other)
	{

	}
#endregion

#region DAMAGEABLE

	public void TakeDamage(int amount)
	{
		if (!canBeDamaged)
			return;

		//If cannot drop grabbed components, take damage instead
		if (!RemoveFractionOfObtainedComps())
		{
			hp = Mathf.Clamp(hp - amount, 0, maxHealth);

			if (hp <= 0)
			{
				Die();
				return;
			}
		}
		
		//Play hurt animation

	}

	public void Die()
	{

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
