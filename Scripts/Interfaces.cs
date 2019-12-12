using UnityEngine;

public interface IDamageable
{
    bool TakeDamage(int amount);

	bool HealDamage(int amount);

	void Die();

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

