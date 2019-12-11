using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ITriggerListener
{
	[Header("Player")]
	[SerializeField] public bool allowMovementInput = true;

	[Header("Rotation")]
	[SerializeField] private float angleStep = 5f;		//The angles that are rotated with each step
	
	[Header("Jumping")]
	[SerializeField] private float cometJumpRadius = 10;	//Max radius at which the player can jump to other comet
	[SerializeField] private float jumpingSpeed = 10;		//The speed (in units/sec) at which the player jumps from comet to comet

	private float lastAngleStep = 0f;			//When was the last angle delta added
	private float angleStepCD = 0.05f;			//Time in secs that must be waited before rotating again
	private bool bIsRotatingCW, bIsRotatingCCW;

	private ForwardMovementComponent movementComp; //Movement component from the ship or comet that the player is on
	private Transform targetJumpingPosition; 
	[HideInInspector] public EPlayerState playerState;
	private HashSet<Transform> nearbyComets;
	private Transform nearestComet;
	private CircleCollider2D DetectionTrigger;


    // Start is called before the first frame update
    void Start()
    {
        movementComp = GetComponentInParent<ForwardMovementComponent>();

		playerState = EPlayerState.OnComet;

		nearbyComets = new HashSet<Transform>();
		
		DetectionTrigger = GetComponentInChildren<Trigger2DRelay>()?.triggerCollider as CircleCollider2D;
		DetectionTrigger.radius = cometJumpRadius;
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

		string names = "";
		while (em.MoveNext()) { 
            float sqrDist = (em.Current.position - transform.position).sqrMagnitude;

			if (sqrDist < minSqrDist)
			{
				minSqrDist = sqrDist;
				nearestComet = em.Current;
			}

			names += ", " + em.Current.name;
        } 

		print("[" + names + "], near: " + nearestComet.name);
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

#region TRIGGER_LISTENER

	public void OnObjectEnteredTrigger(Trigger2DRelay triggerObj, Collider2D other)
	{
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
}
