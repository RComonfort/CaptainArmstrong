using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxMover : MonoBehaviour
{
	[SerializeField] private float speedFactor = 2f;
	
	private Player player;
	private Vector3 velocity;
	

    // Start is called before the first frame update
    void Start()
    {
        player = Object.FindObjectOfType<Player>();
    }

    void FixedUpdate()
    {
		if (player)
		{
			Rigidbody2D playerRB = player.transform.root.GetComponentInChildren<Rigidbody2D>();
			velocity = playerRB.velocity / speedFactor;

			transform.Translate(velocity * Time.fixedDeltaTime, Space.World);
		}
		else
		{
			Destroy(this);
		}
        
    }
}
