using System.Collections.Generic;
using TMPro;
using UnityEngine;

[AddComponentMenu("#Pandemonium/GameManager/GameManager")]
public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI objectifNb;  // Affiche le nombre de portails à détruire

    public List<GameObject> Portails;  // Liste des portails dans le jeu

    public static GameManager instance;  // Instance statique pour accéder facilement à ce gestionnaire

    public GameObject VictoryMenu, DefetMenu;  // Menus de victoire et de défaite à afficher en fonction de l'état du jeu

    public ThirdPersonController controller;  // Référence au contrôleur de personnage (pour verrouiller la caméra lors des menus)

    // Méthode d'initialisation (appelée lors du démarrage de l'objet)
    private void Awake()
    {
        // Vérifie qu'il n'y a pas plusieurs instances de GameManager dans la scène
        if (instance != null)
        {
            Debug.LogError("Il y a plusieurs instances de Game Manager");  // Message d'erreur si une autre instance existe déjà
        }

        instance = this;  // Assigne cette instance à la variable statique `instance` pour un accès global
    }

    // Méthode d'initialisation des objets et paramètres au début du jeu
    private void Start()
    {
        // Récupère tous les objets ayant le tag "Spawner" (probablement des portails)
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Spawner");

        // Ajoute chaque portail à la liste `Portails`
        foreach (GameObject go in gos)
        {
            Portails.Add(go);
        }

        // Met à jour le texte du nombre d'objectifs avec le nombre de portails à détruire
        objectifNb.text = Portails.Count.ToString();

        // Désactive les menus de victoire et de défaite au début du jeu
        VictoryMenu.SetActive(false);
        DefetMenu.SetActive(false);
    }

    // Méthode pour supprimer un portail lorsque celui-ci est détruit
    public void RemovePortal(GameObject obj)
    {
        // Retire le portail de la liste `Portails`
        Portails.Remove(obj);

        // Met à jour le texte du nombre d'objectifs restants
        objectifNb.text = Portails.Count.ToString();

        // Si tous les portails ont été détruits, affiche le menu de victoire
        if (Portails.Count <= 0)
        {
            VictoryMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;  // Déverrouille le curseur
            Cursor.visible = true;  // Rends le curseur visible
            Time.timeScale = 0;  // Met le jeu en pause
            controller.LockCameraPosition = true;  // Verrouille la caméra pour éviter qu'elle bouge
        }
    }

    // Méthode appelée en cas de défaite
    public void Defete()
    {
        // Affiche le menu de défaite
        DefetMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;  // Déverrouille le curseur
        Cursor.visible = true;  // Rends le curseur visible
        Time.timeScale = 0;  // Met le jeu en pause
        controller.LockCameraPosition = true;  // Verrouille la caméra pour éviter qu'elle bouge
    }
}
