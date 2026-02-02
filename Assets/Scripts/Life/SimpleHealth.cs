using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SimpleHealth : MonoBehaviour, IHealth, IDamageable
{
    [Header("Health Variables")]
    [Range(1, 20), SerializeField] private int _maxHealth;
    [Range(0, 20), SerializeField] private int _currentHealth;
    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;

    [Header("Health Events")]
    [SerializeField] private UnityEvent _onHealthChanged;
    [SerializeField] private UnityEvent _onDeath;
    public UnityEvent<Vector2> OnKnockback;
    public UnityEvent OnHealthChanged => _onHealthChanged;
    public UnityEvent OnDeath => _onDeath;

    public bool IsVulnerable => _isInvulnerable;
    [SerializeField] private bool _isInvulnerable = false;
    [SerializeField] private float invincibilityDuration = 1f;
    private Coroutine damageRoutine;

    public void Heal(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Heal amount shouldn't be negative.", this);
            return;
        }
        SetLife(_currentHealth + amount);
    }
    public void InstantKill() => Die();

    /// <summary>
    /// Pure perte de vie, à utiliser par exemple pour les DoT
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (damage < 0)
        {
            Debug.LogWarning("Damage amount shouldn't be negative.", this);
            return;
        }

        if (_isInvulnerable) return;
        damageRoutine = StartCoroutine(TakingDamage(damage));
    }

    /// <summary>
    /// Dégats provenant d'une source avec une direction
    /// </summary>
    /// <param name="damage"> Quantité de vie perdu </param>
    /// <param name="knockback"> Direction + Force de l'impact</param>
    public void TakeHit(int damage, Vector2 knockback)
    {
        TakeDamage(damage);
        OnKnockback?.Invoke(knockback);
    }


    private void SetLife(int value)
    {
        _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
        _onHealthChanged.Invoke();
    }
    private void Die()
    {
        _currentHealth = 0;
        OnHealthChanged?.Invoke();

        _onDeath.Invoke();
    }
    private IEnumerator TakingDamage(int damage)
    {
        SetLife(_currentHealth - damage);

        // Invincibility frames
        _isInvulnerable = true;
        yield return new WaitForSeconds(invincibilityDuration);
        _isInvulnerable = false;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }
}
