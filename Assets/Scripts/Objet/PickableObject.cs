using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("#Pandemonium/Objet/PickableObject")]
public class PickableObject : MonoBehaviour
{
    // L'�l�ment principal que l'objet repr�sente dans l'inventaire
    public ItemEntry item;

    // Liste des ressources suppl�mentaires que cet objet peut donner al�atoirement
    public List<MoreRessources> ressources;

    // M�thode appel�e lors de l'interaction avec l'objet
    void Interact()
    {
        // Ajout de l'�l�ment principal � l'inventaire
        InventoryManager.instance.AddItem(item, this);

        // Parcours des ressources suppl�mentaires et ajout al�atoire � l'inventaire
        foreach (MoreRessources r in ressources)
        {
            // G�n�re un nombre al�atoire entre 0 et 100
            int n = Random.Range(0, 100);

            // Si le nombre est sup�rieur ou �gal au pourcentage de la ressource, on l'ajoute � l'inventaire
            if (n >= r.percentage)
            {
                InventoryManager.instance.AddItem(r.item, r.quantity);
            }
        }
    }

    // M�thode appel�e lorsque l'objet est d�truit (par exemple lorsqu'il est ramass�)
    private void OnDestroy()
    {
        // Indique � l'InventoryManager que l'objet n'est plus en train d'�tre ramass�
        InventoryManager.instance.setGrab(false);
    }
}
