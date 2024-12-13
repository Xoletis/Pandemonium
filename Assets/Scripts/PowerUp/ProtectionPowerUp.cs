using UnityEngine;

[AddComponentMenu("#Pandemonium/PowerUp/ProtectionPowerUp")]
public class ProtectionPowerUp : MonoBehaviour
{
    public ParticleSystem protection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            PlayerStats.instance.protectionPU();

            if (protection != null)
            {
                Instantiate(protection, PlayerStats.instance.gameObject.transform);
                Destroy(protection, 5);
            }
            Destroy(gameObject);
        }
    }

    
}
