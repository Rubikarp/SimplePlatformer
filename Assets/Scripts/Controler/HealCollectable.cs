using UnityEngine;

public class HealCollectable : MonoBehaviour
{
    [Header("Heal Settings")]
    public int healAmount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SimpleHealth health = collision.GetComponent<SimpleHealth>();
        if (health != null)
        {
            health.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}