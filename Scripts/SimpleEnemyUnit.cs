using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyUnit : MonoBehaviour, IDamageable
{
	[SerializeField] private int maxHealth = 1;
	[SerializeField] private GameObject deathFXPrefab;
	[SerializeField] private int damage = 1;

	private bool isDead = false;
	private int hp;

	private void Start() {
		hp = maxHealth;
	}
	
    public bool TakeDamage(int amount)
	{
		if (isDead)
			return false;

		//If player is a child of this object, ignore damage
		if (GetComponentInChildren<Player>())
			return false;

		hp = Mathf.Clamp(hp - amount, 0, maxHealth);
		if (hp == 0)
			Die();

		return true;
	}

	public bool HealDamage(int amount)
	{
		if (hp == maxHealth)
			return false;

		hp = Mathf.Clamp(hp + amount, 0, maxHealth);

		return true;
	}

	public void Die()
	{
		isDead = true;

		//Play FX
		if (deathFXPrefab)
			Instantiate(deathFXPrefab, transform.position, Quaternion.identity);

		Destroy(gameObject);
	}

	private void OnCollisionEnter2D(Collision2D other) {
		
		IDamageable damageableObj = other.gameObject.GetComponent<IDamageable>();

		if (damageableObj != null)
		{
			damageableObj.TakeDamage(damage);
		}
	}
}
