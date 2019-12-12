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
	
    public void TakeDamage(int amount)
	{
		if (isDead)
			return;

		//If player is a child of this object, ignore damage
		if (GetComponentInChildren<Player>())
			return;

		hp -= amount;
		if (hp <= 0)
			Die();
	}

	public void Die()
	{
		isDead = true;

		//Play FX
		Instantiate(deathFXPrefab, transform.position, Quaternion.identity);
	}

	private void OnCollisionEnter2D(Collision2D other) {
		
		IDamageable damageableObj = other.gameObject.GetComponent<IDamageable>();

		if (damageableObj != null)
		{
			damageableObj.TakeDamage(damage);
		}
	}
}
