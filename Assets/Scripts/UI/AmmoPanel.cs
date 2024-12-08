using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/UI/AmmoPanel")]
public class AmmoPanel : MonoBehaviour
{
    public Image ammoType; // Image de l'icône représentant le type de munitions.
    public TextMeshProUGUI ammoNb; // Texte affichant le nombre de munitions actuelles.
    public TextMeshProUGUI ammoInInventory; // Texte affichant le nombre total de munitions dans l'inventaire.

    // Appelée lorsque l'élément UI devient actif.
    private void OnEnable()
    {
        Refresh(); // Actualise l'affichage dès que le panneau devient visible.
    }

    // Actualise le contenu du panneau de munitions.
    public void Refresh()
    {
        // Détermine le slot d'arme actuellement utilisé par le joueur (par exemple, arme lourde, moyenne, légère).
        Slots slot;
        switch (InventoryManager.instance.SlotUse)
        {
            case 0: slot = InventoryManager.instance.heavyWeapon; break;
            case 1: slot = InventoryManager.instance.mediumWeapon; break;
            case 2: slot = InventoryManager.instance.lightWeapon; break;
            default: slot = null; break;
        }

        if (slot.itemEntry.itemType == null) return;

        // Récupère l'arme du slot et l'utilise pour récupérer les informations sur les munitions.
        WeaponItem item = slot.itemEntry.itemType as WeaponItem;

        // Si le slot est nul (aucune arme équipée), on quitte la fonction.
        if (item == null) return;
        if (!item.isRangeWeapon) return;

        // Met à jour l'image de type de munitions.
        ammoType.sprite = item.ammo.image;

        // Met à jour le texte indiquant le nombre de munitions dans le slot actuel.
        ammoNb.text = slot.itemEntry.number + ""; // Affiche le nombre de munitions dans le slot.

        // Met à jour le texte indiquant le nombre total de munitions de ce type dans l'inventaire.
        ammoInInventory.text = InventoryManager.instance.GetNbItemInInventory(item.ammo) + ""; // Affiche le nombre total de munitions en inventaire.
    }
}
