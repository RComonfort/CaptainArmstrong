using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ForwardMovementComponent : MonoBehaviour
{
	[SerializeField] private float acceleration = 10f;
    [SerializeField] private float turnSpeed = 360; 
	
	private Rigidbody2D rb;
	private float anglesToRotate = 0f;

	private void Awake() {
		rb = GetComponent<Rigidbody2D>();
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void AddRotationAngles(float angles)
	{
		anglesToRotate = angles;
	}

	private void FixedUpdate() {
		
		//Rotate towards dir
		if (!Mathf.Approximately(anglesToRotate, 0f))
		{
			float angleDelta = turnSpeed * Time.fixedDeltaTime;

			anglesToRotate = Mathf.Sign(anglesToRotate) * Mathf.Abs(anglesToRotate) - angleDelta;

			Quaternion targetRot = Quaternion.AngleAxis(anglesToRotate, Vector3.forward);
        	Quaternion newRot = Quaternion.RotateTowards(targetRot, transform.rotation, angleDelta);

        	rb.MoveRotation(newRot);
		}
			
        //Move forwards
        rb.AddForce(transform.forward * acceleration * Time.fixedDeltaTime);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		
	}
}
