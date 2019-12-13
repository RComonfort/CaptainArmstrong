using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : SimpleEnemyUnit, IRideable
{
	[SerializeField] private Player player;

	private Collider2D hitbox;
	private ForwardMovementComponent movementComponent;

	protected override void Start() {
		base.Start();

		hitbox = GetComponent<Collider2D>();
		movementComponent = GetComponent<ForwardMovementComponent>();
	}

	public void GetsRidden(Player by)
	{
		player = by;
		gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");
		indestructible = true;
	}

	public void StopBeingRidden()
	{
		AddTemporalInvunerability(player, 1f);

		player.AddTemporalInvunerability(this, 1f);

		CancelRotation();
		indestructible = false;
		gameObject.layer = LayerMask.NameToLayer("Default");

		player = null;
	}

	public void Rotate(float angle)
	{
		movementComponent.AddRotationAngles(angle);
	}

	public void CancelRotation()
	{
		movementComponent?.CancelRotation();
	}
}
