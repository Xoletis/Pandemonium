using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/UI/SelectionWeapon")]
public class SelectionWeapon : MonoBehaviour
{
    [Header("GENERAL")]
    public Camera UICamera;  // Caméra de l'UI, utilisée pour manipuler la vue du menu
    public GameObject BackgroundPanel;  // Le panneau d'arrière-plan du menu
    public GameObject CircleMenuElementPrefab;  // Le prefab d'un élément du menu circulaire
    public bool UseGradient;  // Détermine si un dégradé est utilisé pour les couleurs

    [Header("BUTTONS")]
    public Color NormalButtonColor;  // Couleur de fond par défaut des boutons
    public Color HighlightedButtonColor;  // Couleur de fond des boutons survolés
    public Gradient HiglightedButtonGradient = new Gradient();  // Dégradé pour l'effet de survol

    [Header("INFORMATION CENTER")]
    public Image InformalCenterBackground;  // Fond central qui affiche des informations sur l'arme sélectionnée
    public TextMeshProUGUI ItemName;  // Le nom de l'arme
    public Image ItemIcon;  // L'icône de l'arme

    private int currentMenuItemIndex;  // L'index de l'élément actuellement sélectionné
    private int previusMenuItemIndex;  // L'index de l'élément précédemment sélectionné
    private float calculatedMenuIndex;  // L'index calculé basé sur l'angle de sélection
    private float currentSelectionAngle;  // L'angle actuel de la souris par rapport au menu
    private Vector2 currentMousePosition;  // Position actuelle de la souris

    [Header("OTHER")]
    public List<CirculatMenuElement> menuElements = new List<CirculatMenuElement>();  // Liste des éléments du menu circulaire

    List<GameObject> menuObjects = new List<GameObject>();  // Liste des objets du menu (instanciés à partir du prefab)

    public static SelectionWeapon instance;  // Instance statique pour accéder au script globalement
    public bool Active { get { return BackgroundPanel.activeSelf; } }  // Vérifie si le menu est actif

