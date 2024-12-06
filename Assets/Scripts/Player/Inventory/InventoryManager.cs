using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/Player/Inventory/InventoryManager")]
public class InventoryManager : MonoBehaviour
{
    // R�f�rences aux GameObjects pour les �l�ments UI de l'inventaire
    public GameObject InventoryObject; // Fen�tre principale de l'inventaire
    public GameObject ItemCase; // Conteneur d'objets
    public GameObject Panel; // Panel g�n�ral
    public GameObject PlayerHand; // Position de la main du joueur pour tenir un objet
    public GameObject ItemDescription; // Description d'un objet s�lectionn�
    public GameObject AmmoPanel; // Panneau pour afficher les munitions
    public GameObject ItemFieldHistory; // Conteneur pour les �l�ments de l'historique des objets
    public GameObject ItemFieldPrefab; // Pr�fabriqu� pour un champ d'objet
    public ThirdPersonController player; // Contr�leur du joueur

    // Sprites et images pour les slots de l'inventaire
    public Sprite[] slotSprites; // Sprites des slots
    public Image[] slotImages; // Images des slots correspondants

    // R�f�rence � l'arme actuellement �quip�e
    [HideInInspector]
    public GameObject weaponInstance;

    // Syst�me d'entr�es pour g�rer l'inventaire et les armes
    private InputForPlayer _playerInput;

    // Objets sp�cifiques (ex. pioche, hache)
    public Item Pickaxe, axe;

    // Diff�rents slots d'armes (lourdes, moyennes, l�g�res)
    public Slots heavyWeapon;
    public Slots mediumWeapon;
    public Slots lightWeapon;

    // Slot actuellement s�lectionn�
    [HideInInspector]
    public int SlotUse;

    // Permet de contr�ler si le joueur peut d�filer entre les slots
    bool canScroll = true;

    // Texte pour afficher le nombre d'objets (ex. nombre de "sang")
    public TextMeshProUGUI sangNb;
    public Item sang; // R�f�rence � l'objet "sang"

    // Liste des slots d'inventaire
    public List<Slots> slots;

    // Instance singleton de l'InventoryManager
    public static InventoryManager instance;

    // Objet utilis� pour attraper des items
    public GameObject grab;

    // R�f�rence � l'attaque du joueur
    [HideInInspector] public PlayerAttack attack;

    private void Awake()
    {
        // Initialisation du syst�me d'entr�es
        _playerInput = new InputForPlayer();
        _playerInput.Player.Inventory.performed += OnInventoryAction; // Ouvrir/fermer l'inventaire
        _playerInput.Player.SwitchWeapon.performed += SwitchWeapon; // Changer d'arme
        _playerInput.Player.HeavyWeapon.performed += GetHeavyWeapon; // S�lectionner une arme lourde
        _playerInput.Player.MediumWeapon.performed += GetMediumWeapon; // S�lectionner une arme moyenne
        _playerInput.Player.LightWeapon.performed += GetLightWeapon; // S�lectionner une arme l�g�re
        _playerInput.Player.OpenWheel.started += OpenWeaponWheel; // Ouvrir la roue d'armes
        _playerInput.Player.OpenWheel.canceled += CloseWeaponWheel; // Fermer la roue d'armes
        _playerInput.Player.Escape.performed += OnInventoryQuit; // Quitter l'inventaire

        // V�rifier qu'une seule instance de l'InventoryManager est pr�sente
        if (instance != null)
        {
            Debug.LogError("Il y a plusieurs instances d'InventoryManager dans la sc�ne");
        }
        instance = this;

        // D�sactiver l'inventaire au d�marrage
        InventoryObject.SetActive(false);
    }

    private void Start()
    {
        // Initialiser l'�tat de l'UI et du jeu
        ItemDescription.SetActive(false); // Masquer la description
        SlotUse = 0; // Slot s�lectionn� par d�faut
        AmmoPanel.SetActive(false); // Masquer le panneau de munitions
        Cursor.visible = false; // Masquer le curseur
        Cursor.lockState = CursorLockMode.Locked; // Verrouiller le curseur
        Time.timeScale = 1; // Temps normal
        grab.SetActive(false); // D�sactiver l'objet de saisie
    }

