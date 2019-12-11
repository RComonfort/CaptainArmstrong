using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ForwardMovementComponent : MonoBehaviour
{
	[SerializeField] private float movementForce = 100f;
    [SerializeField] private float turnSpeed = 72f; 
	public EMovementEntityType type {get; private set;} = EMovementEntityType.Comet;

	private Rigidbody2D rb;
	private Quaternion targetRot;

    // Start is called before the first frame update
    void Start()
    {
		rb = GetComponent<Rigidbody2D>();

        targetRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void AddRotationAngles(float angles)
	{
		targetRot *= Quaternion.Euler(Vector3.forward * -angles);
	}

	public void CancelRotation()
	{
		targetRot = transform.rotation;
	}

	private void FixedUpdate() {
		
		//Rotate only if the current rot and target rot are different enough
		float theta = Quaternion.Angle(transform.rotation, targetRot);
		if (!Mathf.Approximately(theta, 0f))
		{
			//The angles to rotate this frame
			float angleDelta = turnSpeed * Time.fixedDeltaTime;

			float angleAlpha = angleDelta / theta;
			Quaternion newRot = Quaternion.Slerp(transform.rotation, targetRot, angleAlpha);

        	rb.MoveRotation(newRot);
		}
			
        //Move forwards
        rb.AddForce(transform.right * movementForce * Time.fixedDeltaTime);
	}
}
