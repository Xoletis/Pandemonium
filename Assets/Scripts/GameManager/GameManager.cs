using System.Collections.Generic;
using TMPro;
using UnityEngine;

[AddComponentMenu("#Pandemonium/GameManager/GameManager")]
public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI objectifNb;  // Affiche le nombre de portails � d�truire

    public List<GameObject> Portails;  // Liste des portails dans le jeu

    public static GameManager instance;  // Instance statique pour acc�der facilement � ce gestionnaire

    public GameObject VictoryMenu, DefetMenu;  // Menus de victoire et de d�faite � afficher en fonction de l'�tat du jeu

    public ThirdPersonController controller;  // R�f�rence au contr�leur de personnage (pour verrouiller la cam�ra lors des menus)

    // M�thode d'initialisation (appel�e lors du d�marrage de l'objet)
    private void Awake()
    {
        // V�rifie qu'il n'y a pas plusieurs instances de GameManager dans la sc�ne
        if (instance != null)
        {
            Debug.LogError("Il y a plusieurs instances de Game Manager");  // Message d'erreur si une autre instance existe d�j�
        }

        instance = this;  // Assigne cette instance � la variable statique `instance` pour un acc�s global
    }

    // M�thode d'initialisation des objets et param�tres au d�but du jeu
    private void Start()
    {
        // R�cup�re tous les objets ayant le tag "Spawner" (probablement des portails)
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Spawner");

        // Ajoute chaque portail � la liste `Portails`
        foreach (GameObject go in gos)
        {
            Portails.Add(go);
        }

        // Met � jour le texte du nombre d'objectifs avec le nombre de portails � d�truire
        objectifNb.text = Portails.Count.ToString();

        // D�sactive les menus de victoire et de d�faite au d�but du jeu
        VictoryMenu.SetActive(false);
        DefetMenu.SetActive(false);
    }

    // M�thode pour supprimer un portail lorsque celui-ci est d�truit
    public void RemovePortal(GameObject obj)
    {
        // Retire le portail de la liste `Portails`
        Portails.Remove(obj);

        // Met � jour le texte du nombre d'objectifs restants
        objectifNb.text = Portails.Count.ToString();

        // Si tous les portails ont �t� d�truits, affiche le menu de victoire
        if (Portails.Count <= 0)
        {
            VictoryMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;  // D�verrouille le curseur
            Cursor.visible = true;  // Rends le curseur visible
            Time.timeScale = 0;  // Met le jeu en pause
            controller.LockCameraPosition = true;  // Verrouille la cam�ra pour �viter qu'elle bouge
        }
    }

    // M�thode appel�e en cas de d�faite
    public void Defete()
    {
        // Affiche le menu de d�faite
        DefetMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;  // D�verrouille le curseur
        Cursor.visible = true;  // Rends le curseur visible
        Time.timeScale = 0;  // Met le jeu en pause
        controller.LockCameraPosition = true;  // Verrouille la cam�ra pour �viter qu'elle bouge
    }
}
