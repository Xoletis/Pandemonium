using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("#Pandemonium/IA/FieldOfView")]
public class FieldOfView : MonoBehaviour
{
    // Définir la portée de la vue (rayon de détection)
    public float radius;

    // Angle de la vue en degrés, spécifiant le champ de vision
    [Range(0, 360)]
    public float angle;

    // Distance à laquelle l'IA peut détecter des odeurs (utilisée pour des comportements sensoriels autres que la vue)
    public float smellDistance;

    // Référence au joueur pour la détection
    public GameObject playerRef;

    // Masques de couches pour la détection des cibles et les obstacles (murs, objets)
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    // Indicateur si le joueur peut être vu par l'IA
    public bool canSeePlayer;

    // Option pour afficher un gizmo de débogage dans l'éditeur
    public bool DebugGizmo = false;

    // Initialisation des variables
    private void Awake()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player"); // Recherche le joueur dans la scène
    }

    // Démarre la routine qui vérifie la vue de l'IA en boucle
    private void Start()
    {
        StartCoroutine(FOVRoutine()); // Démarre la coroutine qui vérifie la vue de l'IA
    }

    // Coroutine qui vérifie l'état de la vue à intervalles réguliers
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f); // Pause de 0.2 secondes entre chaque vérification

        while (true)
        {
            yield return wait; // Attente de la période définie avant de continuer
            FieldOfViewCheck(); // Vérifie la vue de l'IA
        }
    }

    // Fonction qui effectue la vérification du champ de vision
    private void FieldOfViewCheck()
    {
        if(PlayerStats.instance.invisible)
        {
            canSeePlayer = false;
            return;
        }
        // Vérification de la présence de cibles dans le rayon de détection
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        // Vérification de la présence d'une cible à une distance de "smellDistance" (utilisé pour détecter des cibles à proximité via l'odorat)
        Collider[] smellChecks = Physics.OverlapSphere(transform.position, smellDistance, targetMask);

        if (smellChecks.Length != 0)
        {
            // Si quelque chose est détecté dans le rayon de l'odorat, l'IA peut voir le joueur
            canSeePlayer = true;
            return; // Sort de la fonction pour ne pas faire la vérification de la vue si l'odorat est actif
        }

        if (rangeChecks.Length != 0)  // Si un objet est détecté dans le rayon de la vue
        {
            // Obtient la cible détectée (premier objet dans rangeChecks)
            Transform target = rangeChecks[0].transform;

            // Calcule la direction vers la cible
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // Vérifie si l'angle entre la direction de l'IA et la cible est inférieur à l'angle de vue
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                // Calcule la distance entre l'IA et la cible
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                // Si l'IA n'est pas bloquée par un obstacle entre elle et la cible (raycast)
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true; // L'IA peut voir le joueur
                }
                else
                    canSeePlayer = false; // Si un obstacle bloque la vue, l'IA ne peut pas voir le joueur
            }
            else
                canSeePlayer = false; // Si la cible est en dehors de l'angle de vue, l'IA ne peut pas voir le joueur
        }
        else if (canSeePlayer)
            canSeePlayer = false; // Si l'IA ne détecte plus de cible et que la variable canSeePlayer est vraie, elle ne voit plus le joueur
    }

    // Dessine un gizmo de débogage dans l'éditeur Unity pour visualiser la portée de l'odorat
    private void OnDrawGizmos()
    {
        if (DebugGizmo)
        {
            Gizmos.color = Color.yellow; // Définit la couleur du gizmo
            Gizmos.DrawWireSphere(transform.position, smellDistance); // Dessine une sphère autour de l'objet pour représenter la distance d'odorat
        }
    }
}
