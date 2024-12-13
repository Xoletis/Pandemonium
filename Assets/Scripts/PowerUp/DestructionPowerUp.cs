using UnityEngine;

[AddComponentMenu("#Pandemonium/PowerUp/DestructionPowerUp")]
public class DestructionPowerUp : MonoBehaviour
{
    public ParticleSystem destruction;
    public float radius = 50f; // Rayon de la sphère

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            ApplyDamageToNearby();

            if (destruction != null)
            {
                Instantiate(destruction, PlayerStats.instance.gameObject.transform.position, Quaternion.identity);
                Destroy(destruction, 5);
            }
            Destroy(gameObject);
        }
    }

    void ApplyDamageToNearby()
    {
        Debug.Log("aa");
        // Centre de la sphère (on utilise la position de l'objet actuel)
        Vector3 sphereCenter = transform.position;

        // Récupération de tous les objets dans la sphère
        Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, radius);

        foreach (Collider hitCollider in hitColliders)
        {
            // Vérifie si l'objet a le tag "Damagable"
            if (hitCollider.CompareTag("Damagable"))
            {
                // Tente de récupérer le composant RatStat
                RatsStats ratStat = hitCollider.GetComponent<RatsStats>();
                if (ratStat != null)
                {
                    // Appelle la fonction UpdateHealth du composant RatStat
                    ratStat.UpdateHealth(-10000, Vector3.zero, 0);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
