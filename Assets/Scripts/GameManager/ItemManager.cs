using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("#Pandemonium/GameManager/ItemManager")]
public class ItemManager : MonoBehaviour
{
    public List<Item> allItems;  // Liste qui contiendra tous les objets de type Item

    public static ItemManager Instance;  // Instance statique pour un accès global à ce gestionnaire d'items

    // Méthode d'initialisation appelée lors de la création de l'objet
    private void Awake()
    {
        // Vérifie s'il existe déjà une instance de ItemManager dans la scène
        if (Instance != null)
        {
            Debug.LogError("Il y a plusieurs instances de ItemManager dans la scène");  // Message d'erreur si une instance existe déjà
        }

        Instance = this;  // Assigne cette instance comme l'instance unique du gestionnaire d'items
    }

    // Méthode appelée au démarrage du jeu
    private void Start()
    {
        LoadAllItems();  // Charge tous les items au début du jeu
    }

    // Méthode pour charger tous les objets de type Item dans la liste `allItems`
    private void LoadAllItems()
    {
        // Charge tous les objets de type Item situés dans le dossier Resources/Items
        Item[] items = Resources.LoadAll<Item>("Items");

        // Convertit le tableau d'items en une liste pour une gestion plus facile
        allItems = new List<Item>(items);

        // Affiche dans la console le nombre d'items chargés
        Debug.Log($"Ajouté {allItems.Count} items à la liste.");

        // Initialise le gestionnaire de craft en appelant sa méthode `Initialize`
        CraftManager.Instance.Initialize();
    }
}
