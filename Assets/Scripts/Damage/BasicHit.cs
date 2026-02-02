using UnityEngine;

public class BasicHit : MonoBehaviour
{
    public int damage = 1;
    public float knockbackIntensity;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        IDamageable target;
        if (collider.TryGetComponent<IDamageable>(out target))
        {
            Vector2 hitDirection = -(transform.position - collider.transform.position).normalized;
            target.TakeHit(damage, hitDirection.normalized * knockbackIntensity);
        }
    }
}