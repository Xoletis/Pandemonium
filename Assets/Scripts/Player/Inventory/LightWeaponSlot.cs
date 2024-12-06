using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("#Pandemonium/Player/Inventory/LightWeaponSlot")]
public class LightWeaponSlot : InventorySlot
{
    // V�rifie si l'objet d�pos� est une arme l�g�re avant de l'accepter.
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

    // Met � jour l'arme �quip�e lorsqu'un objet est affect� � ce slot.
    public override void onChangeItemFunc(ItemEntry item)
    {
        base.onChangeItemFunc(item);
        InventoryManager.instance.OnChangeEquipedWeapon();
    }
}
