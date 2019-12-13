using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FinalDestination : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
		Player player = other.gameObject.GetComponent<Player>();

		if (player)
		{
			player.Escape();
		}
	}
}
