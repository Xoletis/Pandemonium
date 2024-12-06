using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/Player/Inventory/InventoryManager")]
public class InventoryManager : MonoBehaviour
{
    // Références aux GameObjects pour les éléments UI de l'inventaire
    public GameObject InventoryObject; // Fenêtre principale de l'inventaire
    public GameObject ItemCase; // Conteneur d'objets
    public GameObject Panel; // Panel général
    public GameObject PlayerHand; // Position de la main du joueur pour tenir un objet
    public GameObject ItemDescription; // Description d'un objet sélectionné
    public GameObject AmmoPanel; // Panneau pour afficher les munitions
    public GameObject ItemFieldHistory; // Conteneur pour les éléments de l'historique des objets
    public GameObject ItemFieldPrefab; // Préfabriqué pour un champ d'objet
    public ThirdPersonController player; // Contrôleur du joueur

    // Sprites et images pour les slots de l'inventaire
    public Sprite[] slotSprites; // Sprites des slots
    public Image[] slotImages; // Images des slots correspondants

    // Référence à l'arme actuellement équipée
    [HideInInspector]
    public GameObject weaponInstance;

    // Système d'entrées pour gérer l'inventaire et les armes
    private InputForPlayer _playerInput;

    // Objets spécifiques (ex. pioche, hache)
    public Item Pickaxe, axe;

    // Différents slots d'armes (lourdes, moyennes, légères)
    public Slots heavyWeapon;
    public Slots mediumWeapon;
    public Slots lightWeapon;

    // Slot actuellement sélectionné
    [HideInInspector]
    public int SlotUse;

    // Permet de contrôler si le joueur peut défiler entre les slots
    bool canScroll = true;

    // Texte pour afficher le nombre d'objets (ex. nombre de "sang")
    public TextMeshProUGUI sangNb;
    public Item sang; // Référence à l'objet "sang"

    // Liste des slots d'inventaire
    public List<Slots> slots;

    // Instance singleton de l'InventoryManager
    public static InventoryManager instance;

    // Objet utilisé pour attraper des items
    public GameObject grab;

    // Référence à l'attaque du joueur
    [HideInInspector] public PlayerAttack attack;

    private void Awake()
    {
        // Initialisation du système d'entrées
        _playerInput = new InputForPlayer();
        _playerInput.Player.Inventory.performed += OnInventoryAction; // Ouvrir/fermer l'inventaire
        _playerInput.Player.SwitchWeapon.performed += SwitchWeapon; // Changer d'arme
        _playerInput.Player.HeavyWeapon.performed += GetHeavyWeapon; // Sélectionner une arme lourde
        _playerInput.Player.MediumWeapon.performed += GetMediumWeapon; // Sélectionner une arme moyenne
        _playerInput.Player.LightWeapon.performed += GetLightWeapon; // Sélectionner une arme légère
        _playerInput.Player.OpenWheel.started += OpenWeaponWheel; // Ouvrir la roue d'armes
        _playerInput.Player.OpenWheel.canceled += CloseWeaponWheel; // Fermer la roue d'armes
        _playerInput.Player.Escape.performed += OnInventoryQuit; // Quitter l'inventaire

        // Vérifier qu'une seule instance de l'InventoryManager est présente
        if (instance != null)
        {
            Debug.LogError("Il y a plusieurs instances d'InventoryManager dans la scène");
        }
        instance = this;

        // Désactiver l'inventaire au démarrage
        InventoryObject.SetActive(false);
    }

    private void Start()
    {
        // Initialiser l'état de l'UI et du jeu
        ItemDescription.SetActive(false); // Masquer la description
        SlotUse = 0; // Slot sélectionné par défaut
        AmmoPanel.SetActive(false); // Masquer le panneau de munitions
        Cursor.visible = false; // Masquer le curseur
        Cursor.lockState = CursorLockMode.Locked; // Verrouiller le curseur
        Time.timeScale = 1; // Temps normal
        grab.SetActive(false); // Désactiver l'objet de saisie
    }

