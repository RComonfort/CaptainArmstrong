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

		player.AddSpaceshipComponent(type);

		Instantiate(pickupFXPrefab, transform.position, Quaternion.identity);

		Destroy(this);
	}
}

public interface IPickable {
	
	void Pickup(Player player);
}
