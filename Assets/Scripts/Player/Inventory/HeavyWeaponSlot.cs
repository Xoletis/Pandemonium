using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("#Pandemonium/Player/Inventory/HeavyWeaponSlot")] // Ajoute ce script au menu des composants Unity
public class HeavyWeaponSlot : InventorySlot // Hérite des fonctionnalités de la classe InventorySlot
{
    // Méthode appelée lorsqu'un objet est lâché dans ce slot
    public override void OnDrop(PointerEventData eventData)
    {
        // Récupérer l'objet d'inventaire attaché à l'objet en train d'être déplacé
        InventoryItem _inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        // Vérifier si l'objet est une arme (WeaponItem)
        if (_inventoryItem.item.itemType as WeaponItem)
        {
            // Convertir l'objet en WeaponItem pour accéder à ses propriétés
            WeaponItem item = (WeaponItem)_inventoryItem.item.itemType;

            // Vérifier si l'arme est de type "Lourde"
            if (item.type == WeaponType.Lourde)
            {
                base.OnDrop(eventData); // Appeler la méthode parent pour gérer le drop
            }
        }
    }

    // Méthode appelée lorsqu'un objet change dans le slot
    public override void onChangeItemFunc(ItemEntry item)
    {
        base.onChangeItemFunc(item); // Appeler la méthode parent pour mettre à jour le slot

        // Notifier le gestionnaire d'inventaire que l'arme équipée a changé
        InventoryManager.instance.OnChangeEquipedWeapon();
    }
}
