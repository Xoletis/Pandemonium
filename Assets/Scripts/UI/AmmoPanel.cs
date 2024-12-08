using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/UI/AmmoPanel")]
public class AmmoPanel : MonoBehaviour
{
    public Image ammoType; // Image de l'ic�ne repr�sentant le type de munitions.
    public TextMeshProUGUI ammoNb; // Texte affichant le nombre de munitions actuelles.
    public TextMeshProUGUI ammoInInventory; // Texte affichant le nombre total de munitions dans l'inventaire.

    // Appel�e lorsque l'�l�ment UI devient actif.
    private void OnEnable()
    {
        Refresh(); // Actualise l'affichage d�s que le panneau devient visible.
    }

    // Actualise le contenu du panneau de munitions.
    public void Refresh()
    {
        // D�termine le slot d'arme actuellement utilis� par le joueur (par exemple, arme lourde, moyenne, l�g�re).
        Slots slot;
        switch (InventoryManager.instance.SlotUse)
        {
            case 0: slot = InventoryManager.instance.heavyWeapon; break;
            case 1: slot = InventoryManager.instance.mediumWeapon; break;
            case 2: slot = InventoryManager.instance.lightWeapon; break;
            default: slot = null; break;
        }

        if (slot.itemEntry.itemType == null) return;

        // R�cup�re l'arme du slot et l'utilise pour r�cup�rer les informations sur les munitions.
        WeaponItem item = slot.itemEntry.itemType as WeaponItem;

        // Si le slot est nul (aucune arme �quip�e), on quitte la fonction.
        if (item == null) return;
        if (!item.isRangeWeapon) return;

        // Met � jour l'image de type de munitions.
        ammoType.sprite = item.ammo.image;

        // Met � jour le texte indiquant le nombre de munitions dans le slot actuel.
        ammoNb.text = slot.itemEntry.number + ""; // Affiche le nombre de munitions dans le slot.

        // Met � jour le texte indiquant le nombre total de munitions de ce type dans l'inventaire.
        ammoInInventory.text = InventoryManager.instance.GetNbItemInInventory(item.ammo) + ""; // Affiche le nombre total de munitions en inventaire.
    }
}