    private void OnEnable()
    {
        // Activer le syst�me d'entr�es
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        // D�sactiver le syst�me d'entr�es
        _playerInput.Disable();
    }

    // Ajouter un objet � l'inventaire
    public void AddItem(ItemEntry item, PickableObject Object = null)
    {
        // Si l'objet est une arme, l'ajouter directement dans un nouveau slot
        if (item.itemType is WeaponItem)
        {
            AddItemInNewSlot(item);
            if (Object != null)
            {
                Destroy(Object.gameObject); // D�truire l'objet r�cup�r�
            }
            return;
        }

        // Cr�er une entr�e dans l'interface pour l'objet
        GameObject field = Instantiate(ItemFieldPrefab, ItemFieldHistory.transform);
        field.GetComponent<ItemField>().Create(item.itemType, item.number);

        // Essayer d'empiler l'objet dans les slots existants
        for (int i = 0; i < slots.Count; i++)
        {
            if (item.number <= 0) break; // Si aucun objet restant, arr�ter

            // Empiler dans un slot existant de m�me type
            if (slots[i].itemEntry.itemType == item.itemType)
            {
                int availableSpace = item.itemType.maxStack - slots[i].itemEntry.number;
                if (item.number > availableSpace)
                {
                    item.number -= availableSpace;
                    slots[i].itemEntry.number = item.itemType.maxStack;
                    AddItemInSlot(slots[i].itemEntry, i);
                }
                else
                {
                    slots[i].itemEntry.number += item.number;
                    AddItemInSlot(slots[i].itemEntry, i);
                    item.number = 0;
                }
            }
        }

        // Si l'objet ne peut pas �tre empil�, cr�er un nouveau slot
        if (item.number > 0)
        {
            if (AddItemInNewSlot(item) && Object != null)
            {
                Destroy(Object.gameObject);
                setGrab(false);
            }
        }

        // Actualiser l'UI et les munitions
        ActualiseInventory();
        RefreshUIAmmo();
    }

    // M�thode pour ajouter un objet � l'inventaire avec une quantit� sp�cifi�e
    public void AddItem(Item item, int Count)
    {
        // Cr�e un nouvel objet de type ItemEntry pour l'�l�ment � ajouter
        ItemEntry newItemEntry = new ItemEntry
        {
            // Affecte l'�l�ment � la propri�t� itemType de l'entr�e
            itemType = item,
            // Affecte la quantit� de l'objet � la propri�t� number de l'entr�e
            number = Count
        };

        // Appelle la m�thode AddItem en passant l'ItemEntry nouvellement cr��
        AddItem(newItemEntry);
    }


    // Ajouter un objet dans un nouveau slot d'inventaire
    public bool AddItemInNewSlot(ItemEntry item)
    {
        foreach (var _slot in slots)
        {
            if (item.number <= 0) break; // Si tous les objets ont �t� ajout�s, arr�ter

            // Ajouter dans un slot vide
            if (_slot.itemEntry.itemType == null)
            {
                int quantityToAdd = Mathf.Min(item.number, item.itemType.maxStack); // Quantit� pouvant �tre ajout�e
                ItemEntry newItemEntry = new ItemEntry
                {
                    itemType = item.itemType, // Type d'objet
                    number = quantityToAdd // Quantit� d'objets ajout�s
                };
                _slot.itemEntry = newItemEntry; // Mettre � jour le slot
                item.number -= quantityToAdd; // R�duire le nombre restant
            }
        }

        ActualiseInventory(); // Mettre � jour l'UI
        return item.number <= 0; // Retourner true si tout a �t� ajout�
    }

    // Ajouter un objet dans un slot existant
    public void AddItemInSlot(ItemEntry item, int slotID)
    {
        Slots actualSlot = slots[slotID]; // R�cup�rer le slot
        if (actualSlot.itemEntry == null)
        {
            // Si le slot est vide, assigner directement
            actualSlot.itemEntry = item;
        }
        else if (actualSlot.itemEntry.itemType == item.itemType)
        {
            // Si le type correspond, mettre � jour
            actualSlot.itemEntry = item;
        }
    }

