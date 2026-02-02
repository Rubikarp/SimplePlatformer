using UnityEngine;

public interface IDamageable
{
    public bool IsVulnerable { get; }
    void InstantKill();
    void TakeDamage(int damage);
    void TakeHit(int damage, Vector2 knockback);
}
