using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyUnit : MonoBehaviour, IDamageable
{
	[SerializeField] private int maxHealth = 1;
	[SerializeField] private GameObject deathFXPrefab;
	[SerializeField] private int damage = 1;
	[SerializeField] private float damageCDPerObj = 2f; //The time allowed to an object as not targetable after being damaged by this unit
	

	public bool indestructible = false;
	private bool isDead = false;
	private int hp;
	private Dictionary<IDamageable, float> invulnerableObjects;

	protected virtual void Start() 
	{
		hp = maxHealth;
		invulnerableObjects = new Dictionary<IDamageable, float>();
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
			//This object has been damaged before
			if (invulnerableObjects.ContainsKey(damageableObj))
			{
				//If enough time has passed, remove from invulnerable objects
				if (Time.timeSinceLevelLoad > invulnerableObjects[damageableObj] + damageCDPerObj)
				{
					invulnerableObjects.Remove(damageableObj);
				}
				else //not enough time passed, exit function
					return;
			}

			damageableObj.TakeDamage(damage);
			invulnerableObjects.Add(damageableObj, Time.timeSinceLevelLoad);
		}
	}
}
