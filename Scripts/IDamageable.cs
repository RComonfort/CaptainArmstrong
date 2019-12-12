public interface IDamageable
{
    bool TakeDamage(int amount);

	bool HealDamage(int amount);

	void Die();

}
