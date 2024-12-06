using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("#Pandemonium/Environement/Spawner")]
public class Spawner : MonoBehaviour
{
    [HideInInspector]
    public List<MonsterSpawn> spawns = new List<MonsterSpawn>();  // Liste des types de monstres et leurs caract�ristiques (probabilit�, etc.)
    public GameObject objectToSpawn;  // L'objet � instancier (monstre ou autre)
    public Vector2 areaSize = new Vector2(10, 10);  // Taille de la zone de spawn (en x et z)
    bool activate;  // �tat d'activation du spawner
    public float secondsBetweenSpawns = 2f;  // D�lai entre chaque spawn
    public GameObject ActivateVisual;  // Objet visuel pour signaler que le spawner est activ�

    private void Start()
    {
        StartCoroutine(spawnCorountine());  // Lance la coroutine pour effectuer les spawns
        SetActivate(false);  // D�active le spawner au d�marrage
    }

    // Active ou d�sactive l'affichage visuel du spawner et son �tat
    public void SetActivate(bool value)
    {
        ActivateVisual.SetActive(value);  // Active ou d�sactive l'objet visuel
        activate = value;  // Modifie l'�tat d'activation
    }

    // G�re le spawn des monstres en fonction du multiplicateur
    public void Spawn(int multiply = 1)
    {
        if (multiply <= 0) multiply = 1;  // Si le multiplicateur est 0 ou n�gatif, on le remet � 1
        int n = GetRandomSpawnIndex();  // R�cup�re un indice de spawn al�atoire
        if (n == -1) return;  // Si aucun spawn valide n'est trouv�, on sort de la m�thode

        GameObject objectToSpawn = spawns[n].monster;  // Le monstre � instancier (selon l'indice obtenu)

        // Centre de la zone de spawn, bas� sur la position du spawner
        Vector3 center = transform.position;

        // D�termine le nombre de monstres � spawn (bas� sur la plage min/max et le multiplicateur)
        int nb = Random.Range(spawns[n].min, spawns[n].max);
        nb *= multiply;

        // G�n�re les monstres � spawn
        for (int i = 0; i < nb; i++)
        {
            // G�n�re une position al�atoire dans la zone d�finie autour du centre
            float randomX = Random.Range(-areaSize.x / 2, areaSize.x / 2) + center.x;
            float randomZ = Random.Range(-areaSize.y / 2, areaSize.y / 2) + center.z;
            Vector3 spawnPosition = new Vector3(randomX, transform.position.y, randomZ);

            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);  // Instancie le monstre � la position calcul�e
        }
    }

    // S�lectionne un spawn de monstre al�atoire en fonction des probabilit�s configur�es
    public int GetRandomSpawnIndex()
    {
        if (spawns == null || spawns.Count == 0)
            return -1;  // Si la liste de spawns est vide, retourne -1

        int totalWeight = 0;
        foreach (var spawn in spawns)
        {
            totalWeight += spawn.percentage;  // Calcule le poids total des probabilit�s
        }

        if (totalWeight == 0)
            return -1;  // Si les probabilit�s ne sont pas configur�es, retourne -1

        int randomValue = Random.Range(0, totalWeight);  // Tire un nombre al�atoire entre 0 et le poids total
        int cumulativeWeight = 0;

        for (int i = 0; i < spawns.Count; i++)
        {
            cumulativeWeight += spawns[i].percentage;  // Cumul des poids
            if (randomValue < cumulativeWeight)
            {
                return i;  // Retourne l'indice du spawn s�lectionn�
            }
        }

        return -1;  // Cas improbable si les probabilit�s sont correctement configur�es
    }

    // Lorsque le joueur entre dans la zone de d�clenchement du spawner
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Si l'objet entrant est le joueur
        {
            SetActivate(true);  // Active le spawner
            Spawn();  // Lance un spawn imm�diatement
        }
    }

    // Lorsque le joueur sort de la zone de d�clenchement du spawner
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // Si l'objet sortant est le joueur
        {
            SetActivate(false);  // D�sactive le spawner
        }
    }

    // Coroutine qui s'ex�cute en continu pour spawn des monstres � intervalle r�gulier
    IEnumerator spawnCorountine()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsBetweenSpawns);  // Attend un d�lai entre chaque spawn
            if (activate)  // Si le spawner est activ�
            {
                Spawn();  // Effectue un spawn
            }
        }
    }
}
