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
		GetComponent<Rigidbody2D>().AddForce(dir * initialMoveForce * Random.Range(.85f, 1.1f), ForceMode2D.Impulse);
	}

	public void Pickup(Player player)
	{
		if (pickedUp)
			return;

		//Only destroy if player could be healed by the container
		if (!player.HealDamage(healingAmount))
			return;

		pickedUp = true;

		if (pickupFXPrefab)
			Instantiate(pickupFXPrefab, transform.position, Quaternion.identity);

		Destroy(gameObject);
	}
}
