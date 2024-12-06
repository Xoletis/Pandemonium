using UnityEngine;
using UnityEngine.EventSystems;

// Classe repr�sentant un emplacement pour armes moyennes.
[AddComponentMenu("#Pandemonium/Player/Inventory/MediumWeaponSlot")]
public class MediumWeaponSlot : InventorySlot
{
    // V�rifie si l'objet d�pos� est une arme moyenne avant de l'accepter.
    public override void OnDrop(PointerEventData eventData)
    {
        InventoryItem _inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        if (_inventoryItem.item.itemType as WeaponItem)
        {
            WeaponItem item = (WeaponItem)_inventoryItem.item.itemType;
            if (item.type == WeaponType.Moyenne)
            {
                base.OnDrop(eventData);
            }
        }
    }

    // Met � jour l'arme �quip�e lorsqu'un objet est affect� � ce slot.
    public override void onChangeItemFunc(ItemEntry item)
    {
        base.onChangeItemFunc(item);
        InventoryManager.instance.OnChangeEquipedWeapon();
    }
}
