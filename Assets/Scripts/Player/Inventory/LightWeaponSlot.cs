using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("#Pandemonium/Player/Inventory/LightWeaponSlot")]
public class LightWeaponSlot : InventorySlot
{
    // Vérifie si l'objet déposé est une arme légère avant de l'accepter.
    public override void OnDrop(PointerEventData eventData)
    {
        InventoryItem _inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        if (_inventoryItem.item.itemType as WeaponItem)
        {
            WeaponItem item = (WeaponItem)_inventoryItem.item.itemType;
            if (item.type == WeaponType.Legere)
            {
                base.OnDrop(eventData);
            }
        }
    }

    // Met à jour l'arme équipée lorsqu'un objet est affecté à ce slot.
    public override void onChangeItemFunc(ItemEntry item)
    {
        base.onChangeItemFunc(item);
        InventoryManager.instance.OnChangeEquipedWeapon();
    }
}
