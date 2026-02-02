using UnityEngine.Events;

public interface IHealth
{
    int CurrentHealth { get; }
    int MaxHealth { get; }

    public UnityEvent OnHealthChanged { get; }
    public UnityEvent OnDeath { get; }

    void Heal(int amount);
}
