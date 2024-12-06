using System.Collections;
using UnityEngine;

// Ajoute un menu personnalisé dans l'éditeur Unity pour cet objet en particulier.
// Utilisé pour gérer les armes à distance.
[AddComponentMenu("#Pandemonium/Player/RangeWeapon/Core")]
public class RangeWeapon : MonoBehaviour
{
    public LayerMask mask; // Masque de couche pour définir les surfaces sur lesquelles le tir peut atterrir.
    public WeaponItem item; // Référence à l'objet d'arme (comme un pistolet, fusil, etc.).

    public Transform firePoint; // Le point d'où le tir va partir (par exemple, l'embout du canon de l'arme).
    public ParticleSystem fireSystem; // Le système de particules pour afficher l'effet de tir.

    public bool fireing; // Indique si l'arme est en train de tirer (utilisé pour les armes automatiques).

    Camera cam; // Référence à la caméra principale.

    float lastShootTime; // Le moment où le dernier tir a eu lieu.
    private float nextFireTime = 0f; // Temps avant que le prochain tir ne soit autorisé (pour les armes semi-automatiques).

    private void Start()
    {
        // Récupère la caméra principale de la scène.
        cam = Camera.main;
    }

    // Gère le tir d'une arme semi-automatique (tir unique à chaque pression sur le bouton).
    public void HandleSemiAutomaticWeapon()
    {
        shot(); // Appelle la méthode de tir.
    }

    // Fonction de tir de l'arme.
    void shot()
    {
        Debug.Log("Shot"); // Affiche un message dans la console lorsqu'un tir est effectué.

        // Détermine dans quel slot se trouve l'arme (par exemple, arme lourde, moyenne ou légère).
        Slots slot;
        switch (InventoryManager.instance.SlotUse)
        {
            case 0: slot = InventoryManager.instance.heavyWeapon; break;
            case 1: slot = InventoryManager.instance.mediumWeapon; break;
            case 2: slot = InventoryManager.instance.lightWeapon; break;
            default: slot = new Slots(); break;
        }

        // Si le joueur n'a plus de munitions pour cette arme, il ne peut pas tirer.
        if (slot.itemEntry.number <= 0) { return; }

        // Si suffisamment de temps s'est écoulé depuis le dernier tir (gère le délai entre les tirs).
        if (lastShootTime + item.ShootDelay < Time.time)
        {
            slot.itemEntry.number--; // Réduit le nombre de munitions.
            InventoryManager.instance.RefreshUIAmmo(); // Met à jour l'interface utilisateur avec le nouveau nombre de munitions.
            fireSystem.Play(); // Joue l'effet de particules de tir.

            RaycastHit hit; // Contient des informations sur l'impact du tir.

            // Lance un rayon depuis le point de tir (firePoint) dans la direction calculée (obtenue via GetDirection()).
            if (Physics.Raycast(firePoint.position, GetDirection(), out hit, item.range, mask))
            {
                // Si le rayon touche un objet, crée un effet de traînée pour simuler la trajectoire du projectile.
                TrailRenderer trail = Instantiate(item.BulletTrail, firePoint.position, Quaternion.identity);

                // Lance une coroutine pour animer la traînée du tir jusqu'à l'impact.
                StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));

                lastShootTime = Time.time; // Met à jour l'heure du dernier tir.

                // Si l'impact touche un objet avec le tag "Damagable", inflige des dégâts.
                if (hit.collider.CompareTag("Damagable"))
                {
                    hit.collider.GetComponent<RatsStats>().UpdateHealth(-1 * item.damage, hit.point, item.forceMagnitude);
                }
            }
            else
            {
                // Si le tir ne touche rien, crée une traînée qui se termine à une certaine distance.
                TrailRenderer trail = Instantiate(item.BulletTrail, firePoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, firePoint.position + GetDirection() * 100, Vector3.zero, false));

                lastShootTime = Time.time; // Met à jour l'heure du dernier tir.
            }
        }
    }

    // Mise à jour chaque frame pour gérer les armes automatiques.
    private void Update()
    {
        if (item.isAutomaticWeapon && fireing) // Si l'arme est automatique et que le joueur maintient le tir.
        {
            shot(); // Effectue le tir automatiquement.
        }
    }

    // Calcule la direction du tir en fonction de l'orientation de la caméra et de l'éventuelle dispersion des balles.
    private Vector3 GetDirection()
    {
        // Crée un rayon depuis le centre de l'écran.
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        Vector3 direction = ray.direction; // Direction du rayon.

        // Si l'arme a une dispersion des balles (par exemple, pour un pistolet ou une mitraillette).
        if (item.asBulletSpread)
        {
            // Ajoute une variation aléatoire à la direction du tir pour simuler la dispersion des balles.
            direction += new Vector3(
                Random.Range(-item.BulletSpreadVariance.x, item.BulletSpreadVariance.x),
                Random.Range(-item.BulletSpreadVariance.y, item.BulletSpreadVariance.y),
                Random.Range(-item.BulletSpreadVariance.z, item.BulletSpreadVariance.z)
            );

            direction.Normalize(); // Normalise la direction pour que la longueur du vecteur reste constante.
        }

        return direction;
    }

    // Affiche un rayon visuel dans l'éditeur pour montrer la portée du tir.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white; // Définit la couleur du rayon.

        Vector3 origin = firePoint.position; // Point de départ du tir.
        Vector3 direction = cam.transform.forward * item.range; // Direction du rayon en fonction de la portée de l'arme.

        Gizmos.DrawLine(origin, origin + direction); // Affiche la ligne de la portée dans l'éditeur.
    }

    // Coroutine pour animer la traînée du tir.
    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal, bool MadeImpacte)
    {
        Vector3 startpos = trail.transform.position; // Position de départ de la traînée.
        float distance = Vector3.Distance(trail.transform.position, hitPoint); // Distance entre le départ et l'impact.
        float remainingDistance = distance; // Distance restante à parcourir.

        while (remainingDistance > 0)
        {
            // Interpole la position de la traînée entre le point de départ et le point d'impact.
            trail.transform.position = Vector3.Lerp(startpos, hitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= item.bulletSpeed * Time.deltaTime; // Réduit la distance restante en fonction de la vitesse de la balle.

            yield return null; // Attendre la prochaine frame.
        }

        trail.transform.position = hitPoint; // Met à jour la position finale de la traînée.

        // Si l'impact a eu lieu, génère un effet d'impact.
        if (MadeImpacte)
        {
            GameObject impact = Instantiate(item.ImpactParticuleSystem, hitPoint, Quaternion.LookRotation(hitNormal));
            Destroy(impact, 1f); // Détruit l'effet d'impact après 1 seconde.
        }

        Destroy(trail.gameObject, trail.time); // Détruit la traînée après qu'elle soit animée.
    }
}
