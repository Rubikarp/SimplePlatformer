using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerRespawn : MonoBehaviour
{
    [Header("CheckPoints")]
    public Vector3 defaultRespawnPos = Vector3.zero;
    public CheckPoint lastCheckPoint = null;

    [Header("References")]
    public Rigidbody2D playerRigidbody;
    public SimpleHealth playerHealth;
    public PlayerKnocback playerKnocback;
    public PlayerJump playerJump;
    public PlayerMovement playerMovement;

    public UnityEvent onRespawn;

    private Coroutine respawnCoroutine;
    public Vector3 GetRespawnPosition()
    {
        if (lastCheckPoint != null)
        {
            return lastCheckPoint.transform.position;
        }
        return defaultRespawnPos;
    }
    public void RespawnIn(float duration = 1f)
    {
        if (respawnCoroutine != null) return;
        respawnCoroutine = StartCoroutine(RespawningIn(duration));

    }
    private IEnumerator RespawningIn(float duration = 1f)
    {
        playerKnocback.DisablePlayerAbility();

        yield return new WaitForSeconds(duration);

        transform.position = GetRespawnPosition();
        playerRigidbody.linearVelocity = Vector2.zero;
        playerRigidbody.angularVelocity = 0f;

        playerHealth.Heal(playerHealth.MaxHealth);
        playerKnocback.EnablePlayerAbility();
        onRespawn?.Invoke();

        respawnCoroutine = null;
    }
}