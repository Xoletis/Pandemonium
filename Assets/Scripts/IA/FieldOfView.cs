using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("#Pandemonium/IA/FieldOfView")]
public class FieldOfView : MonoBehaviour
{
    // D�finir la port�e de la vue (rayon de d�tection)
    public float radius;

    // Angle de la vue en degr�s, sp�cifiant le champ de vision
    [Range(0, 360)]
    public float angle;

    // Distance � laquelle l'IA peut d�tecter des odeurs (utilis�e pour des comportements sensoriels autres que la vue)
    public float smellDistance;

    // R�f�rence au joueur pour la d�tection
    public GameObject playerRef;

    // Masques de couches pour la d�tection des cibles et les obstacles (murs, objets)
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    // Indicateur si le joueur peut �tre vu par l'IA
    public bool canSeePlayer;

    // Option pour afficher un gizmo de d�bogage dans l'�diteur
    public bool DebugGizmo = false;

    // Initialisation des variables
    private void Awake()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player"); // Recherche le joueur dans la sc�ne
    }

    // D�marre la routine qui v�rifie la vue de l'IA en boucle
    private void Start()
    {
        StartCoroutine(FOVRoutine()); // D�marre la coroutine qui v�rifie la vue de l'IA
    }

    // Coroutine qui v�rifie l'�tat de la vue � intervalles r�guliers
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f); // Pause de 0.2 secondes entre chaque v�rification

        while (true)
        {
            yield return wait; // Attente de la p�riode d�finie avant de continuer
            FieldOfViewCheck(); // V�rifie la vue de l'IA
        }
    }

    // Fonction qui effectue la v�rification du champ de vision
    private void FieldOfViewCheck()
    {
        if(PlayerStats.instance.invisible)
        {
            canSeePlayer = false;
            return;
        }
        // V�rification de la pr�sence de cibles dans le rayon de d�tection
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        // V�rification de la pr�sence d'une cible � une distance de "smellDistance" (utilis� pour d�tecter des cibles � proximit� via l'odorat)
        Collider[] smellChecks = Physics.OverlapSphere(transform.position, smellDistance, targetMask);

        if (smellChecks.Length != 0)
        {
            // Si quelque chose est d�tect� dans le rayon de l'odorat, l'IA peut voir le joueur
            canSeePlayer = true;
            return; // Sort de la fonction pour ne pas faire la v�rification de la vue si l'odorat est actif
        }

        if (rangeChecks.Length != 0)  // Si un objet est d�tect� dans le rayon de la vue
        {
            // Obtient la cible d�tect�e (premier objet dans rangeChecks)
            Transform target = rangeChecks[0].transform;

            // Calcule la direction vers la cible
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // V�rifie si l'angle entre la direction de l'IA et la cible est inf�rieur � l'angle de vue
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                // Calcule la distance entre l'IA et la cible
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                // Si l'IA n'est pas bloqu�e par un obstacle entre elle et la cible (raycast)
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
            canSeePlayer = false; // Si l'IA ne d�tecte plus de cible et que la variable canSeePlayer est vraie, elle ne voit plus le joueur
    }

    // Dessine un gizmo de d�bogage dans l'�diteur Unity pour visualiser la port�e de l'odorat
    private void OnDrawGizmos()
    {
        if (DebugGizmo)
        {
            Gizmos.color = Color.yellow; // D�finit la couleur du gizmo
            Gizmos.DrawWireSphere(transform.position, smellDistance); // Dessine une sph�re autour de l'objet pour repr�senter la distance d'odorat
        }
    }
}
