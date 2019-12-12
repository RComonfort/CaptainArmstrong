using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HeartContainer : MonoBehaviour, IPickable
{
    [SerializeField] private int healingAmount = 1;
	[SerializeField] private float initialMoveForce = 50f;
	
	[SerializeField] private GameObject pickupFXPrefab;
	
	private bool pickedUp = false;

	private void Start() {
		//Create random vector at XY plane
		float randRad = Random.Range(0f, Mathf.PI * 2);
		Vector3 dir = new Vector3(Mathf.Cos(randRad), Mathf.Sin(randRad), 0f);

		//Push the heart container
		GetComponent<Rigidbody2D>().AddForce(dir * initialMoveForce);
	}

	public void Pickup(Player player)
	{
		if (pickedUp)
			return;

		player.TakeDamage(-healingAmount);

		Instantiate(pickupFXPrefab, transform.position, Quaternion.identity);

		Destroy(this);
	}
}