    // Méthode d'initialisation
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Il y a plusieurs instances de la roue dans la scène");
        }
        instance = this;  // Assure qu'il y a une seule instance du menu
    }

    // Démarre le processus et masque le menu au début
    private void Start()
    {
        Initialize();
        BackgroundPanel.SetActive(false);
    }

    // Méthode d'initialisation des éléments du menu
    public void Initialize()
    {
        menuElements.Clear();  // Vide la liste des éléments du menu

        // Ajoute les différentes armes disponibles dans le menu en fonction de l'inventaire du joueur
        if (InventoryManager.instance.heavyWeapon.itemEntry.itemType != null)
        {
            menuElements.Add(new CirculatMenuElement(InventoryManager.instance.heavyWeapon.itemEntry.itemType));
        }

        if (InventoryManager.instance.mediumWeapon.itemEntry.itemType != null)
        {
            menuElements.Add(new CirculatMenuElement(InventoryManager.instance.mediumWeapon.itemEntry.itemType));
        }

        if (InventoryManager.instance.lightWeapon.itemEntry.itemType != null)
        {
            menuElements.Add(new CirculatMenuElement(InventoryManager.instance.lightWeapon.itemEntry.itemType));
        }

        if (InventoryManager.instance.axe != null)
        {
            menuElements.Add(new CirculatMenuElement(InventoryManager.instance.axe));
        }

        if (InventoryManager.instance.Pickaxe != null)
        {
            menuElements.Add(new CirculatMenuElement(InventoryManager.instance.Pickaxe));
        }

        // Calcule la valeur d'incrémentation pour la disposition circulaire des éléments
        float rotationalIncrementalValue = 360f / menuElements.Count;
        float currentRotationValue = 0;
        float fillPercentageValue = 1f / menuElements.Count;

        // Détruit les objets de menu existants pour en créer de nouveaux
        foreach (GameObject go in menuObjects)
        {
            Destroy(go);
        }
        menuObjects.Clear();

        // Instancie chaque élément du menu circulaire
        for (int i = 0; i < menuElements.Count; i++)
        {
            GameObject menuElementGameObject = Instantiate(CircleMenuElementPrefab);  // Crée un nouvel élément du menu
            menuElementGameObject.name = i + " : " + currentRotationValue;
            menuElementGameObject.transform.SetParent(BackgroundPanel.transform);  // Ajoute l'élément au panneau

            MenuButton menuButton = menuElementGameObject.GetComponent<MenuButton>();  // Récupère le script MenuButton

            menuObjects.Add(menuElementGameObject);

            // Ajuste la position et la rotation de chaque élément pour le disposer circulairement
            menuButton.Recttransform.localScale = Vector3.one;
            menuButton.Recttransform.localPosition = Vector3.zero;
            menuButton.Recttransform.rotation = Quaternion.Euler(0f, 0f, currentRotationValue);
            currentRotationValue += rotationalIncrementalValue;

            // Remplissage de l'icône et du fond de chaque élément
            menuButton.BackgroundImage.fillAmount = fillPercentageValue - 0.01f;
            menuElements[i].ButtonBackground = menuButton.BackgroundImage;
            menuElements[i].ButtonBackground.color = NormalButtonColor;

            menuButton.IconImage.sprite = menuElements[i].item.image;
            menuButton.IconRecttransform.rotation = Quaternion.identity;
        }
    }

    // Met à jour l'élément actuellement sélectionné en fonction de la position de la souris
    private void Update()
    {
        if (!Active) return;
        GetCurrentMenuEllement();  // Appelle la méthode pour déterminer l'élément sélectionné
    }

    // Détecte l'élément du menu sélectionné en fonction de la position de la souris
    private void GetCurrentMenuEllement()
    {
        float rotationalIncrementalValue = 360f / menuElements.Count;
        currentMousePosition = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);  // Récupère la position de la souris

        currentSelectionAngle = 90 + rotationalIncrementalValue + Mathf.Atan2(currentMousePosition.y, currentMousePosition.x) * Mathf.Rad2Deg;  // Calcule l'angle de sélection
        currentSelectionAngle = (currentSelectionAngle + 360f) % 360f;  // Normalise l'angle pour le ramener entre 0 et 360°

        currentMenuItemIndex = (int)(currentSelectionAngle / rotationalIncrementalValue);  // Calcule l'index de l'élément sélectionné

        if (currentMenuItemIndex != previusMenuItemIndex)
        {
            // Change la couleur de l'élément précédemment sélectionné
            menuElements[previusMenuItemIndex].ButtonBackground.color = NormalButtonColor;

            previusMenuItemIndex = currentMenuItemIndex;  // Met à jour l'index précédent

            // Change la couleur de l'élément actuellement sélectionné
            menuElements[currentMenuItemIndex].ButtonBackground.color = UseGradient ? HiglightedButtonGradient.Evaluate(1f / menuElements.Count * currentMenuItemIndex) : HighlightedButtonColor;
            InformalCenterBackground.color = UseGradient ? HiglightedButtonGradient.Evaluate(1f / menuElements.Count * currentMenuItemIndex) : HighlightedButtonColor;

            // Affiche les informations sur l'arme sélectionnée
            ItemName.text = menuElements[currentMenuItemIndex].item.Nom;
            ItemIcon.sprite = menuElements[currentMenuItemIndex].item.image;
        }
    }

    // Sélectionne l'arme et met à jour l'arme équipée du joueur
    private void Select()
    {
        WeaponItem weapon = (WeaponItem)menuElements[currentMenuItemIndex].item;  // Récupère l'arme sélectionnée
        Debug.Log(weapon.type);  // Affiche le type de l'arme sélectionnée

        // Met à jour l'index de l'arme équipée en fonction du type d'arme
        switch (weapon.type)
        {
            case WeaponType.Lourde: InventoryManager.instance.SlotUse = 0; break;
            case WeaponType.Moyenne: InventoryManager.instance.SlotUse = 1; break;
            case WeaponType.Legere: InventoryManager.instance.SlotUse = 2; break;
            case WeaponType.Axe: InventoryManager.instance.SlotUse = 3; break;
            case WeaponType.Pickaxe: InventoryManager.instance.SlotUse = 4; break;
        }
        InventoryManager.instance.OnChangeEquipedWeapon();  // Applique le changement d'arme
    }

    // Active le menu circulaire
    public void Activate()
    {
        if (Active) return;  // Si le menu est déjà actif, ne rien faire
        Cursor.visible = false;  // Masque le curseur
        Cursor.lockState = CursorLockMode.None;  // Déverrouille le curseur
        Initialize();  // Réinitialise le menu
        BackgroundPanel.SetActive(true);  // Affiche le panneau de menu
    }

    // Désactive le menu circulaire et sélectionne l'arme
    public void Desactivate()
    {
        Select();  // Sélectionne l'arme
        BackgroundPanel.SetActive(false);  // Cache le panneau de menu
    }
}
