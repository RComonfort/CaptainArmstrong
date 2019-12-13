using UnityEngine;

public interface IDamageable
{
    bool TakeDamage(int amount, IDamageDealer instigator);

	bool HealDamage(int amount);

	void Die();

	void AddTemporalInvunerability(IDamageDealer forEntity, float duration);

	void RemoveTemporalInvunerability(IDamageDealer forEntity);

}

public interface IDamageDealer
{
	void DealDamage(int amount, IDamageable entity);
}

public interface IRideable
{
	void GetsRidden(Player by);

	void StopBeingRidden();

	void Rotate(float angle);

	void CancelRotation();
}

public interface IPickable {
	
	void Pickup(Player player);
}


public interface ITriggerListener
{
	void OnObjectEnteredTrigger(Trigger2DRelay triggerObj, Collider2D other);

	void OnObjectExitedTrigger(Trigger2DRelay triggerObj, Collider2D other);

	void OnObjectStayedTrigger(Trigger2DRelay triggerObj, Collider2D other);
}

