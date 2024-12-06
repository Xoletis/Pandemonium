using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("#Pandemonium/GameManager/CraftManager")]
public class CraftManager : MonoBehaviour
{
    public static CraftManager Instance;  // Instance statique pour acc�der facilement � cette classe

    public List<CraftBlueprint> crafts;  // Liste de tous les plans de fabrication (blueprints)

    // M�thode appel�e au d�marrage de l'objet (lors de l'initialisation de la sc�ne)
    private void Awake()
    {
        // V�rifie qu'il n'y a pas plusieurs instances de CraftManager dans la sc�ne
        if (Instance != null)
        {
            Debug.LogError("Il y a plusieurs CraftManager dans la scene");  // Affiche un message d'erreur s'il y a plusieurs instances
        }

        Instance = this;  // Attribue cette instance � l'instance statique, pour y acc�der globalement
    }

    // M�thode pour initialiser les recettes de fabrication (crafting)
    public void Initialize()
    {
        // Parcourt tous les objets dans ItemManager et ajoute ceux qui sont craftables � la liste des recettes
        foreach (Item item in ItemManager.Instance.allItems)
        {
            if (item.isCraftable)  // V�rifie si l'objet peut �tre fabriqu�
            {
                // Ajoute un plan de fabrication (CraftBlueprint) pour cet objet � la liste
                crafts.Add(new CraftBlueprint(item, true));
            }
        }
    }
}
