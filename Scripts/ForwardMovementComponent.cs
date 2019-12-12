using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ForwardMovementComponent : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] private float movementForce = 100f;
	[SerializeField] private float torque = 20f;
	[SerializeField] private float maxVelocityMagnitude = 5000;
    [SerializeField] private float turnSpeed = 72f; 

	[Header("Movement Component")]
	[SerializeField] private EMovementEntityType _type = EMovementEntityType.Comet;

	private Rigidbody2D rb;
	private Quaternion targetRot;
	public EMovementEntityType type {get {return _type;} private set {_type = value;}}
	

    // Start is called before the first frame update
    void Start()
    {
		rb = GetComponent<Rigidbody2D>();

        targetRot = transform.rotation;

		if (type == EMovementEntityType.DebrisAdrift)
		{
			rb.AddForce(transform.forward * movementForce, ForceMode2D.Impulse);
			rb.AddTorque(torque, ForceMode2D.Impulse);
		}
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
		
		if (type == EMovementEntityType.DebrisAdrift )
			return;

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
		if (rb.velocity.magnitude < maxVelocityMagnitude)
        	rb.AddForce(transform.right * movementForce * Time.fixedDeltaTime);
	}
}
