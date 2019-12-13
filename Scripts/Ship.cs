﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ForwardMovementComponent))]
[RequireComponent(typeof(BoxCollider2D))]
public class Ship : MonoBehaviour, IRideable
{
    private Player player;
	[SerializeField] private Sprite fixedShipSprite;
	
	private Sprite destroyedShipSprite;
	private ForwardMovementComponent movementComponent;
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D collision; 

	protected void Start() {

		movementComponent = GetComponent<ForwardMovementComponent>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		collision = GetComponent<BoxCollider2D>();
	}

	public void GetsRidden(Player by)
	{
		player = by;
		destroyedShipSprite = spriteRenderer.sprite;
		spriteRenderer.sprite = fixedShipSprite;
	}

	public void StopBeingRidden()
	{
		player = null;
		CancelRotation();
		spriteRenderer.sprite = destroyedShipSprite;
	}

	public void Rotate(float angle)
	{
		movementComponent.AddRotationAngles(angle);
	}

	public void CancelRotation()
	{
		movementComponent?.CancelRotation();
	}

	private void OnCollisionEnter2D(Collision2D other) {
		
		if (this.player) //We are already controlled by player
		{	
			//relay collision message to him
			player.OnCollisionEnter2D(other);
		}
		else //Do not have a player yet
		{
			Player player = other.gameObject.GetComponent<Player>();

			//Only let player board if he can repair ship
			if (player && player.TotalNeededComps() == player.TotalObtainedComps())
			{
				player.BoardShip(this);
			}
		}
		
		
	}
}
