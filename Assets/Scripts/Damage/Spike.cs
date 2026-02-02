using UnityEngine;

public class Spike : MonoBehaviour
{
    public int damage = 1;
    public Vector3 knockbackDirection;
    public float knockbackIntensity;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable target;
        if (collision.collider.TryGetComponent<IDamageable>(out target))
        {
            target.TakeHit(damage, knockbackDirection.normalized * knockbackIntensity);
        }
    }
}
