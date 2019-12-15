using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ForwardMovementComponent))]
[RequireComponent(typeof(BoxCollider2D))]
public class Ship : MonoBehaviour, IRideable, IDamageable, IDamageDealer
{
    private Player player;
	[SerializeField] private Sprite fixedShipSprite;
	[SerializeField] private GameObject shipDestroyedFX;
	[SerializeField] private GameObject shipBoardedFX;
	
	
	private Sprite destroyedShipSprite;
	private ForwardMovementComponent movementComponent;
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D collision; 
	private AudioSource shipNoise;

	protected void Start() {

		movementComponent = GetComponent<ForwardMovementComponent>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		collision = GetComponent<BoxCollider2D>();
		shipNoise = GetComponent<AudioSource>();
	}

	public void GetsRidden(Player by)
	{
		player = by;
		movementComponent.enabled = true;
		destroyedShipSprite = spriteRenderer.sprite;
		spriteRenderer.sprite = fixedShipSprite;

		if (shipBoardedFX)
			Instantiate(shipBoardedFX, transform.position, Quaternion.identity);

		shipNoise.Play();
	}

	public void StopBeingRidden()
	{
		player = null;
		CancelRotation();

		shipNoise.Stop();

		if (shipDestroyedFX)
			Instantiate(shipDestroyedFX, transform.position, Quaternion.identity);

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

	public bool TakeDamage(int amount, IDamageDealer instigator)
	{
		if (!player)
			return false;
			
		return player.TakeDamage(amount, instigator);
	}

	public void Die()
	{

	}

	public void DealDamage(int amount, IDamageable entity)
	{

	}

	public bool HealDamage(int amount)
	{
		if (!player)
			return false;

		return player.HealDamage(amount);
	}

	public void AddTemporalInvunerability(IDamageDealer forEntity, float duration)
	{
		player.AddTemporalInvunerability(forEntity, duration);
	}

	public void RemoveTemporalInvunerability(IDamageDealer forEntity)
	{
		player.RemoveTemporalInvunerability(forEntity);
	}
}