    // Retirer un certain nombre d'objets d'un type sp�cifique
    public int RemoveItem(Item item, int nb)
    {
        if (GetNbItemInInventory(item) <= 0)
        {
            return nb; // Si l'objet n'existe pas, retourner le nombre � retirer
        }

        foreach (var _slot in slots)
        {
            if (_slot.itemEntry.itemType == item)
            {
                if (_slot.itemEntry.number > nb)
                {
                    _slot.itemEntry.number -= nb; // R�duire directement le nombre
                    return 0; // Tous les objets ont �t� retir�s
                }
                else if (_slot.itemEntry.number == nb)
                {
                    // Si le slot contient exactement le nombre � retirer
                    _slot.itemEntry.number = 1; // Remettre � 1 (ou 0 selon logique)
                    _slot.itemEntry.itemType = null; // Vider le slot
                    return 0;
                }
                else
                {
                    // Si le slot contient moins que le nombre � retirer
                    nb -= _slot.itemEntry.number; // R�duire le reste
                    _slot.itemEntry.number = 1; // R�initialiser le slot
                    _slot.itemEntry.itemType = null; // Vider le slot
                }
            }
        }
        return nb; // Retourner le nombre restant � retirer
    }


    // Changer d'arme avec l'input de d�filement
    private void SwitchWeapon(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<float>(); // Valeur de d�filement
        if (canScroll)
        {
            if (scrollValue > 0)
            {
                SlotUse--; // Aller au slot pr�c�dent
            }
            else if (scrollValue < 0)
            {
                SlotUse++; // Aller au slot suivant
            }

            // Boucler entre les slots
            if (SlotUse < 0)
            {
                SlotUse = 2;
            }
            else if (SlotUse > 2)
            {
                SlotUse = 0;
            }

            OnChangeEquipedWeapon(); // Mettre � jour l'arme �quip�e
            canScroll = false; // Bloquer le d�filement temporairement
            StartCoroutine(scrollCountdown()); // D�marrer le cooldown
        }
    }

    // Gestion du cooldown pour �viter le spam du d�filement
    IEnumerator scrollCountdown()
    {
        yield return new WaitForSeconds(0.1f); // Attendre 0.1 seconde
        canScroll = true; // R�activer le d�filement
    }

    // Action pour ouvrir/fermer l'inventaire
    private void OnInventoryAction(InputAction.CallbackContext context)
    {
        InventoryObject.SetActive(!InventoryObject.activeSelf); // Basculer l'�tat actif de l'inventaire
        attack.canAttack = !InventoryObject.activeSelf; // D�sactiver l'attaque si l'inventaire est ouvert

        if (InventoryObject.activeSelf)
        {
            Cursor.visible = true; // Afficher le curseur
            Cursor.lockState = CursorLockMode.None; // D�bloquer le curseur
            Time.timeScale = 0; // Mettre le jeu en pause
        }
        else
        {
            Cursor.visible = false; // Masquer le curseur
            Cursor.lockState = CursorLockMode.Locked; // Verrouiller le curseur
            Time.timeScale = 1; // Reprendre le jeu
        }

        ActualiseInventory(); // Mettre � jour l'interface de l'inventaire
    }

    // Quitter l'inventaire avec le bouton d'�chappement
    private void OnInventoryQuit(InputAction.CallbackContext context)
    {
        if (!InventoryObject.activeSelf) return; // Si l'inventaire est d�j� ferm�, ne rien faire

        InventoryObject.SetActive(false); // Fermer l'inventaire
        attack.canAttack = true; // R�activer l'attaque
        Cursor.visible = false; // Masquer le curseur
        Cursor.lockState = CursorLockMode.Locked; // Verrouiller le curseur
        Time.timeScale = 1; // Reprendre le jeu
    }

