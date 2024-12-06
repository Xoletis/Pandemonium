using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/UI/SelectionWeapon")]
public class SelectionWeapon : MonoBehaviour
{
    [Header("GENERAL")]
    public Camera UICamera;  // Cam�ra de l'UI, utilis�e pour manipuler la vue du menu
    public GameObject BackgroundPanel;  // Le panneau d'arri�re-plan du menu
    public GameObject CircleMenuElementPrefab;  // Le prefab d'un �l�ment du menu circulaire
    public bool UseGradient;  // D�termine si un d�grad� est utilis� pour les couleurs

    [Header("BUTTONS")]
    public Color NormalButtonColor;  // Couleur de fond par d�faut des boutons
    public Color HighlightedButtonColor;  // Couleur de fond des boutons survol�s
    public Gradient HiglightedButtonGradient = new Gradient();  // D�grad� pour l'effet de survol

    [Header("INFORMATION CENTER")]
    public Image InformalCenterBackground;  // Fond central qui affiche des informations sur l'arme s�lectionn�e
    public TextMeshProUGUI ItemName;  // Le nom de l'arme
    public Image ItemIcon;  // L'ic�ne de l'arme

    private int currentMenuItemIndex;  // L'index de l'�l�ment actuellement s�lectionn�
    private int previusMenuItemIndex;  // L'index de l'�l�ment pr�c�demment s�lectionn�
    private float calculatedMenuIndex;  // L'index calcul� bas� sur l'angle de s�lection
    private float currentSelectionAngle;  // L'angle actuel de la souris par rapport au menu
    private Vector2 currentMousePosition;  // Position actuelle de la souris

    [Header("OTHER")]
    public List<CirculatMenuElement> menuElements = new List<CirculatMenuElement>();  // Liste des �l�ments du menu circulaire

    List<GameObject> menuObjects = new List<GameObject>();  // Liste des objets du menu (instanci�s � partir du prefab)

    public static SelectionWeapon instance;  // Instance statique pour acc�der au script globalement
    public bool Active { get { return BackgroundPanel.activeSelf; } }  // V�rifie si le menu est actif

    // M�thode d'initialisation
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Il y a plusieurs instances de la roue dans la sc�ne");
        }
        instance = this;  // Assure qu'il y a une seule instance du menu
    }

    // D�marre le processus et masque le menu au d�but
    private void Start()
    {
        Initialize();
        BackgroundPanel.SetActive(false);
    }

    // M�thode d'initialisation des �l�ments du menu
    public void Initialize()
    {
        menuElements.Clear();  // Vide la liste des �l�ments du menu

        // Ajoute les diff�rentes armes disponibles dans le menu en fonction de l'inventaire du joueur
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

        // Calcule la valeur d'incr�mentation pour la disposition circulaire des �l�ments
        float rotationalIncrementalValue = 360f / menuElements.Count;
        float currentRotationValue = 0;
        float fillPercentageValue = 1f / menuElements.Count;

        // D�truit les objets de menu existants pour en cr�er de nouveaux
        foreach (GameObject go in menuObjects)
        {
            Destroy(go);
        }
        menuObjects.Clear();

        // Instancie chaque �l�ment du menu circulaire
        for (int i = 0; i < menuElements.Count; i++)
        {
            GameObject menuElementGameObject = Instantiate(CircleMenuElementPrefab);  // Cr�e un nouvel �l�ment du menu
            menuElementGameObject.name = i + " : " + currentRotationValue;
            menuElementGameObject.transform.SetParent(BackgroundPanel.transform);  // Ajoute l'�l�ment au panneau

            MenuButton menuButton = menuElementGameObject.GetComponent<MenuButton>();  // R�cup�re le script MenuButton

            menuObjects.Add(menuElementGameObject);

            // Ajuste la position et la rotation de chaque �l�ment pour le disposer circulairement
            menuButton.Recttransform.localScale = Vector3.one;
            menuButton.Recttransform.localPosition = Vector3.zero;
            menuButton.Recttransform.rotation = Quaternion.Euler(0f, 0f, currentRotationValue);
            currentRotationValue += rotationalIncrementalValue;

            // Remplissage de l'ic�ne et du fond de chaque �l�ment
            menuButton.BackgroundImage.fillAmount = fillPercentageValue - 0.01f;
            menuElements[i].ButtonBackground = menuButton.BackgroundImage;
            menuElements[i].ButtonBackground.color = NormalButtonColor;

            menuButton.IconImage.sprite = menuElements[i].item.image;
            menuButton.IconRecttransform.rotation = Quaternion.identity;
        }
    }

    // Met � jour l'�l�ment actuellement s�lectionn� en fonction de la position de la souris
    private void Update()
    {
        if (!Active) return;
        GetCurrentMenuEllement();  // Appelle la m�thode pour d�terminer l'�l�ment s�lectionn�
    }

    // D�tecte l'�l�ment du menu s�lectionn� en fonction de la position de la souris
    private void GetCurrentMenuEllement()
    {
        float rotationalIncrementalValue = 360f / menuElements.Count;
        currentMousePosition = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);  // R�cup�re la position de la souris

        currentSelectionAngle = 90 + rotationalIncrementalValue + Mathf.Atan2(currentMousePosition.y, currentMousePosition.x) * Mathf.Rad2Deg;  // Calcule l'angle de s�lection
        currentSelectionAngle = (currentSelectionAngle + 360f) % 360f;  // Normalise l'angle pour le ramener entre 0 et 360�

        currentMenuItemIndex = (int)(currentSelectionAngle / rotationalIncrementalValue);  // Calcule l'index de l'�l�ment s�lectionn�

        if (currentMenuItemIndex != previusMenuItemIndex)
        {
            // Change la couleur de l'�l�ment pr�c�demment s�lectionn�
            menuElements[previusMenuItemIndex].ButtonBackground.color = NormalButtonColor;

            previusMenuItemIndex = currentMenuItemIndex;  // Met � jour l'index pr�c�dent

            // Change la couleur de l'�l�ment actuellement s�lectionn�
            menuElements[currentMenuItemIndex].ButtonBackground.color = UseGradient ? HiglightedButtonGradient.Evaluate(1f / menuElements.Count * currentMenuItemIndex) : HighlightedButtonColor;
            InformalCenterBackground.color = UseGradient ? HiglightedButtonGradient.Evaluate(1f / menuElements.Count * currentMenuItemIndex) : HighlightedButtonColor;

            // Affiche les informations sur l'arme s�lectionn�e
            ItemName.text = menuElements[currentMenuItemIndex].item.Nom;
            ItemIcon.sprite = menuElements[currentMenuItemIndex].item.image;
        }
    }

    // S�lectionne l'arme et met � jour l'arme �quip�e du joueur
    private void Select()
    {
        WeaponItem weapon = (WeaponItem)menuElements[currentMenuItemIndex].item;  // R�cup�re l'arme s�lectionn�e
        Debug.Log(weapon.type);  // Affiche le type de l'arme s�lectionn�e

        // Met � jour l'index de l'arme �quip�e en fonction du type d'arme
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
        if (Active) return;  // Si le menu est d�j� actif, ne rien faire
        Cursor.visible = false;  // Masque le curseur
        Cursor.lockState = CursorLockMode.None;  // D�verrouille le curseur
        Initialize();  // R�initialise le menu
        BackgroundPanel.SetActive(true);  // Affiche le panneau de menu
    }

    // D�sactive le menu circulaire et s�lectionne l'arme
    public void Desactivate()
    {
        Select();  // S�lectionne l'arme
        BackgroundPanel.SetActive(false);  // Cache le panneau de menu
    }
}
