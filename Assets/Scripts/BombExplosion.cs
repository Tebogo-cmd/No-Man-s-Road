using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    public ParticleSystem explosion;
    public float playerDamage = 30f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Player"))
        {
            if (explosion != null)
            {
                ParticleSystem fx = Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(fx.gameObject, fx.main.duration + fx.main.startLifetime.constantMax);
            }

            if (other.gameObject.CompareTag("Player"))
            {
                PlayerInfo info = other.gameObject.GetComponent<PlayerController>()?.playerInfo;
                if (info != null)
                    info.Lives -= playerDamage;
            }

            Destroy(gameObject);
        }
    }
}

