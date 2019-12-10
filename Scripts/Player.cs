using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	
	[SerializeField] private float angleStep = 5f;		//The angles that are rotated with each step
	[SerializeField] public bool allowMovementInput = true;
	[SerializeField] private float cometJumpRadius = 10;
	

	private float lastAngleStep = 0f;			//When was the last angle delta added
	private float angleStepCD = 0.05f;			//Time in secs that must be waited before rotating again
	private bool bIsRotatingCW, bIsRotatingCCW;

	private ForwardMovementComponent movementComp; //Movement component from the ship or comet that the player is on
	private Transform targetJumpingPosition; 
	[HideInInspector] public EPlayerState playerState;
	private List<Transform> nearbyComets;
	private CircleCollider2D cometDetectZone;


    // Start is called before the first frame update
    void Start()
    {
        movementComp = GetComponentInParent<ForwardMovementComponent>();

		playerState = EPlayerState.OnComet;

		nearbyComets = new List<Transform>();
		
		cometDetectZone = gameObject.AddComponent<CircleCollider2D>();
		cometDetectZone.isTrigger = true;
		cometDetectZone.radius = cometJumpRadius;
    }

    // Update is called once per frame
    void Update()
    {
		if (allowMovementInput && playerState != EPlayerState.Jumping)
		{
			MovementInputUpdate();
		}
		
    }

	private void MovementInputUpdate(){
		
		float action1Input = Input.GetAxisRaw("Action1");
		float action2Input = Input.GetAxisRaw("Action2");

		//Action 1
		if (action1Input > 0.2f)
		{
			//If in a comet (jump)
			if (playerState == EPlayerState.OnComet)
			{

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
		movementComp?.CancelRotation();


	}
}
