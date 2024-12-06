using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("#Pandemonium/GameManager/CraftManager")]
public class CraftManager : MonoBehaviour
{
    public static CraftManager Instance;  // Instance statique pour accéder facilement à cette classe

    public List<CraftBlueprint> crafts;  // Liste de tous les plans de fabrication (blueprints)

    // Méthode appelée au démarrage de l'objet (lors de l'initialisation de la scène)
    private void Awake()
    {
        // Vérifie qu'il n'y a pas plusieurs instances de CraftManager dans la scène
        if (Instance != null)
        {
            Debug.LogError("Il y a plusieurs CraftManager dans la scene");  // Affiche un message d'erreur s'il y a plusieurs instances
        }

        Instance = this;  // Attribue cette instance à l'instance statique, pour y accéder globalement
    }

    // Méthode pour initialiser les recettes de fabrication (crafting)
    public void Initialize()
    {
        // Parcourt tous les objets dans ItemManager et ajoute ceux qui sont craftables à la liste des recettes
        foreach (Item item in ItemManager.Instance.allItems)
        {
            if (item.isCraftable)  // Vérifie si l'objet peut être fabriqué
            {
                // Ajoute un plan de fabrication (CraftBlueprint) pour cet objet à la liste
                crafts.Add(new CraftBlueprint(item, true));
            }
        }
    }
}
