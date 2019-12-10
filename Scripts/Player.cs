using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	
	[SerializeField] private float angleStep = 5f;		//The angles that are rotated with each step
	[SerializeField] public bool allowMovementInput = true;

	private float lastAngleStep = 0f;			//When was the last angle delta added
	private float angleStepCD = 0.2f;			//Time in secs that must be waited before rotating again
	private ForwardMovementComponent movementComp;
	[HideInInspector] public EPlayerState playerState;

    // Start is called before the first frame update
    void Start()
    {
        movementComp = GetComponentInParent<ForwardMovementComponent>();

		playerState = EPlayerState.OnComet;
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
			}
		}

		//Action 2 Do clockwise rotation
        if (action2Input > 0.2f)
		{
			DoRotationStep(angleStep);
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
}
