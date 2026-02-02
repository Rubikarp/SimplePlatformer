using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerRespawn playerRespawn = collision.GetComponent<PlayerRespawn>();
        if (playerRespawn != null)
        {
            playerRespawn.lastCheckPoint = this;
        }
    }
}
