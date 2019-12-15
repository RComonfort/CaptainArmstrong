using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsPlayer : MonoBehaviour
{
	[SerializeField] private float speed = 5f;
	
	private Player player;

    void Start()
    {
		player = GameObject.FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
		if (player)
			transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }
}
