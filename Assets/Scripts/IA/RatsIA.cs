using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[AddComponentMenu("#Pandemonium/IA/Rats IA")]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FieldOfView))]
public class RatsIA : MonoBehaviour
{
    // Enumération des états possibles du comportement de l'IA
    public enum FlockingState { Errance, Attack, Flee }
    public FlockingState currentState = FlockingState.Errance;

    [Header("Commun")]
    public float separationDistance = 2f;  // Distance à laquelle les rats se séparent pour éviter le chevauchement
    public float maxDistance = 10f;  // Distance maximale pour rechercher des positions
    public float maxForce = 5f;  // Limite de la force totale de flocking

    [Header("Errance")]
    public float wanderSpeed = 3f;  // Vitesse de déplacement en errance
    public float wanderRadius = 5f;  // Rayon dans lequel l'IA cherche à errer

    [Header("Attaque")]
    public float attackCallRadius = 10f;  // Rayon de détection pour l'attaque
    public float attackSpeed = 6f;  // Vitesse de déplacement en mode attaque

    [Header("Fuite")]
    public float fleeSpeed = 7f;  // Vitesse de déplacement en fuite
    public float fleeDistance = 15f;  // Distance maximale pour choisir une position de fuite

    public float separationWeight = 1.5f;  // Poids de la séparation dans le flocking
    public float alignmentWeight = 1.0f;  // Poids de l'alignement dans le flocking
    public float cohesionWeight = 1.0f;  // Poids de la cohésion dans le flocking

    // Variables privées pour le comportement des rats
    private List<RatsIA> neighbors;  // Liste des voisins proches
    private NavMeshAgent agent;  // Référence au composant NavMeshAgent
    private Vector3 wanderTarget;  // Cible pour l'errance
    private Vector3 fleeTarget;  // Cible pour la fuite
    [SerializeField]
    private Transform attackTarget;  // Cible pour l'attaque
    private RatsStats stats;  // Référence aux statistiques du rat (pour les attaques)
    private float wanderCooldown = 2f;  // Temps d'attente avant de choisir une nouvelle cible d'errance
    private float wanderTimer;  // Timer pour l'errance

    private FieldOfView fov;  // Référence au composant FieldOfView pour la détection du joueur

    public bool ShowDebugWire = true;  // Affichage des gizmos de débogage dans l'éditeur

    void Start()
    {
        neighbors = new List<RatsIA>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = wanderSpeed;

        // Choisir une première cible d'errance aléatoire
        wanderTarget = GetRandomPositionOnNavMeshSurface();
        agent.SetDestination(wanderTarget);
        wanderTimer = wanderCooldown;

        fov = GetComponent<FieldOfView>();
        attackTarget = fov.playerRef.transform;  // Assigner la cible d'attaque (le joueur)
        stats = GetComponent<RatsStats>();
    }

    void Update()
    {
        // Mise à jour de l'état de l'IA en fonction de la visibilité du joueur
        switch (currentState)
        {
            case FlockingState.Errance:
                Wander();  // Comportement en errance
                break;

            case FlockingState.Attack:
                AttackPlayer();  // Comportement en attaque
                break;

            case FlockingState.Flee:
                Flee();  // Comportement en fuite
                break;
        }

        // Change d'état en fonction de la détection du joueur
        if (currentState != FlockingState.Flee)
        {
            currentState = fov.canSeePlayer ? FlockingState.Attack : FlockingState.Errance;
        }
    }

    void Wander()
    {
        agent.speed = wanderSpeed;

        // Si la destination est atteinte ou le cooldown est terminé, choisir une nouvelle cible d'errance
        wanderTimer -= Time.deltaTime;
        if ((!agent.pathPending && agent.remainingDistance < 0.5f) || wanderTimer <= 0f)
        {
            wanderTarget = GetRandomPositionOnNavMeshSurface();
            agent.SetDestination(wanderTarget);
            wanderTimer = wanderCooldown; // Réinitialiser le timer
        }

        // Trouver les voisins et appliquer les forces de flocking
        FindNeighbors();
        Vector3 flockingForce = CalculateFlockingForce();

        // Limiter la force totale pour éviter des mouvements circulaires
        flockingForce = Vector3.ClampMagnitude(flockingForce, maxForce);

        // Ajuster la direction vers la cible d'errance en prenant en compte le flocking
        Vector3 targetDirection = (wanderTarget - transform.position).normalized + flockingForce;
        targetDirection.y = 0; // Verrouiller l'axe Y

        // Calculer la direction finale à appliquer
        Vector3 finalDirection = Vector3.ClampMagnitude(targetDirection, maxForce);
        Vector3 finalDestination = transform.position + finalDirection;

        // Appliquer la direction finale
        if (agent.remainingDistance > 0.5f)
        {
            agent.SetDestination(finalDestination);
        }
    }

