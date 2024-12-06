using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("#Pandemonium/Objet/PickableObject")]
public class PickableObject : MonoBehaviour
{
    // L'élément principal que l'objet représente dans l'inventaire
    public ItemEntry item;

    // Liste des ressources supplémentaires que cet objet peut donner aléatoirement
    public List<MoreRessources> ressources;

    // Méthode appelée lors de l'interaction avec l'objet
    void Interact()
    {
        // Ajout de l'élément principal à l'inventaire
        InventoryManager.instance.AddItem(item, this);

        // Parcours des ressources supplémentaires et ajout aléatoire à l'inventaire
        foreach (MoreRessources r in ressources)
        {
            // Génère un nombre aléatoire entre 0 et 100
            int n = Random.Range(0, 100);

            // Si le nombre est supérieur ou égal au pourcentage de la ressource, on l'ajoute à l'inventaire
            if (n >= r.percentage)
            {
                InventoryManager.instance.AddItem(r.item, r.quantity);
            }
        }
    }

    // Méthode appelée lorsque l'objet est détruit (par exemple lorsqu'il est ramassé)
    private void OnDestroy()
    {
        // Indique à l'InventoryManager que l'objet n'est plus en train d'être ramassé
        InventoryManager.instance.setGrab(false);
    }
}
