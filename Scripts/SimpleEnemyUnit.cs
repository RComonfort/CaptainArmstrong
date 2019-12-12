using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyUnit : MonoBehaviour, IDamageable
{
	[SerializeField] private int maxHealth = 1;
	[SerializeField] private GameObject deathFXPrefab;
	[SerializeField] private int damage = 1;

	public bool indestructible = false;
	private bool isDead = false;
	private int hp;

	protected virtual void Start() 
	{
		hp = maxHealth;
	}
	
    public bool TakeDamage(int amount)
	{
		if (isDead || indestructible)
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
		
		if (isDead)
			return;

		IDamageable damageableObj = other.gameObject.GetComponent<IDamageable>();

		if (damageableObj != null)
		{
			damageableObj.TakeDamage(damage);
		}
	}
}