    private void OnEnable()
    {
        // Activer le système d'entrées
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        // Désactiver le système d'entrées
        _playerInput.Disable();
    }

    // Ajouter un objet à l'inventaire
    public void AddItem(ItemEntry item, PickableObject Object = null)
    {
        // Si l'objet est une arme, l'ajouter directement dans un nouveau slot
        if (item.itemType is WeaponItem)
        {
            AddItemInNewSlot(item);
            if (Object != null)
            {
                Destroy(Object.gameObject); // Détruire l'objet récupéré
            }
            return;
        }

        // Créer une entrée dans l'interface pour l'objet
        GameObject field = Instantiate(ItemFieldPrefab, ItemFieldHistory.transform);
        field.GetComponent<ItemField>().Create(item.itemType, item.number);

        // Essayer d'empiler l'objet dans les slots existants
        for (int i = 0; i < slots.Count; i++)
        {
            if (item.number <= 0) break; // Si aucun objet restant, arrêter

            // Empiler dans un slot existant de même type
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

        // Si l'objet ne peut pas être empilé, créer un nouveau slot
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

    // Méthode pour ajouter un objet à l'inventaire avec une quantité spécifiée
    public void AddItem(Item item, int Count)
    {
        // Crée un nouvel objet de type ItemEntry pour l'élément à ajouter
        ItemEntry newItemEntry = new ItemEntry
        {
            // Affecte l'élément à la propriété itemType de l'entrée
            itemType = item,
            // Affecte la quantité de l'objet à la propriété number de l'entrée
            number = Count
        };

        // Appelle la méthode AddItem en passant l'ItemEntry nouvellement créé
        AddItem(newItemEntry);
    }


    // Ajouter un objet dans un nouveau slot d'inventaire
    public bool AddItemInNewSlot(ItemEntry item)
    {
        foreach (var _slot in slots)
        {
            if (item.number <= 0) break; // Si tous les objets ont été ajoutés, arrêter

            // Ajouter dans un slot vide
            if (_slot.itemEntry.itemType == null)
            {
                int quantityToAdd = Mathf.Min(item.number, item.itemType.maxStack); // Quantité pouvant être ajoutée
                ItemEntry newItemEntry = new ItemEntry
                {
                    itemType = item.itemType, // Type d'objet
                    number = quantityToAdd // Quantité d'objets ajoutés
                };
                _slot.itemEntry = newItemEntry; // Mettre à jour le slot
                item.number -= quantityToAdd; // Réduire le nombre restant
            }
        }

        ActualiseInventory(); // Mettre à jour l'UI
        return item.number <= 0; // Retourner true si tout a été ajouté
    }

    // Ajouter un objet dans un slot existant
    public void AddItemInSlot(ItemEntry item, int slotID)
    {
        Slots actualSlot = slots[slotID]; // Récupérer le slot
        if (actualSlot.itemEntry == null)
        {
            // Si le slot est vide, assigner directement
            actualSlot.itemEntry = item;
        }
        else if (actualSlot.itemEntry.itemType == item.itemType)
        {
            // Si le type correspond, mettre à jour
            actualSlot.itemEntry = item;
        }
    }

    // Retirer un certain nombre d'objets d'un type spécifique
    public int RemoveItem(Item item, int nb)
    {
        if (GetNbItemInInventory(item) <= 0)
        {
            return nb; // Si l'objet n'existe pas, retourner le nombre à retirer
        }

        foreach (var _slot in slots)
        {
            if (_slot.itemEntry.itemType == item)
            {
                if (_slot.itemEntry.number > nb)
                {
                    _slot.itemEntry.number -= nb; // Réduire directement le nombre
                    return 0; // Tous les objets ont été retirés
                }
                else if (_slot.itemEntry.number == nb)
                {
                    // Si le slot contient exactement le nombre à retirer
                    _slot.itemEntry.number = 1; // Remettre à 1 (ou 0 selon logique)
                    _slot.itemEntry.itemType = null; // Vider le slot
                    return 0;
                }
                else
                {
                    // Si le slot contient moins que le nombre à retirer
                    nb -= _slot.itemEntry.number; // Réduire le reste
                    _slot.itemEntry.number = 1; // Réinitialiser le slot
                    _slot.itemEntry.itemType = null; // Vider le slot
                }
            }
        }
        return nb; // Retourner le nombre restant à retirer
    }


    // Changer d'arme avec l'input de défilement
    private void SwitchWeapon(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<float>(); // Valeur de défilement
        if (canScroll)
        {
            if (scrollValue > 0)
            {
                SlotUse--; // Aller au slot précédent
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

            OnChangeEquipedWeapon(); // Mettre à jour l'arme équipée
            canScroll = false; // Bloquer le défilement temporairement
            StartCoroutine(scrollCountdown()); // Démarrer le cooldown
        }
    }

    // Gestion du cooldown pour éviter le spam du défilement
    IEnumerator scrollCountdown()
    {
        yield return new WaitForSeconds(0.1f); // Attendre 0.1 seconde
        canScroll = true; // Réactiver le défilement
    }

    // Action pour ouvrir/fermer l'inventaire
    private void OnInventoryAction(InputAction.CallbackContext context)
    {
        InventoryObject.SetActive(!InventoryObject.activeSelf); // Basculer l'état actif de l'inventaire
        attack.canAttack = !InventoryObject.activeSelf; // Désactiver l'attaque si l'inventaire est ouvert

        if (InventoryObject.activeSelf)
        {
            Cursor.visible = true; // Afficher le curseur
            Cursor.lockState = CursorLockMode.None; // Débloquer le curseur
            Time.timeScale = 0; // Mettre le jeu en pause
        }
        else
        {
            Cursor.visible = false; // Masquer le curseur
            Cursor.lockState = CursorLockMode.Locked; // Verrouiller le curseur
            Time.timeScale = 1; // Reprendre le jeu
        }

        ActualiseInventory(); // Mettre à jour l'interface de l'inventaire
    }

    // Quitter l'inventaire avec le bouton d'échappement
    private void OnInventoryQuit(InputAction.CallbackContext context)
    {
        if (!InventoryObject.activeSelf) return; // Si l'inventaire est déjà fermé, ne rien faire

        InventoryObject.SetActive(false); // Fermer l'inventaire
        attack.canAttack = true; // Réactiver l'attaque
        Cursor.visible = false; // Masquer le curseur
        Cursor.lockState = CursorLockMode.Locked; // Verrouiller le curseur
        Time.timeScale = 1; // Reprendre le jeu
    }

    // Ouvrir la roue de sélection des armes
    private void OpenWeaponWheel(InputAction.CallbackContext context)
    {
        SelectionWeapon.instance.Activate(); // Activer la roue
        player.LockCameraPosition = true; // Verrouiller la caméra
    }

    // Fermer la roue de sélection des armes
    private void CloseWeaponWheel(InputAction.CallbackContext context)
    {
        SelectionWeapon.instance.Desactivate(); // Désactiver la roue
        player.LockCameraPosition = false; // Déverrouiller la caméra
    }

    // Équiper une arme lourde
    private void GetHeavyWeapon(InputAction.CallbackContext context)
    {
        SlotUse = 0; // Sélectionner le premier slot
        OnChangeEquipedWeapon(); // Mettre à jour l'arme équipée
    }

    // Équiper une arme moyenne
    private void GetMediumWeapon(InputAction.CallbackContext context)
    {
        SlotUse = 1; // Sélectionner le deuxième slot
        OnChangeEquipedWeapon(); // Mettre à jour l'arme équipée
    }

    // Équiper une arme légère
    private void GetLightWeapon(InputAction.CallbackContext context)
    {
        SlotUse = 2; // Sélectionner le troisième slot
        OnChangeEquipedWeapon(); // Mettre à jour l'arme équipée
    }


    // Méthode qui actualise l'inventaire en mettant à jour les slots, les quantités d'objets, et l'arme équipée
    public void ActualiseInventory()
    {
        // Efface l'objet actuellement dans le slot de l'arme lourde (heavyWeapon) en définissant son item sur null
        heavyWeapon.Slot.item = null;

        // Parcourt tous les slots de l'inventaire (presumably 'slots' est une collection de slots dans l'inventaire)
        foreach (var slot in slots)
        {
            // Assigne l'item actuel du slot (probablement un ItemEntry) à l'élément correspondant dans le slot
            slot.Slot.item = slot.itemEntry;

            // Configure chaque slot, probablement pour l'affichage ou les interactions
            slot.Slot.SetUp();
        }

        // Met à jour le texte qui affiche la quantité d'un item spécifique ('sang')
        sangNb.text = GetNbItemInInventory(sang) + ""; // On convertit la quantité en texte

        // Restaure l'objet de l'arme lourde dans le slot correspondant
        heavyWeapon.Slot.item = heavyWeapon.itemEntry;

        // Configure à nouveau le slot de l'arme lourde
        heavyWeapon.Slot.SetUp();

        // Appelle la méthode pour gérer le changement d'arme équipée
        OnChangeEquipedWeapon();
    }


    // Méthode pour trouver un slot spécifique dans l'inventaire en fonction du slot cible
    public Slots FindSlotByInventorySlot(InventorySlot targetSlot)
    {
        // Parcourt tous les slots dans la collection 'slots'
        foreach (Slots slot in slots)
        {
            // Si le 'Slot' du slot actuel correspond au 'targetSlot' passé en paramètre
            if (slot.Slot == targetSlot)
            {
                // Retourne ce slot trouvé
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

        // Récupérer tous les enfants de Panel avec un composant InventorySlot
        foreach (Transform child in Panel.transform)
        {
            InventorySlot inventorySlot = child.GetComponent<InventorySlot>();
            if (inventorySlot != null)
            {
                // Créer une nouvelle instance de Slots et l'ajouter à la liste
                Slots slot = new Slots
                {
                    Slot = inventorySlot,
                    itemEntry = new ItemEntry() // Crée un nouvel ItemEntry par défaut
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
                n++; // Incrémentation du compteur pour chaque slot libre
            }
        }

        return n; // Retourne le nombre total de slots libres
    }

    // Retourne vrai si l'inventaire est plein, sinon faux
    public bool isFull()
    {
        // Si le nombre de slots libres est inférieur ou égal à 0, l'inventaire est plein
        return NbFreeSlotInInventory() <= 0;
    }


    // Change la couleur des slots d'armes pendant le glissement d'objets (drag)
    public void ChangeSlotColor(bool drag, int id = 0)
    {
        if (drag) // Si un objet est en train d'être glissé
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
        else // Si l'objet n'est plus en train d'être glissé
        {
            // Rétablit la couleur par défaut (blanc) pour tous les slots
            heavyWeapon.Slot.GetComponent<Image>().color = Color.white;
            mediumWeapon.Slot.GetComponent<Image>().color = Color.white;
            lightWeapon.Slot.GetComponent<Image>().color = Color.white;
        }
    }


    public void OnChangeEquipedWeapon()
    {
        // Détruire l'instance actuelle de l'arme équipée pour éviter les objets en double dans la scène
        Destroy(weaponInstance);

        // Vérifier quel slot d'arme est actuellement utilisé et changer l'arme équipée en conséquence
        switch (SlotUse)
        {
            // Si le slot 0 (arme lourde) est sélectionné
            case 0:
                {
                    // Vérifier si une arme lourde est présente et si elle a un type d'objet valide
                    if (heavyWeapon != null && heavyWeapon.itemEntry.itemType != null)
                    {
                        // Créer une nouvelle instance de l'arme lourde et la placer dans les mains du joueur
                        weaponInstance = Instantiate(heavyWeapon.itemEntry.itemType.objet, PlayerHand.transform);
                    }
                    // Si aucune arme lourde n'est présente, enlever l'arme équipée
                    if (heavyWeapon.itemEntry.itemType == null) attack.changeWeapon(null);
                    else attack.changeWeapon(heavyWeapon.itemEntry.itemType as WeaponItem);
                    break;
                }
            // Si le slot 1 (arme moyenne) est sélectionné
            case 1:
                {
                    // Vérifier si une arme moyenne est présente et si elle a un type d'objet valide
                    if (mediumWeapon != null && mediumWeapon.itemEntry.itemType != null)
                    {
                        // Créer une nouvelle instance de l'arme moyenne et la placer dans les mains du joueur
                        weaponInstance = Instantiate(mediumWeapon.itemEntry.itemType.objet, PlayerHand.transform);
                    }
                    // Si aucune arme moyenne n'est présente, enlever l'arme équipée
                    if (mediumWeapon.itemEntry.itemType == null) attack.changeWeapon(null);
                    else attack.changeWeapon(mediumWeapon.itemEntry.itemType as WeaponItem);
                    break;
                }
            // Si le slot 2 (arme légère) est sélectionné
            case 2:
                {
                    // Vérifier si une arme légère est présente et si elle a un type d'objet valide
                    if (lightWeapon != null && lightWeapon.itemEntry.itemType != null)
                    {
                        // Créer une nouvelle instance de l'arme légère et la placer dans les mains du joueur
                        weaponInstance = Instantiate(lightWeapon.itemEntry.itemType.objet, PlayerHand.transform);
                    }
                    // Si aucune arme légère n'est présente, enlever l'arme équipée
                    if (lightWeapon.itemEntry.itemType == null) attack.changeWeapon(null);
                    else attack.changeWeapon(lightWeapon.itemEntry.itemType as WeaponItem);
                    break;
                }
            // Si le slot 3 (hache) est sélectionné
            case 3:
                {
                    // Vérifier si une hache est présente et si elle a un objet valide
                    if (axe != null)
                    {
                        // Créer une nouvelle instance de l'axe et la placer dans les mains du joueur
                        weaponInstance = Instantiate(axe.objet, PlayerHand.transform);
                    }
                    // Si aucune hache n'est présente, enlever l'arme équipée
                    if (axe == null) attack.changeWeapon(null);
                    else attack.changeWeapon(axe as WeaponItem);
                    break;
                }
            // Si le slot 4 (pioche) est sélectionné
            case 4:
                {
                    // Vérifier si une pioche est présente et si elle a un objet valide
                    if (Pickaxe != null)
                    {
                        // Créer une nouvelle instance de la pioche et la placer dans les mains du joueur
                        weaponInstance = Instantiate(Pickaxe.objet, PlayerHand.transform);
                    }
                    // Si aucune pioche n'est présente, enlever l'arme équipée
                    if (Pickaxe == null) attack.changeWeapon(null);
                    else attack.changeWeapon(Pickaxe as WeaponItem);
                    break;
                }
            default: break;
        }

        // Mettre à jour les icônes des slots d'armes pour refléter les armes équipées
        if (heavyWeapon.Slot.item.itemType == null)
        {
            slotImages[0].sprite = slotSprites[0]; // Si aucune arme lourde, afficher l'icône vide
        }
        else
        {
            slotImages[0].sprite = null; // Si une arme lourde est équipée, ne pas afficher l'icône vide
        }

        if (mediumWeapon.Slot.item.itemType == null)
        {
            slotImages[1].sprite = slotSprites[1]; // Si aucune arme moyenne, afficher l'icône vide
        }
        else
        {
            slotImages[1].sprite = null; // Si une arme moyenne est équipée, ne pas afficher l'icône vide
        }

        if (lightWeapon.Slot.item.itemType == null)
        {
            slotImages[2].sprite = slotSprites[2]; // Si aucune arme légère, afficher l'icône vide
        }
        else
        {
            slotImages[2].sprite = null; // Si une arme légère est équipée, ne pas afficher l'icône vide
        }

        // Mettre à jour l'affichage des munitions
        ShowAmmoPanel();
    }

    // Actualise l'interface utilisateur des munitions en appelant la méthode Refresh() du panneau des munitions
    public void RefreshUIAmmo()
    {
        AmmoPanel.GetComponent<AmmoPanel>().Refresh(); // Rafraîchit le panneau des munitions
    }

    // Retourne le nombre total d'un objet particulier (item) dans l'inventaire
    public int GetNbItemInInventory(Item item)
    {
        int n = 0;

        // Parcourt tous les slots dans l'inventaire
        foreach (Slots slot in slots)
        {
            // Si le type d'item dans le slot correspond à l'item recherché, ajouter sa quantité
            if (slot.itemEntry.itemType == item) n += slot.itemEntry.number;
        }

        return n; // Retourne le nombre total d'items trouvés dans l'inventaire
    }

    // Affiche ou cache le panneau des munitions en fonction de l'arme équipée
    void ShowAmmoPanel()
    {
        switch (SlotUse) // Selon le slot actuellement utilisé (0: lourde, 1: moyenne, 2: légère, etc.)
        {
            case 0:
                {
                    // Vérifie si une arme lourde est équipée et si c'est une arme à distance
                    if (heavyWeapon != null && heavyWeapon.itemEntry.itemType != null)
                    {
                        WeaponItem item = heavyWeapon.itemEntry.itemType as WeaponItem;
                        // Si l'arme est à distance, affiche le panneau des munitions
                        if (item.isRangeWeapon)
                        {
                            AmmoPanel.SetActive(true);
                        }
                        else
                        {
                            AmmoPanel.SetActive(false); // Si l'arme n'est pas à distance, cache le panneau des munitions
                        }
                    }
                    else
                    {
                        AmmoPanel.SetActive(false); // Si aucune arme n'est équipée, cache le panneau
                    }
                    break;
                }
            case 1:
                {
                    // Répète la même logique pour le slot 1 (arme moyenne)
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
                    // Répète la même logique pour le slot 2 (arme légère)
                    if (lightWeapon != null && lightWeapon.itemEntry.itemType != null)
                    {
                        WeaponItem item = lightWeapon.itemEntry.itemType as WeaponItem;
                        if (item.isRangeWeapon)
                        {
                            AmmoPanel.SetActive(true); // Affiche le panneau des munitions si l'arme est à distance
                        }
                        else
                        {
                            AmmoPanel.SetActive(false); // Cache le panneau si l'arme n'est pas à distance
                        }
                    }
                    else
                    {
                        AmmoPanel.SetActive(false); // Cache le panneau si l'arme est absente
                    }
                    break;
                }
            default:
                AmmoPanel.SetActive(false); // Cache le panneau des munitions si aucun slot valide n'est sélectionné
                break;
        }
    }

    // Retourne les dégâts de l'arme actuellement équipée en fonction du slot utilisé
    public int GetDamage()
    {
        switch (SlotUse) // Selon le slot actuellement utilisé
        {
            case 0: // Si l'arme lourde est équipée
                {
                    WeaponItem item = (WeaponItem)heavyWeapon.itemEntry.itemType;
                    return item.damage; // Retourne les dégâts de l'arme lourde
                }
            case 1: // Si l'arme moyenne est équipée
                {
                    WeaponItem item = (WeaponItem)mediumWeapon.itemEntry.itemType;
                    return item.damage; // Retourne les dégâts de l'arme moyenne
                }
            case 2: // Si l'arme légère est équipée
                {
                    WeaponItem item = (WeaponItem)lightWeapon.itemEntry.itemType;
                    return item.damage; // Retourne les dégâts de l'arme légère
                }
            case 3: // Si une hache est équipée
                {
                    WeaponItem item = (WeaponItem)axe;
                    return item.damage; // Retourne les dégâts de la hache
                }
            case 4: // Si une pioche est équipée
                {
                    WeaponItem item = (WeaponItem)Pickaxe;
                    return item.damage; // Retourne les dégâts de la pioche
                }
            default:
                return 0; // Si aucun arme n'est équipée, retourne 0 (pas de dégâts)
        }
    }

    // Active ou désactive l'indicateur de prise (grab) selon la valeur passée
    public void setGrab(bool grab)
    {
        this.grab.SetActive(grab); // Active ou désactive l'objet grab (souvent un indicateur visuel ou un objet dans l'interface)
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