    void AttackPlayer()
    {
        // Définir la destination de l'agent vers la position du joueur
        agent.SetDestination(attackTarget.position);
        agent.speed = attackSpeed;

        // Si l'IA est assez proche du joueur, effectuer une attaque
        if (Vector3.Distance(transform.position, attackTarget.position) <= 1f)
        {
            stats.Attack();  // Appeler la méthode d'attaque des statistiques
        }
    }

    void Flee()
    {
        agent.speed = fleeSpeed;

        // Si la destination de fuite est atteinte, repasser en errance
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentState = FlockingState.Errance;
            wanderTarget = GetRandomPositionOnNavMeshSurface();
            agent.SetDestination(wanderTarget);
            return;
        }
    }

    public void TriggerFlee()
    {
        // Déclencher la fuite et choisir un point éloigné
        currentState = FlockingState.Flee;
        fleeTarget = GetFleePosition();
        agent.SetDestination(fleeTarget);
    }

    private void FindNeighbors()
    {
        neighbors.Clear();
        RatsIA[] allAgents = FindObjectsOfType<RatsIA>();  // Trouver tous les rats dans la scène

        // Trouver les voisins proches dans un rayon d'errance
        foreach (var agent in allAgents)
        {
            if (agent != this && Vector3.Distance(agent.transform.position, transform.position) < wanderRadius)
            {
                neighbors.Add(agent);
            }
        }
    }

    private Vector3 CalculateFlockingForce()
    {
        // Calculer la force totale de flocking (séparation, alignement, cohésion)
        Vector3 separation = CalculateSeparation() * separationWeight;
        Vector3 alignment = CalculateAlignment() * alignmentWeight;
        Vector3 cohesion = CalculateCohesion() * cohesionWeight;

        return separation + alignment + cohesion;
    }

    private Vector3 CalculateSeparation()
    {
        Vector3 separationForce = Vector3.zero;
        int count = 0;

        // Calculer la force de séparation (éviter que les rats ne se chevauchent)
        foreach (var neighbor in neighbors)
        {
            float distance = Vector3.Distance(neighbor.transform.position, transform.position);
            if (distance < separationDistance)
            {
                Vector3 diff = transform.position - neighbor.transform.position;
                diff = diff.normalized / distance;  // Inverser proportionnellement à la distance
                diff.y = 0;  // Verrouiller l'axe Y
                separationForce += diff;
                count++;
            }
        }

        if (count > 0)
        {
            separationForce /= count;
        }

        return separationForce;
    }

    private Vector3 CalculateAlignment()
    {
        Vector3 alignmentForce = Vector3.zero;
        int count = 0;

        // Calculer l'alignement des rats (suivre la direction des voisins)
        foreach (var neighbor in neighbors)
        {
            alignmentForce += neighbor.transform.forward;
            count++;
        }

        if (count > 0)
        {
            alignmentForce /= count;
            alignmentForce.y = 0;  // Verrouiller l'axe Y
            alignmentForce = alignmentForce.normalized; // Normaliser la direction
        }

        return alignmentForce;
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 cohesionForce = Vector3.zero;
        int count = 0;

        // Calculer la cohésion (se regrouper avec les voisins)
        foreach (var neighbor in neighbors)
        {
            cohesionForce += neighbor.transform.position;
            count++;
        }

        if (count > 0)
        {
            cohesionForce /= count;
            cohesionForce = cohesionForce - transform.position;
            cohesionForce.y = 0;  // Verrouiller l'axe Y
            cohesionForce = cohesionForce.normalized;
        }

        return cohesionForce;
    }

    private Vector3 GetRandomPositionOnNavMeshSurface()
    {
        // Générer une position aléatoire dans un rayon autour du rat et vérifier si elle est valide sur le NavMesh
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + transform.position;
        randomPos.y = Terrain.activeTerrain.SampleHeight(transform.position) + Terrain.activeTerrain.GetPosition().y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas))
        {
            return hit.position; // Retourner la position valide
        }
        else
        {
            Debug.LogWarning($"Aucune position valide trouvée autour de {randomPos}");
            return transform.position; // Retourner la position actuelle si aucune position valide
        }
    }

    private Vector3 GetFleePosition()
    {
        // Générer une position de fuite aléatoire éloignée
        Vector3 randomDirection = Random.insideUnitSphere * fleeDistance;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, fleeDistance, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            Debug.LogWarning("Aucune position de fuite valide trouvée.");
            return transform.position; // Retourner la position actuelle si aucune position valide
        }
    }

    private void OnDrawGizmos()
    {
        if (ShowDebugWire)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, wanderRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackCallRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, fleeDistance);
        }
    }
}