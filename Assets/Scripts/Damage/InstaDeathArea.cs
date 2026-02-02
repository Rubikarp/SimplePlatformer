using UnityEngine;

public class InstaDeathArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        IDamageable target;
        if(collider.TryGetComponent<IDamageable>(out target))
        {
            target.InstantKill();
        }
    }
}
