using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("#Pandemonium/GameManager/ItemManager")]
public class ItemManager : MonoBehaviour
{
    public List<Item> allItems;  // Liste qui contiendra tous les objets de type Item

    public static ItemManager Instance;  // Instance statique pour un acc�s global � ce gestionnaire d'items

    // M�thode d'initialisation appel�e lors de la cr�ation de l'objet
    private void Awake()
    {
        // V�rifie s'il existe d�j� une instance de ItemManager dans la sc�ne
        if (Instance != null)
        {
            Debug.LogError("Il y a plusieurs instances de ItemManager dans la sc�ne");  // Message d'erreur si une instance existe d�j�
        }

        Instance = this;  // Assigne cette instance comme l'instance unique du gestionnaire d'items
    }

    // M�thode appel�e au d�marrage du jeu
    private void Start()
    {
        LoadAllItems();  // Charge tous les items au d�but du jeu
    }

    // M�thode pour charger tous les objets de type Item dans la liste `allItems`
    private void LoadAllItems()
    {
        // Charge tous les objets de type Item situ�s dans le dossier Resources/Items
        Item[] items = Resources.LoadAll<Item>("Items");

        // Convertit le tableau d'items en une liste pour une gestion plus facile
        allItems = new List<Item>(items);

        // Affiche dans la console le nombre d'items charg�s
        Debug.Log($"Ajout� {allItems.Count} items � la liste.");

        // Initialise le gestionnaire de craft en appelant sa m�thode `Initialize`
        CraftManager.Instance.Initialize();
    }
}
