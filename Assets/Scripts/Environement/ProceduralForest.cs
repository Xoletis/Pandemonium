using UnityEngine;
using System.Collections.Generic;

public class ProceduralForest : MonoBehaviour
{
    [Header("For�t Configuration")]
    public List<GameObject> arbrePrefabs; // Liste d'arbres possibles
    public int densite = 100; // Densit� des arbres (nombre d'arbres)
    public List<Vector3> pointsDeZone; // Liste des points d�finissant la forme de la for�t
    public float minDistanceArbres = 5f; // Distance minimale entre les arbres
    public LayerMask mask;

    private void Start()
    {
        GenererForet();
    }

    void GenererForet()
    {
        // V�rifier que la zone est d�finie avec assez de points
        if (pointsDeZone.Count < 3)
        {
            Debug.LogError("Il faut au moins 3 points pour d�finir une zone!");
            return;
        }

        // G�n�rer des arbres dans la zone d�finie
        for (int i = 0; i < densite; i++)
        {
            // Trouver un point al�atoire � l'int�rieur de la zone polygonale
            Vector3 point = TrouverPointDansZone();

            // V�rifier que le point est suffisamment �loign� des autres arbres
            if (EstEloigneDesAutresArbres(point))
            {
                // Choisir un arbre au hasard parmi la liste
                GameObject arbrePrefab = arbrePrefabs[Random.Range(0, arbrePrefabs.Count)];

                // Cr�er l'arbre
                Instantiate(arbrePrefab, point, Quaternion.identity);
            }
        }
    }

    // Trouver un point al�atoire � l'int�rieur du polygone d�fini par pointsDeZone
    Vector3 TrouverPointDansZone()
    {
        // Assurez-vous que la zone a plus de 2 points
        if (pointsDeZone.Count < 3) return Vector3.zero;

        Vector3 point = Vector3.zero;
        bool pointDansZone = false;

        while (!pointDansZone)
        {
            // Calculer les bornes X et Z en prenant en compte tous les points
            float xMin = float.MaxValue;
            float xMax = float.MinValue;
            float zMin = float.MaxValue;
            float zMax = float.MinValue;

            foreach (Vector3 v in pointsDeZone)
            {
                xMin = Mathf.Min(xMin, v.x);
                xMax = Mathf.Max(xMax, v.x);
                zMin = Mathf.Min(zMin, v.z);
                zMax = Mathf.Max(zMax, v.z);
            }

            // G�n�rer un point al�atoire dans ces bornes
            point = new Vector3(
                Random.Range(xMin, xMax),
                transform.position.y, // Vous pouvez ajuster cela selon votre logique
                Random.Range(zMin, zMax)
            );

            // V�rifier si ce point est � l'int�rieur du polygone
            pointDansZone = EstDansZone(point);
        }

        return point;
    }

    // M�thode pour v�rifier si un point est � l'int�rieur du polygone
    bool EstDansZone(Vector3 point)
    {
        int j = pointsDeZone.Count - 1;
        bool dansZone = false;

        // Algorithme de ray-casting pour tester si le point est dans le polygone
        for (int i = 0; i < pointsDeZone.Count; i++)
        {
            if ((pointsDeZone[i].z > point.z) != (pointsDeZone[j].z > point.z) &&
                (point.x < (pointsDeZone[j].x - pointsDeZone[i].x) * (point.z - pointsDeZone[i].z) / (pointsDeZone[j].z - pointsDeZone[i].z) + pointsDeZone[i].x))
            {
                dansZone = !dansZone;
            }
            j = i;
        }

        return dansZone;
    }

    // V�rifier si le point est suffisamment �loign� des autres arbres
    bool EstEloigneDesAutresArbres(Vector3 point)
    {
        Collider[] colliders = Physics.OverlapSphere(point, minDistanceArbres, mask);
        return colliders.Length == 0;
    }

    // Visualiser la zone de g�n�ration dans l'�diteur
    private void OnDrawGizmos()
    {
        if (pointsDeZone.Count < 3)
            return;

        Gizmos.color = Color.green; // Choisir une couleur pour la zone

        // Dessiner un polygone reliant les points de la zone
        for (int i = 0; i < pointsDeZone.Count; i++)
        {
            Vector3 startPoint = pointsDeZone[i];
            Vector3 endPoint = pointsDeZone[(i + 1) % pointsDeZone.Count]; // Connecter le dernier point au premier pour fermer le polygone

            Gizmos.DrawLine(startPoint, endPoint); // Dessiner la ligne entre les points
        }
    }
}
