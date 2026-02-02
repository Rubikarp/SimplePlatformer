using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerKnocback : MonoBehaviour
{
    private Rigidbody2D body;

    [Header("References")]
    public PlayerJump jumpAbility;
    public PlayerMovement moveAbility;

    [Header("Settings")]
    public float duration = .3f;
    public Coroutine KnockBackCoroutine;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }
    public void KnockBackPlayer(Vector2 hit)
    {
        if (KnockBackCoroutine != null)
        {
            StopCoroutine(KnockBackCoroutine);
        }

        KnockBackCoroutine = StartCoroutine(GettingKnockback(hit));

    }

    private IEnumerator GettingKnockback(Vector2 hitForce)
    {
        DisablePlayerAbility();
        body.linearVelocity = hitForce;

        yield return new WaitForSeconds(duration);

        EnablePlayerAbility();
        KnockBackCoroutine = null;
    }

    public void DisablePlayerAbility()
    {
        jumpAbility.enabled = false;
        moveAbility.enabled = false;
    }
    public void EnablePlayerAbility()
    {
        jumpAbility.enabled = true;
        moveAbility.enabled = true;

        moveAbility.ResetControl();
        jumpAbility.ResetControl();
    }
}