    // Ouvrir la roue de s�lection des armes
    private void OpenWeaponWheel(InputAction.CallbackContext context)
    {
        SelectionWeapon.instance.Activate(); // Activer la roue
        player.LockCameraPosition = true; // Verrouiller la cam�ra
    }

    // Fermer la roue de s�lection des armes
    private void CloseWeaponWheel(InputAction.CallbackContext context)
    {
        SelectionWeapon.instance.Desactivate(); // D�sactiver la roue
        player.LockCameraPosition = false; // D�verrouiller la cam�ra
    }

    // �quiper une arme lourde
    private void GetHeavyWeapon(InputAction.CallbackContext context)
    {
        SlotUse = 0; // S�lectionner le premier slot
        OnChangeEquipedWeapon(); // Mettre � jour l'arme �quip�e
    }

    // �quiper une arme moyenne
    private void GetMediumWeapon(InputAction.CallbackContext context)
    {
        SlotUse = 1; // S�lectionner le deuxi�me slot
        OnChangeEquipedWeapon(); // Mettre � jour l'arme �quip�e
    }

    // �quiper une arme l�g�re
    private void GetLightWeapon(InputAction.CallbackContext context)
    {
        SlotUse = 2; // S�lectionner le troisi�me slot
        OnChangeEquipedWeapon(); // Mettre � jour l'arme �quip�e
    }


    // M�thode qui actualise l'inventaire en mettant � jour les slots, les quantit�s d'objets, et l'arme �quip�e
    public void ActualiseInventory()
    {
        // Efface l'objet actuellement dans le slot de l'arme lourde (heavyWeapon) en d�finissant son item sur null
        heavyWeapon.Slot.item = null;

        // Parcourt tous les slots de l'inventaire (presumably 'slots' est une collection de slots dans l'inventaire)
        foreach (var slot in slots)
        {
            // Assigne l'item actuel du slot (probablement un ItemEntry) � l'�l�ment correspondant dans le slot
            slot.Slot.item = slot.itemEntry;

            // Configure chaque slot, probablement pour l'affichage ou les interactions
            slot.Slot.SetUp();
        }

        // Met � jour le texte qui affiche la quantit� d'un item sp�cifique ('sang')
        sangNb.text = GetNbItemInInventory(sang) + ""; // On convertit la quantit� en texte

        // Restaure l'objet de l'arme lourde dans le slot correspondant
        heavyWeapon.Slot.item = heavyWeapon.itemEntry;

        // Configure � nouveau le slot de l'arme lourde
        heavyWeapon.Slot.SetUp();

        // Appelle la m�thode pour g�rer le changement d'arme �quip�e
        OnChangeEquipedWeapon();
    }


    // M�thode pour trouver un slot sp�cifique dans l'inventaire en fonction du slot cible
    public Slots FindSlotByInventorySlot(InventorySlot targetSlot)
    {
        // Parcourt tous les slots dans la collection 'slots'
        foreach (Slots slot in slots)
        {
            // Si le 'Slot' du slot actuel correspond au 'targetSlot' pass� en param�tre
            if (slot.Slot == targetSlot)
            {
                // Retourne ce slot trouv�
                return slot;
            }
        }

        // Si aucun slot ne correspond, retourne 'null'
        return null;
    }


    public void PopulateSlots()
    {
        // Vider la liste actuelle des slots
        slots.Clear();

        // R�cup�rer tous les enfants de Panel avec un composant InventorySlot
        foreach (Transform child in Panel.transform)
        {
            InventorySlot inventorySlot = child.GetComponent<InventorySlot>();
            if (inventorySlot != null)
            {
                // Cr�er une nouvelle instance de Slots et l'ajouter � la liste
                Slots slot = new Slots
                {
                    Slot = inventorySlot,
                    itemEntry = new ItemEntry() // Cr�e un nouvel ItemEntry par d�faut
                };
                slots.Add(slot);
            }
        }
    }

