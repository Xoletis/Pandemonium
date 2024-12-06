using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("#Pandemonium/Environement/Spawner")]
public class Spawner : MonoBehaviour
{
    [HideInInspector]
    public List<MonsterSpawn> spawns = new List<MonsterSpawn>();  // Liste des types de monstres et leurs caractéristiques (probabilité, etc.)
    public GameObject objectToSpawn;  // L'objet à instancier (monstre ou autre)
    public Vector2 areaSize = new Vector2(10, 10);  // Taille de la zone de spawn (en x et z)
    bool activate;  // État d'activation du spawner
    public float secondsBetweenSpawns = 2f;  // Délai entre chaque spawn
    public GameObject ActivateVisual;  // Objet visuel pour signaler que le spawner est activé

    private void Start()
    {
        StartCoroutine(spawnCorountine());  // Lance la coroutine pour effectuer les spawns
        SetActivate(false);  // Déactive le spawner au démarrage
    }

    // Active ou désactive l'affichage visuel du spawner et son état
    public void SetActivate(bool value)
    {
        ActivateVisual.SetActive(value);  // Active ou désactive l'objet visuel
        activate = value;  // Modifie l'état d'activation
    }

    // Gère le spawn des monstres en fonction du multiplicateur
    public void Spawn(int multiply = 1)
    {
        if (multiply <= 0) multiply = 1;  // Si le multiplicateur est 0 ou négatif, on le remet à 1
        int n = GetRandomSpawnIndex();  // Récupère un indice de spawn aléatoire
        if (n == -1) return;  // Si aucun spawn valide n'est trouvé, on sort de la méthode

        GameObject objectToSpawn = spawns[n].monster;  // Le monstre à instancier (selon l'indice obtenu)

        // Centre de la zone de spawn, basé sur la position du spawner
        Vector3 center = transform.position;

        // Détermine le nombre de monstres à spawn (basé sur la plage min/max et le multiplicateur)
        int nb = Random.Range(spawns[n].min, spawns[n].max);
        nb *= multiply;

        // Génère les monstres à spawn
        for (int i = 0; i < nb; i++)
        {
            // Génère une position aléatoire dans la zone définie autour du centre
            float randomX = Random.Range(-areaSize.x / 2, areaSize.x / 2) + center.x;
            float randomZ = Random.Range(-areaSize.y / 2, areaSize.y / 2) + center.z;
            Vector3 spawnPosition = new Vector3(randomX, transform.position.y, randomZ);

            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);  // Instancie le monstre à la position calculée
        }
    }

    // Sélectionne un spawn de monstre aléatoire en fonction des probabilités configurées
    public int GetRandomSpawnIndex()
    {
        if (spawns == null || spawns.Count == 0)
            return -1;  // Si la liste de spawns est vide, retourne -1

        int totalWeight = 0;
        foreach (var spawn in spawns)
        {
            totalWeight += spawn.percentage;  // Calcule le poids total des probabilités
        }

        if (totalWeight == 0)
            return -1;  // Si les probabilités ne sont pas configurées, retourne -1

        int randomValue = Random.Range(0, totalWeight);  // Tire un nombre aléatoire entre 0 et le poids total
        int cumulativeWeight = 0;

        for (int i = 0; i < spawns.Count; i++)
        {
            cumulativeWeight += spawns[i].percentage;  // Cumul des poids
            if (randomValue < cumulativeWeight)
            {
                return i;  // Retourne l'indice du spawn sélectionné
            }
        }

        return -1;  // Cas improbable si les probabilités sont correctement configurées
    }

    // Lorsque le joueur entre dans la zone de déclenchement du spawner
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Si l'objet entrant est le joueur
        {
            SetActivate(true);  // Active le spawner
            Spawn();  // Lance un spawn immédiatement
        }
    }

    // Lorsque le joueur sort de la zone de déclenchement du spawner
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // Si l'objet sortant est le joueur
        {
            SetActivate(false);  // Désactive le spawner
        }
    }

    // Coroutine qui s'exécute en continu pour spawn des monstres à intervalle régulier
    IEnumerator spawnCorountine()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsBetweenSpawns);  // Attend un délai entre chaque spawn
            if (activate)  // Si le spawner est activé
            {
                Spawn();  // Effectue un spawn
            }
        }
    }
}
