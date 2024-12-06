using System.Collections;
using UnityEngine;

// Ajoute un menu personnalis� dans l'�diteur Unity pour cet objet en particulier.
// Utilis� pour g�rer les armes � distance.
[AddComponentMenu("#Pandemonium/Player/RangeWeapon/Core")]
public class RangeWeapon : MonoBehaviour
{
    public LayerMask mask; // Masque de couche pour d�finir les surfaces sur lesquelles le tir peut atterrir.
    public WeaponItem item; // R�f�rence � l'objet d'arme (comme un pistolet, fusil, etc.).

    public Transform firePoint; // Le point d'o� le tir va partir (par exemple, l'embout du canon de l'arme).
    public ParticleSystem fireSystem; // Le syst�me de particules pour afficher l'effet de tir.

    public bool fireing; // Indique si l'arme est en train de tirer (utilis� pour les armes automatiques).

    Camera cam; // R�f�rence � la cam�ra principale.

    float lastShootTime; // Le moment o� le dernier tir a eu lieu.
    private float nextFireTime = 0f; // Temps avant que le prochain tir ne soit autoris� (pour les armes semi-automatiques).

    private void Start()
    {
        // R�cup�re la cam�ra principale de la sc�ne.
        cam = Camera.main;
    }

    // G�re le tir d'une arme semi-automatique (tir unique � chaque pression sur le bouton).
    public void HandleSemiAutomaticWeapon()
    {
        shot(); // Appelle la m�thode de tir.
    }

    // Fonction de tir de l'arme.
    void shot()
    {
        Debug.Log("Shot"); // Affiche un message dans la console lorsqu'un tir est effectu�.

        // D�termine dans quel slot se trouve l'arme (par exemple, arme lourde, moyenne ou l�g�re).
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

        // Si suffisamment de temps s'est �coul� depuis le dernier tir (g�re le d�lai entre les tirs).
        if (lastShootTime + item.ShootDelay < Time.time)
        {
            slot.itemEntry.number--; // R�duit le nombre de munitions.
            InventoryManager.instance.RefreshUIAmmo(); // Met � jour l'interface utilisateur avec le nouveau nombre de munitions.
            fireSystem.Play(); // Joue l'effet de particules de tir.

            RaycastHit hit; // Contient des informations sur l'impact du tir.

            // Lance un rayon depuis le point de tir (firePoint) dans la direction calcul�e (obtenue via GetDirection()).
            if (Physics.Raycast(firePoint.position, GetDirection(), out hit, item.range, mask))
            {
                // Si le rayon touche un objet, cr�e un effet de tra�n�e pour simuler la trajectoire du projectile.
                TrailRenderer trail = Instantiate(item.BulletTrail, firePoint.position, Quaternion.identity);

                // Lance une coroutine pour animer la tra�n�e du tir jusqu'� l'impact.
                StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));

                lastShootTime = Time.time; // Met � jour l'heure du dernier tir.

                // Si l'impact touche un objet avec le tag "Damagable", inflige des d�g�ts.
                if (hit.collider.CompareTag("Damagable"))
                {
                    hit.collider.GetComponent<RatsStats>().UpdateHealth(-1 * item.damage, hit.point, item.forceMagnitude);
                }
            }
            else
            {
                // Si le tir ne touche rien, cr�e une tra�n�e qui se termine � une certaine distance.
                TrailRenderer trail = Instantiate(item.BulletTrail, firePoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, firePoint.position + GetDirection() * 100, Vector3.zero, false));

                lastShootTime = Time.time; // Met � jour l'heure du dernier tir.
            }
        }
    }

    // Mise � jour chaque frame pour g�rer les armes automatiques.
    private void Update()
    {
        if (item.isAutomaticWeapon && fireing) // Si l'arme est automatique et que le joueur maintient le tir.
        {
            shot(); // Effectue le tir automatiquement.
        }
    }

    // Calcule la direction du tir en fonction de l'orientation de la cam�ra et de l'�ventuelle dispersion des balles.
    private Vector3 GetDirection()
    {
        // Cr�e un rayon depuis le centre de l'�cran.
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        Vector3 direction = ray.direction; // Direction du rayon.

        // Si l'arme a une dispersion des balles (par exemple, pour un pistolet ou une mitraillette).
        if (item.asBulletSpread)
        {
            // Ajoute une variation al�atoire � la direction du tir pour simuler la dispersion des balles.
            direction += new Vector3(
                Random.Range(-item.BulletSpreadVariance.x, item.BulletSpreadVariance.x),
                Random.Range(-item.BulletSpreadVariance.y, item.BulletSpreadVariance.y),
                Random.Range(-item.BulletSpreadVariance.z, item.BulletSpreadVariance.z)
            );

            direction.Normalize(); // Normalise la direction pour que la longueur du vecteur reste constante.
        }

        return direction;
    }

    // Affiche un rayon visuel dans l'�diteur pour montrer la port�e du tir.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white; // D�finit la couleur du rayon.

        Vector3 origin = firePoint.position; // Point de d�part du tir.
        Vector3 direction = cam.transform.forward * item.range; // Direction du rayon en fonction de la port�e de l'arme.

        Gizmos.DrawLine(origin, origin + direction); // Affiche la ligne de la port�e dans l'�diteur.
    }

    // Coroutine pour animer la tra�n�e du tir.
    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal, bool MadeImpacte)
    {
        Vector3 startpos = trail.transform.position; // Position de d�part de la tra�n�e.
        float distance = Vector3.Distance(trail.transform.position, hitPoint); // Distance entre le d�part et l'impact.
        float remainingDistance = distance; // Distance restante � parcourir.

        while (remainingDistance > 0)
        {
            // Interpole la position de la tra�n�e entre le point de d�part et le point d'impact.
            trail.transform.position = Vector3.Lerp(startpos, hitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= item.bulletSpeed * Time.deltaTime; // R�duit la distance restante en fonction de la vitesse de la balle.

            yield return null; // Attendre la prochaine frame.
        }

        trail.transform.position = hitPoint; // Met � jour la position finale de la tra�n�e.

        // Si l'impact a eu lieu, g�n�re un effet d'impact.
        if (MadeImpacte)
        {
            GameObject impact = Instantiate(item.ImpactParticuleSystem, hitPoint, Quaternion.LookRotation(hitNormal));
            Destroy(impact, 1f); // D�truit l'effet d'impact apr�s 1 seconde.
        }

        Destroy(trail.gameObject, trail.time); // D�truit la tra�n�e apr�s qu'elle soit anim�e.
    }
}
