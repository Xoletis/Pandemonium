using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("#Pandemonium/Environement/Recolt")]
public class Recolt : MonoBehaviour
{
    public Item recolt;  // L'objet ou ressource récoltée (par exemple, un item que le joueur peut récolter).
    public WeaponType typeForRecolt;  // Le type d'outil nécessaire pour récolter (par exemple, une hache ou une pioche).
    public int count;  // La quantité disponible de la ressource à récolter.
    public List<MoreRessources> ressources;  // Une liste d'autres ressources qui peuvent être récoltées lors de l'action, avec une probabilité.

    // Cette méthode est appelée lorsque l'objet entre en collision avec une autre zone (comme la zone d'attaque du joueur).
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AttackZone"))  // Si l'objet qui entre en collision est une zone d'attaque.
        {
            int weapon = InventoryManager.instance.SlotUse;  // Récupère le type d'arme actuellement équipée dans l'inventaire.
            if (CanHarve(weapon))  // Vérifie si l'arme équipée est adéquate pour récolter la ressource.
            {
                int damage = InventoryManager.instance.GetDamage();  // Récupère les dégâts infligés par l'arme équipée.

                // Si les dégâts sont inférieurs ou égaux à la quantité restante à récolter.
                if (damage <= count)
                {
                    InventoryManager.instance.AddItem(recolt, damage);  // Ajoute à l'inventaire la ressource récoltée.
                    count -= damage;  // Diminue la quantité restante à récolter.
                }
                else  // Si les dégâts sont plus élevés que la quantité restante.
                {
                    InventoryManager.instance.AddItem(recolt, count);  // Ajoute la quantité restante à l'inventaire.
                    count = 0;  // Met à zéro la quantité restante à récolter.
                }

                // Cette boucle ajoute des ressources supplémentaires avec une probabilité spécifique.
                foreach (MoreRessources r in ressources)
                {
                    int n = Random.Range(0, 100);  // Génère un nombre aléatoire entre 0 et 100.
                    if (n >= r.percentage)
                    {  // Si ce nombre est plus grand que la probabilité de la ressource.
                        InventoryManager.instance.AddItem(r.item, r.quantity);  // Ajoute l'objet et la quantité à l'inventaire.
                    }
                }

                // Si la quantité de la ressource à récolter est épuisée.
                if (count <= 0)
                {
                    Destroy(gameObject);  // Détruit l'objet de récolte (par exemple, la plante ou le minerai).
                }
            }
        }
    }

    // Vérifie si le type d'outil équipé est compatible avec le type de ressource à récolter.
    public bool CanHarve(int weapon)
    {
        switch (typeForRecolt)
        {
            case WeaponType.Axe:  // Si l'objet à récolter nécessite une hache.
                {
                    return weapon == 3;  // Si l'arme équipée est une hache (par exemple, ID 3).
                }
            case WeaponType.Pickaxe:  // Si l'objet à récolter nécessite une pioche.
                {
                    return weapon == 4;  // Si l'arme équipée est une pioche (par exemple, ID 4).
                }
            default: return false;  // Si le type d'outil n'est pas défini, retourne false.
        }
    }
}
