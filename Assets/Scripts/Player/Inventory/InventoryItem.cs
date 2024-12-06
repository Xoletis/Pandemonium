using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/Player/Inventory/InventoryItem")] // Ajoute ce script au menu des composants Unity
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // Référence à l'image de l'objet dans l'interface utilisateur
    public Image image;

    // Texte affichant la quantité de l'objet
    public TextMeshProUGUI textCount;

    // Entrée d'inventaire associée à cet objet
    public ItemEntry item;

    // Référence au gestionnaire de mise à jour des statistiques
    private ItemStatUpdate Stat;

    // Parent de l'objet avant le début d'un drag
    [HideInInspector] public Transform parentAfterDrag;

    private void Start()
    {
        // Initialise l'objet avec les données de l'item
        InitialiseItem(item);

        // Récupère la référence à l'élément de mise à jour des statistiques
        Stat = InventoryManager.instance.ItemDescription.GetComponent<ItemStatUpdate>();
    }

    // Début du drag (déplacement de l'objet)
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Ignore si ce n'est pas un clic gauche
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // Désactive le raycast pour permettre le drag
        image.raycastTarget = false;

        // Sauvegarde le parent actuel pour revenir à la position initiale après le drag
        parentAfterDrag = transform.parent;

        // Déplace cet objet au sommet de la hiérarchie (root)
        transform.SetParent(transform.root);

        // Vide le slot actuel
        Slots actualSlot = InventoryManager.instance.FindSlotByInventorySlot(parentAfterDrag.GetComponent<InventorySlot>());
        if (actualSlot != null)
        {
            actualSlot.itemEntry = new ItemEntry();
        }
        else if (parentAfterDrag.GetComponent<InventorySlot>() == InventoryManager.instance.heavyWeapon.Slot)
        {
            InventoryManager.instance.heavyWeapon.itemEntry = new ItemEntry();
        }
        else if (parentAfterDrag.GetComponent<InventorySlot>() == InventoryManager.instance.mediumWeapon.Slot)
        {
            InventoryManager.instance.mediumWeapon.itemEntry = new ItemEntry();
        }
        else if (parentAfterDrag.GetComponent<InventorySlot>() == InventoryManager.instance.lightWeapon.Slot)
        {
            InventoryManager.instance.lightWeapon.itemEntry = new ItemEntry();
        }

        // Met à jour l'objet dans le slot
        parentAfterDrag.GetComponent<InventorySlot>().item = new ItemEntry();

        // Si l'objet est une arme, change la couleur des slots compatibles
        if (item.itemType as WeaponItem)
        {
            WeaponItem weaponItem = (WeaponItem)item.itemType;
            if (weaponItem.type == WeaponType.Lourde)
            {
                InventoryManager.instance.ChangeSlotColor(true, 0);
            }
            else if (weaponItem.type == WeaponType.Moyenne)
            {
                InventoryManager.instance.ChangeSlotColor(true, 1);
            }
            else if (weaponItem.type == WeaponType.Legere)
            {
                InventoryManager.instance.ChangeSlotColor(true, 2);
            }
        }
    }

    // Gère le déplacement de l'objet pendant le drag
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // Suit la position de la souris
        transform.position = Input.mousePosition;
    }

    // Fin du drag
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // Réactive le raycast pour permettre l'interaction
        image.raycastTarget = true;

        // Replace l'objet dans son parent original
        transform.SetParent(parentAfterDrag);

        // Restaure l'entrée dans le slot correspondant
        Slots actualSlot = InventoryManager.instance.FindSlotByInventorySlot(parentAfterDrag.GetComponent<InventorySlot>());
        if (actualSlot != null)
        {
            actualSlot.itemEntry = item;
        }
        else if (parentAfterDrag.GetComponent<InventorySlot>() == InventoryManager.instance.heavyWeapon.Slot)
        {
            InventoryManager.instance.heavyWeapon.itemEntry = item;
        }
        else if (parentAfterDrag.GetComponent<InventorySlot>() == InventoryManager.instance.mediumWeapon.Slot)
        {
            InventoryManager.instance.mediumWeapon.itemEntry = item;
        }
        else if (parentAfterDrag.GetComponent<InventorySlot>() == InventoryManager.instance.lightWeapon.Slot)
        {
            InventoryManager.instance.lightWeapon.itemEntry = item;
        }

        // Met à jour l'objet dans le slot
        parentAfterDrag.GetComponent<InventorySlot>().item = item;
        parentAfterDrag.GetComponent<InventorySlot>().SetUp();

        // Réinitialise les couleurs des slots
        InventoryManager.instance.ChangeSlotColor(false);
    }

    // Initialise les données de l'objet dans l'interface utilisateur
    public void InitialiseItem(ItemEntry newItem)
    {
        // Définit l'image et la quantité de l'objet
        image.sprite = newItem.itemType.image;
        textCount.text = newItem.number.ToString();

        // Si l'objet est une arme non à distance, ne montre pas de texte pour la quantité
        if (newItem.itemType as WeaponItem)
        {
            WeaponItem weaponItem = newItem.itemType as WeaponItem;
            if (!weaponItem.isRangeWeapon)
            {
                textCount.text = "";
            }
        }

        // Met à jour les données de l'item
        item = newItem;
    }

    // Gère les clics sur l'objet
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Scinde la pile d'objets en deux
            InventorySlot parentSlot = transform.parent.GetComponent<InventorySlot>();
            parentSlot.SplitStack();
        }
    }

    // Montre la description de l'objet lorsque la souris passe dessus
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item.itemType != null)
        {
            InventoryManager.instance.ItemDescription.SetActive(true);
            Stat.UpdateStats(item.itemType);
        }
    }

    // Cache la description de l'objet lorsque la souris quitte l'objet
    public void OnPointerExit(PointerEventData eventData)
    {
        if (item.itemType != null)
        {
            InventoryManager.instance.ItemDescription.SetActive(false);
        }
    }
}
