using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipComponent : MonoBehaviour, IPickable
{
    [SerializeField] private EShipComponent type;
	[SerializeField] private GameObject pickupFXPrefab;
	
	private bool pickedUp = false;

	public void Pickup(Player player)
	{
		if (pickedUp)
			return;

		//Dont pickup if could not add the component (due to it being maxed out)
		if (!player.AddSpaceshipComponent(type))	
			return;

		pickedUp = true;

		if (pickupFXPrefab)
			Instantiate(pickupFXPrefab, transform.position, Quaternion.identity);

		Destroy(gameObject);
	}
}

public interface IPickable {
	
	void Pickup(Player player);
}
