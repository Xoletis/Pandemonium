using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("#Pandemonium/Player/Inventory/HeavyWeaponSlot")] // Ajoute ce script au menu des composants Unity
public class HeavyWeaponSlot : InventorySlot // H�rite des fonctionnalit�s de la classe InventorySlot
{
    // M�thode appel�e lorsqu'un objet est l�ch� dans ce slot
    public override void OnDrop(PointerEventData eventData)
    {
        // R�cup�rer l'objet d'inventaire attach� � l'objet en train d'�tre d�plac�
        InventoryItem _inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        // V�rifier si l'objet est une arme (WeaponItem)
        if (_inventoryItem.item.itemType as WeaponItem)
        {
            // Convertir l'objet en WeaponItem pour acc�der � ses propri�t�s
            WeaponItem item = (WeaponItem)_inventoryItem.item.itemType;

            // V�rifier si l'arme est de type "Lourde"
            if (item.type == WeaponType.Lourde)
            {
                base.OnDrop(eventData); // Appeler la m�thode parent pour g�rer le drop
            }
        }
    }

    // M�thode appel�e lorsqu'un objet change dans le slot
    public override void onChangeItemFunc(ItemEntry item)
    {
        base.onChangeItemFunc(item); // Appeler la m�thode parent pour mettre � jour le slot

        // Notifier le gestionnaire d'inventaire que l'arme �quip�e a chang�
        InventoryManager.instance.OnChangeEquipedWeapon();
    }
}
