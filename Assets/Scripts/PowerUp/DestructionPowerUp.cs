using UnityEngine;

[AddComponentMenu("#Pandemonium/PowerUp/DestructionPowerUp")]
public class DestructionPowerUp : MonoBehaviour
{
    public ParticleSystem destruction;
    public float radius = 50f; // Rayon de la sph�re

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
        // Centre de la sph�re (on utilise la position de l'objet actuel)
        Vector3 sphereCenter = transform.position;

        // R�cup�ration de tous les objets dans la sph�re
        Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, radius);

        foreach (Collider hitCollider in hitColliders)
        {
            // V�rifie si l'objet a le tag "Damagable"
            if (hitCollider.CompareTag("Damagable"))
            {
                // Tente de r�cup�rer le composant RatStat
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
