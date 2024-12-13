using UnityEngine;

[AddComponentMenu("#Pandemonium/PowerUp/HealthPowerUp")]
public class HealthPowerUp : MonoBehaviour
{
    public ParticleSystem health;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats.instance.health = PlayerStats.instance.maxHealth;
            if (health != null)
            {
                Instantiate(health, PlayerStats.instance.gameObject.transform);
                Destroy(health, 5);
            }
            Destroy(gameObject);
        }
    }
}
