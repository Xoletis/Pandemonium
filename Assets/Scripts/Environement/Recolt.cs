using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("#Pandemonium/Environement/Recolt")]
public class Recolt : MonoBehaviour
{
    public Item recolt;  // L'objet ou ressource r�colt�e (par exemple, un item que le joueur peut r�colter).
    public WeaponType typeForRecolt;  // Le type d'outil n�cessaire pour r�colter (par exemple, une hache ou une pioche).
    public int count;  // La quantit� disponible de la ressource � r�colter.
    public List<MoreRessources> ressources;  // Une liste d'autres ressources qui peuvent �tre r�colt�es lors de l'action, avec une probabilit�.

    // Cette m�thode est appel�e lorsque l'objet entre en collision avec une autre zone (comme la zone d'attaque du joueur).
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AttackZone"))  // Si l'objet qui entre en collision est une zone d'attaque.
        {
            int weapon = InventoryManager.instance.SlotUse;  // R�cup�re le type d'arme actuellement �quip�e dans l'inventaire.
            if (CanHarve(weapon))  // V�rifie si l'arme �quip�e est ad�quate pour r�colter la ressource.
            {
                int damage = InventoryManager.instance.GetDamage();  // R�cup�re les d�g�ts inflig�s par l'arme �quip�e.

                // Si les d�g�ts sont inf�rieurs ou �gaux � la quantit� restante � r�colter.
                if (damage <= count)
                {
                    InventoryManager.instance.AddItem(recolt, damage);  // Ajoute � l'inventaire la ressource r�colt�e.
                    count -= damage;  // Diminue la quantit� restante � r�colter.
                }
                else  // Si les d�g�ts sont plus �lev�s que la quantit� restante.
                {
                    InventoryManager.instance.AddItem(recolt, count);  // Ajoute la quantit� restante � l'inventaire.
                    count = 0;  // Met � z�ro la quantit� restante � r�colter.
                }

                // Cette boucle ajoute des ressources suppl�mentaires avec une probabilit� sp�cifique.
                foreach (MoreRessources r in ressources)
                {
                    int n = Random.Range(0, 100);  // G�n�re un nombre al�atoire entre 0 et 100.
                    if (n >= r.percentage)
                    {  // Si ce nombre est plus grand que la probabilit� de la ressource.
                        InventoryManager.instance.AddItem(r.item, r.quantity);  // Ajoute l'objet et la quantit� � l'inventaire.
                    }
                }

                // Si la quantit� de la ressource � r�colter est �puis�e.
                if (count <= 0)
                {
                    Destroy(gameObject);  // D�truit l'objet de r�colte (par exemple, la plante ou le minerai).
                }
            }
        }
    }

    // V�rifie si le type d'outil �quip� est compatible avec le type de ressource � r�colter.
    public bool CanHarve(int weapon)
    {
        switch (typeForRecolt)
        {
            case WeaponType.Axe:  // Si l'objet � r�colter n�cessite une hache.
                {
                    return weapon == 3;  // Si l'arme �quip�e est une hache (par exemple, ID 3).
                }
            case WeaponType.Pickaxe:  // Si l'objet � r�colter n�cessite une pioche.
                {
                    return weapon == 4;  // Si l'arme �quip�e est une pioche (par exemple, ID 4).
                }
            default: return false;  // Si le type d'outil n'est pas d�fini, retourne false.
        }
    }
}
