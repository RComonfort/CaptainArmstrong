using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ForwardMovementComponent))]
public class Ship : MonoBehaviour, IRideable
{
    private Player player;
	[SerializeField] private Sprite fixedShipSprite;
	
	private Sprite destroyedShipSprite;
	private ForwardMovementComponent movementComponent;
	private SpriteRenderer spriteRenderer;

	protected void Start() {

		movementComponent = GetComponent<ForwardMovementComponent>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
}
