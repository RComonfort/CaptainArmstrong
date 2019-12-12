﻿using System.Collections;
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
		hitbox.enabled = false;
	}

	public void StopBeingRidden()
	{
		hitbox.enabled = true;
		player = null;
		CancelRotation();
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