    // Retourne le nombre de slots libres dans l'inventaire
    public int NbFreeSlotInInventory()
    {
        int n = 0; // Initialisation du compteur de slots libres

        // Parcours de tous les slots dans l'inventaire
        foreach (var slot in slots)
        {
            // Si le type d'objet dans le slot est null, cela signifie que le slot est libre
            if (slot.itemEntry.itemType == null)
            {
                n++; // Incr�mentation du compteur pour chaque slot libre
            }
        }

        return n; // Retourne le nombre total de slots libres
    }

    // Retourne vrai si l'inventaire est plein, sinon faux
    public bool isFull()
    {
        // Si le nombre de slots libres est inf�rieur ou �gal � 0, l'inventaire est plein
        return NbFreeSlotInInventory() <= 0;
    }


    // Change la couleur des slots d'armes pendant le glissement d'objets (drag)
    public void ChangeSlotColor(bool drag, int id = 0)
    {
        if (drag) // Si un objet est en train d'�tre gliss�
        {
            // Change la couleur du slot d'arme correspondant en jaune
            switch (id)
            {
                case 0:
                    heavyWeapon.Slot.GetComponent<Image>().color = Color.yellow;
                    break;
                case 1:
                    mediumWeapon.Slot.GetComponent<Image>().color = Color.yellow;
                    break;
                case 2:
                    lightWeapon.Slot.GetComponent<Image>().color = Color.yellow;
                    break;
                default:
                    break;
            }
        }
        else // Si l'objet n'est plus en train d'�tre gliss�
        {
            // R�tablit la couleur par d�faut (blanc) pour tous les slots
            heavyWeapon.Slot.GetComponent<Image>().color = Color.white;
            mediumWeapon.Slot.GetComponent<Image>().color = Color.white;
            lightWeapon.Slot.GetComponent<Image>().color = Color.white;
        }
    }


