using System;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("#Pandemonium/Player/Inventory/InventorySlot")]
public class InventorySlot : MonoBehaviour, IDropHandler
{
    // L'item actuellement contenu dans ce slot
    [SerializeField]
    private ItemEntry _item;

    // �v�nement d�clench� lorsque l'item change
    public Action<ItemEntry> onItemChange;

    // Propri�t� pour acc�der et modifier l'item dans ce slot
    [SerializeField]
    public ItemEntry item
    {
        get => _item; // Retourne l'item actuel
        set
        {
            if (_item != value) // V�rifie si l'item a chang�
            {
                _item = value;
                onItemChange?.Invoke(_item); // D�clenche l'�v�nement
            }
        }
    }

    private void Start()
    {
        // Abonne une fonction � l'�v�nement de changement d'item
        onItemChange += onChangeItemFunc;
    }

    // Fonction virtuelle appel�e lorsque l'item change
    public virtual void onChangeItemFunc(ItemEntry item) { }

    // Configure visuellement le slot en fonction de l'item qu'il contient
    public void SetUp()
    {
        // Supprime les anciens objets visuels associ�s au slot
        foreach (Transform child in transform)
        {
            if (child.GetComponent<InventoryItem>())
            {
                Destroy(child.gameObject);
            }
        }

        // Si le slot contient un item, cr�e un visuel pour cet item
        if (_item.itemType != null)
        {
            GameObject _item = Instantiate(InventoryManager.instance.ItemCase, transform);
            _item.GetComponent<InventoryItem>().InitialiseItem(this._item);
        }
    }

    // G�re le drag-and-drop d'un objet sur ce slot
    public virtual void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0) // Si le slot est vide
        {
            InventoryItem _inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            _inventoryItem.parentAfterDrag = transform; // Place l'objet dans ce slot
        }
        else // Si le slot contient d�j� un item
        {
            InventoryItem _inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();

            // Ignore les armes (elles ne peuvent pas �tre empil�es)
            if (_inventoryItem.item.itemType as WeaponItem) { return; }

            // Si l'objet est du m�me type que celui dans le slot
            if (_item.itemType == _inventoryItem.item.itemType)
            {
                // Si la somme des deux piles d�passe la capacit� maximale
                if ((_inventoryItem.item.number + _item.number) > _item.itemType.maxStack)
                {
                    int nb = _item.number - (_item.itemType.maxStack - _inventoryItem.item.number);
                    _item.number = _item.itemType.maxStack;
                    _inventoryItem.item.number = nb;
                    SetUp(); // Met � jour visuellement le slot
                }
                else // Sinon, ajoute simplement les deux piles
                {
                    _item.number += _inventoryItem.item.number;
                    Destroy(_inventoryItem.gameObject); // D�truit l'objet d�plac�
                    SetUp(); // Met � jour visuellement le slot
                }
            }
        }
    }

    // Divise une pile d'objets en deux
    public void SplitStack()
    {
        // Ne divise pas si l'objet est une arme ou s'il n'y a qu'un seul item
        if (_item.itemType as WeaponItem) return;
        if (_item.number == 1) return;

        // V�rifie si l'inventaire est plein
        if (InventoryManager.instance.isFull()) return;

        // Logique pour diviser une pile en deux
        ItemEntry newItem = new ItemEntry();
        newItem.itemType = _item.itemType;

        if (_item.number % 2 == 0) // Si le nombre est pair
        {
            _item.number /= 2;
            newItem.number = _item.number;
        }
        else // Si le nombre est impair
        {
            _item.number /= 2;
            newItem.number = _item.number + 1;
        }

        // Met � jour visuellement le slot et ajoute le nouvel item dans un slot libre
        SetUp();
        InventoryManager.instance.AddItemInNewSlot(newItem);
    }
}
