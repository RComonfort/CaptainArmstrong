using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyUnit : MonoBehaviour, IDamageable, IDamageDealer
{
	[SerializeField] private int maxHealth = 1;
	[SerializeField] private GameObject deathFXPrefab;
	[SerializeField] private int damage = 1;
	[SerializeField] private float damageCDPerObj = 2f; //The time allowed to an object as not targetable after being damaged by this unit
	[SerializeField] private float deathDelay = 0f;
	

	public bool indestructible = false;
	private bool isDead = false;
	private int hp;
	private Dictionary<IDamageable, float> invulnerableObjects;
	private HashSet<IDamageDealer> cannotBeDamagedBy;

	protected virtual void Start() 
	{
		hp = maxHealth;
		invulnerableObjects = new Dictionary<IDamageable, float>();
		cannotBeDamagedBy = new HashSet<IDamageDealer>();
	}

	public void DealDamage(int amount, IDamageable entity)
	{
		entity.TakeDamage(damage, this);
		invulnerableObjects.Add(entity, Time.timeSinceLevelLoad);
	}
	
    public bool TakeDamage(int amount, IDamageDealer instigator)
	{
		if (isDead || indestructible || cannotBeDamagedBy.Contains(instigator))
			return false;

		print(gameObject.name + " hit");

		hp = Mathf.Clamp(hp - amount, 0, maxHealth);
		if (hp == 0)
		{
			Die();
			return true;
		}

		OnTookDamage();

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
		print(gameObject.name + " death");

		isDead = true;

		//Play FX
		if (deathFXPrefab)
			Instantiate(deathFXPrefab, transform.position, Quaternion.identity);

		OnDeath();

		Destroy(gameObject, deathDelay);
	}

	public void AddTemporalInvunerability(IDamageDealer forEntity, float duration)
	{
		cannotBeDamagedBy.Add(forEntity);
		StartCoroutine(RemoveTempInvDispatch(forEntity, duration));
	}

	public void RemoveTemporalInvunerability(IDamageDealer forEntity)
	{
		cannotBeDamagedBy.Remove(forEntity);
	}

	IEnumerator RemoveTempInvDispatch(IDamageDealer forEntity, float duration)
	{
		yield return new WaitForSeconds(duration);
		RemoveTemporalInvunerability(forEntity);
	}

	private void OnTriggerEnter2D(Collider2D other) {

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

			DealDamage(damage, damageableObj);
		}
	}

	virtual protected void OnTookDamage()
	{

	}

	virtual protected void OnDeath()
	{

	}
}
