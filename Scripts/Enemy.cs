using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : SimpleEnemyUnit
{
	[Header("Enemy")]
	[SerializeField] private GameObject leftoverPrefab; //Prefab that is spawned after the enemy dies

	private Animator animator;
	private MoveTowardsPlayer movement; 

	protected override void Start()
	{
		base.Start();

		movement = GetComponent<MoveTowardsPlayer>();
		animator = GetComponent<Animator>();
	}

    protected override void OnTookDamage()
	{
		animator.SetTrigger("hit");
	}

	protected override void OnDeath()
	{
		movement.enabled = false;

		gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");
		animator.SetTrigger("die");

		Invoke("SpawnPrefabOnDeath", deathDelay);		
	}

	void SpawnPrefabOnDeath()
	{
		if (leftoverPrefab)
			Instantiate(leftoverPrefab, transform.position, Quaternion.identity);
	}

}