    public void OnChangeEquipedWeapon()
    {
        // D�truire l'instance actuelle de l'arme �quip�e pour �viter les objets en double dans la sc�ne
        Destroy(weaponInstance);

        // V�rifier quel slot d'arme est actuellement utilis� et changer l'arme �quip�e en cons�quence
        switch (SlotUse)
        {
            // Si le slot 0 (arme lourde) est s�lectionn�
            case 0:
                {
                    // V�rifier si une arme lourde est pr�sente et si elle a un type d'objet valide
                    if (heavyWeapon != null && heavyWeapon.itemEntry.itemType != null)
                    {
                        // Cr�er une nouvelle instance de l'arme lourde et la placer dans les mains du joueur
                        weaponInstance = Instantiate(heavyWeapon.itemEntry.itemType.objet, PlayerHand.transform);
                    }
                    // Si aucune arme lourde n'est pr�sente, enlever l'arme �quip�e
                    if (heavyWeapon.itemEntry.itemType == null) attack.changeWeapon(null);
                    else attack.changeWeapon(heavyWeapon.itemEntry.itemType as WeaponItem);
                    break;
                }
            // Si le slot 1 (arme moyenne) est s�lectionn�
            case 1:
                {
                    // V�rifier si une arme moyenne est pr�sente et si elle a un type d'objet valide
                    if (mediumWeapon != null && mediumWeapon.itemEntry.itemType != null)
                    {
                        // Cr�er une nouvelle instance de l'arme moyenne et la placer dans les mains du joueur
                        weaponInstance = Instantiate(mediumWeapon.itemEntry.itemType.objet, PlayerHand.transform);
                    }
                    // Si aucune arme moyenne n'est pr�sente, enlever l'arme �quip�e
                    if (mediumWeapon.itemEntry.itemType == null) attack.changeWeapon(null);
                    else attack.changeWeapon(mediumWeapon.itemEntry.itemType as WeaponItem);
                    break;
                }
            // Si le slot 2 (arme l�g�re) est s�lectionn�
            case 2:
                {
                    // V�rifier si une arme l�g�re est pr�sente et si elle a un type d'objet valide
                    if (lightWeapon != null && lightWeapon.itemEntry.itemType != null)
                    {
                        // Cr�er une nouvelle instance de l'arme l�g�re et la placer dans les mains du joueur
                        weaponInstance = Instantiate(lightWeapon.itemEntry.itemType.objet, PlayerHand.transform);
                    }
                    // Si aucune arme l�g�re n'est pr�sente, enlever l'arme �quip�e
                    if (lightWeapon.itemEntry.itemType == null) attack.changeWeapon(null);
                    else attack.changeWeapon(lightWeapon.itemEntry.itemType as WeaponItem);
                    break;
                }
            // Si le slot 3 (hache) est s�lectionn�
            case 3:
                {
                    // V�rifier si une hache est pr�sente et si elle a un objet valide
                    if (axe != null)
                    {
                        // Cr�er une nouvelle instance de l'axe et la placer dans les mains du joueur
                        weaponInstance = Instantiate(axe.objet, PlayerHand.transform);
                    }
                    // Si aucune hache n'est pr�sente, enlever l'arme �quip�e
                    if (axe == null) attack.changeWeapon(null);
                    else attack.changeWeapon(axe as WeaponItem);
                    break;
                }
            // Si le slot 4 (pioche) est s�lectionn�
            case 4:
                {
                    // V�rifier si une pioche est pr�sente et si elle a un objet valide
                    if (Pickaxe != null)
                    {
                        // Cr�er une nouvelle instance de la pioche et la placer dans les mains du joueur
                        weaponInstance = Instantiate(Pickaxe.objet, PlayerHand.transform);
                    }
                    // Si aucune pioche n'est pr�sente, enlever l'arme �quip�e
                    if (Pickaxe == null) attack.changeWeapon(null);
                    else attack.changeWeapon(Pickaxe as WeaponItem);
                    break;
                }
            default: break;
        }

        // Mettre � jour les ic�nes des slots d'armes pour refl�ter les armes �quip�es
        if (heavyWeapon.Slot.item.itemType == null)
        {
            slotImages[0].sprite = slotSprites[0]; // Si aucune arme lourde, afficher l'ic�ne vide
        }
        else
        {
            slotImages[0].sprite = null; // Si une arme lourde est �quip�e, ne pas afficher l'ic�ne vide
        }

        if (mediumWeapon.Slot.item.itemType == null)
        {
            slotImages[1].sprite = slotSprites[1]; // Si aucune arme moyenne, afficher l'ic�ne vide
        }
        else
        {
            slotImages[1].sprite = null; // Si une arme moyenne est �quip�e, ne pas afficher l'ic�ne vide
        }

        if (lightWeapon.Slot.item.itemType == null)
        {
            slotImages[2].sprite = slotSprites[2]; // Si aucune arme l�g�re, afficher l'ic�ne vide
        }
        else
        {
            slotImages[2].sprite = null; // Si une arme l�g�re est �quip�e, ne pas afficher l'ic�ne vide
        }

        // Mettre � jour l'affichage des munitions
        ShowAmmoPanel();
    }

    // Actualise l'interface utilisateur des munitions en appelant la m�thode Refresh() du panneau des munitions
    public void RefreshUIAmmo()
    {
        AmmoPanel.GetComponent<AmmoPanel>().Refresh(); // Rafra�chit le panneau des munitions
    }

    // Retourne le nombre total d'un objet particulier (item) dans l'inventaire
    public int GetNbItemInInventory(Item item)
    {
        int n = 0;

        // Parcourt tous les slots dans l'inventaire
        foreach (Slots slot in slots)
        {
            // Si le type d'item dans le slot correspond � l'item recherch�, ajouter sa quantit�
            if (slot.itemEntry.itemType == item) n += slot.itemEntry.number;
        }

        return n; // Retourne le nombre total d'items trouv�s dans l'inventaire
    }

    // Affiche ou cache le panneau des munitions en fonction de l'arme �quip�e
    void ShowAmmoPanel()
    {
        switch (SlotUse) // Selon le slot actuellement utilis� (0: lourde, 1: moyenne, 2: l�g�re, etc.)
        {
            case 0:
                {
                    // V�rifie si une arme lourde est �quip�e et si c'est une arme � distance
                    if (heavyWeapon != null && heavyWeapon.itemEntry.itemType != null)
                    {
                        WeaponItem item = heavyWeapon.itemEntry.itemType as WeaponItem;
                        // Si l'arme est � distance, affiche le panneau des munitions
                        if (item.isRangeWeapon)
                        {
                            AmmoPanel.SetActive(true);
                        }
                        else
                        {
                            AmmoPanel.SetActive(false); // Si l'arme n'est pas � distance, cache le panneau des munitions
                        }
                    }
                    else
                    {
                        AmmoPanel.SetActive(false); // Si aucune arme n'est �quip�e, cache le panneau
                    }
                    break;
                }
            case 1:
                {
                    // R�p�te la m�me logique pour le slot 1 (arme moyenne)
                    if (mediumWeapon != null && mediumWeapon.itemEntry.itemType != null)
                    {
                        WeaponItem item = mediumWeapon.itemEntry.itemType as WeaponItem;
                        if (item.isRangeWeapon)
                        {
                            AmmoPanel.SetActive(true);
                        }
                        else
                        {
                            AmmoPanel.SetActive(false);
                        }
                    }
                    else
                    {
                        AmmoPanel.SetActive(false); // Cache le panneau des munitions si l'arme est absente
                    }
                    break;
                }
            case 2:
                {
                    // R�p�te la m�me logique pour le slot 2 (arme l�g�re)
                    if (lightWeapon != null && lightWeapon.itemEntry.itemType != null)
                    {
                        WeaponItem item = lightWeapon.itemEntry.itemType as WeaponItem;
                        if (item.isRangeWeapon)
                        {
                            AmmoPanel.SetActive(true); // Affiche le panneau des munitions si l'arme est � distance
                        }
                        else
                        {
                            AmmoPanel.SetActive(false); // Cache le panneau si l'arme n'est pas � distance
                        }
                    }
                    else
                    {
                        AmmoPanel.SetActive(false); // Cache le panneau si l'arme est absente
                    }
                    break;
                }
            default:
                AmmoPanel.SetActive(false); // Cache le panneau des munitions si aucun slot valide n'est s�lectionn�
                break;
        }
    }

    // Retourne les d�g�ts de l'arme actuellement �quip�e en fonction du slot utilis�
    public int GetDamage()
    {
        switch (SlotUse) // Selon le slot actuellement utilis�
        {
            case 0: // Si l'arme lourde est �quip�e
                {
                    WeaponItem item = (WeaponItem)heavyWeapon.itemEntry.itemType;
                    return item.damage; // Retourne les d�g�ts de l'arme lourde
                }
            case 1: // Si l'arme moyenne est �quip�e
                {
                    WeaponItem item = (WeaponItem)mediumWeapon.itemEntry.itemType;
                    return item.damage; // Retourne les d�g�ts de l'arme moyenne
                }
            case 2: // Si l'arme l�g�re est �quip�e
                {
                    WeaponItem item = (WeaponItem)lightWeapon.itemEntry.itemType;
                    return item.damage; // Retourne les d�g�ts de l'arme l�g�re
                }
            case 3: // Si une hache est �quip�e
                {
                    WeaponItem item = (WeaponItem)axe;
                    return item.damage; // Retourne les d�g�ts de la hache
                }
            case 4: // Si une pioche est �quip�e
                {
                    WeaponItem item = (WeaponItem)Pickaxe;
                    return item.damage; // Retourne les d�g�ts de la pioche
                }
            default:
                return 0; // Si aucun arme n'est �quip�e, retourne 0 (pas de d�g�ts)
        }
    }

    // Active ou d�sactive l'indicateur de prise (grab) selon la valeur pass�e
    public void setGrab(bool grab)
    {
        this.grab.SetActive(grab); // Active ou d�sactive l'objet grab (souvent un indicateur visuel ou un objet dans l'interface)
    }

}

[System.Serializable]
public class ItemEntry
{
    public int number = 1;
    public Item itemType;
}

[System.Serializable]
public class Slots
{
    public InventorySlot Slot;
    public ItemEntry itemEntry;
}

