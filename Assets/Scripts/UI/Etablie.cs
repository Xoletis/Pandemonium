using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/UI/Etablie")]
public class Etablie : MonoBehaviour
{
    // R�f�rences aux objets de l'UI
    public GameObject EtablieUI; // Le panneau d'artisanat (interface)
    public GameObject ItemContent; // Conteneur pour les objets � fabriquer
    public GameObject ItemPrefab; // Le prefab pour chaque objet � fabriquer
    public GameObject RessourcesPrefab; // Le prefab pour les ressources n�cessaires � la fabrication
    public GameObject RessourcesContent; // Conteneur pour afficher les ressources
    public GameObject StatsGO; // Affiche les statistiques de l'objet
    public Image ItemImage; // Affiche l'image de l'objet s�lectionn�
    public Color baseColor; // Couleur de base des objets
    public Color selectdColor; // Couleur quand un objet est s�lectionn�
    public Color textColorNotNumber; // Couleur du texte pour les ressources manquantes
    public Color TextcolorNumber; // Couleur du texte pour les ressources suffisantes
    public Button Craftbtn; // Bouton de fabrication
    int selected = 0; // Indice de l'objet actuellement s�lectionn�
    public Slider progressBar; // Barre de progression pendant la fabrication
    bool canQuit = true; // Permet de v�rifier si on peut quitter l'interface d'artisanat

    private InputForPlayer _playerInput; // Entr�e du joueur

    List<GameObject> ItemsGO = new List<GameObject>(); // Liste des objets affich�s � fabriquer
    List<GameObject> RessourcesGO = new List<GameObject>(); // Liste des ressources n�cessaires

    private void Awake()
    {
        _playerInput = new InputForPlayer();

        // Lier l'action Escape � la fonction Quit
        _playerInput.Player.Escape.performed += Quit;
    }

    // Active les contr�les quand l'UI est activ�e
    private void OnEnable()
    {
        _playerInput.Enable();
    }

    // D�sactive les contr�les quand l'UI est d�sactiv�e
    private void OnDisable()
    {
        _playerInput.Disable();
    }

    // Fonction pour quitter l'interface d'artisanat
    private void Quit(InputAction.CallbackContext context)
    {
        if (!canQuit) return; // Emp�che de quitter si la fabrication est en cours
        EtablieUI.SetActive(false); // D�sactive l'UI
        Cursor.lockState = CursorLockMode.Locked; // Verrouille le curseur
        Cursor.visible = false; // Cache le curseur
    }

    // Initialisation de l'UI
    void Start()
    {
        ItemsGO = new List<GameObject>(); // Initialise la liste des objets
        EtablieUI.SetActive(false); // D�sactive l'interface d'artisanat au d�part
    }

    // Fonction d'initialisation pour l'artisanat
    public void Initialize()
    {
        EtablieUI.SetActive(true); // Active l'interface d'artisanat
        Cursor.lockState = CursorLockMode.None; // D�verrouille le curseur
        Cursor.visible = true; // Affiche le curseur

        // Supprime tous les objets et ressources pr�c�demment cr��s
        foreach (GameObject go in ItemsGO)
        {
            Destroy(go);
        }
        ItemsGO = new List<GameObject>();

        foreach (GameObject go in RessourcesGO)
        {
            Destroy(go);
        }
        RessourcesGO = new List<GameObject>();

        // Affiche tous les objets disponibles � fabriquer
        foreach (CraftBlueprint blueprint in CraftManager.Instance.crafts)
        {
            if (blueprint.craftUnlock) // V�rifie si l'objet est d�bloqu�
            {
                GameObject item = Instantiate(ItemPrefab, ItemContent.transform);
                ObjetCraft obj = item.GetComponent<ObjetCraft>();

                obj.id = ItemsGO.Count; // ID unique pour l'objet
                obj.parent = this;

                ItemsGO.Add(item); // Ajoute l'objet � la liste
                obj.img.sprite = blueprint.item.image; // D�finit l'image de l'objet
                obj.txt.text = blueprint.item.Nom + " (x" + blueprint.item.craftQuantity + ")"; // D�finit le texte de l'objet
                obj.GetComponent<Image>().color = baseColor; // Applique la couleur de base
            }
        }

        // R�initialise l'affichage des ressources et statistiques
        ItemImage.sprite = null;
        progressBar.gameObject.SetActive(false);
        StatsGO.SetActive(false);
    }

    // S�lectionne un objet � fabriquer en fonction de son ID
    public void Selected(int id)
    {
        int i = 0;
        foreach (GameObject obj in ItemsGO)
        {
            if (i == id) // Si l'objet est s�lectionn�, change sa couleur
            {
                obj.GetComponent<Image>().color = selectdColor;
            }
            else
            {
                obj.GetComponent<Image>().color = baseColor;
            }
            i++;
        }
        selected = id; // Met � jour l'objet s�lectionn�
        ItemImage.sprite = CraftManager.Instance.crafts[selected].item.image; // Affiche l'image de l'objet s�lectionn�
        SetRessource(); // Met � jour les ressources n�cessaires pour cet objet
    }

    // Met � jour l'affichage des ressources n�cessaires pour l'objet s�lectionn�
    public void SetRessource()
    {
        StatsGO.SetActive(true); // Affiche les statistiques
        StatsGO.GetComponent<ItemStatUpdate>().UpdateStats(CraftManager.Instance.crafts[selected].item); // Met � jour les statistiques de l'objet

        int nbGood = 0;
        foreach (GameObject go in RessourcesGO)
        {
            Destroy(go); // Supprime les anciennes ressources
        }

        RessourcesGO = new List<GameObject>();

        // Affiche chaque ressource n�cessaire
        foreach (CraftMaterial mat in CraftManager.Instance.crafts[selected].item.materials)
        {
            GameObject item = Instantiate(RessourcesPrefab, RessourcesContent.transform);
            ObjetCraft obj = item.GetComponent<ObjetCraft>();
            RessourcesGO.Add(item);

            obj.img.sprite = mat.item.image; // Affiche l'image de la ressource
            obj.txt.color = textColorNotNumber; // Applique la couleur de texte pour les ressources manquantes
            obj.txt.text = InventoryManager.instance.GetNbItemInInventory(mat.item) + "/" + mat.count; // Affiche la quantit� de la ressource n�cessaire / en inventaire
            if (InventoryManager.instance.GetNbItemInInventory(mat.item) >= mat.count)
            {
                obj.txt.color = TextcolorNumber; // Si on a suffisamment de ressources, change la couleur du texte
                nbGood++; // Augmente le compteur de ressources suffisantes
            }
        }

        // Active ou d�sactive le bouton de fabrication en fonction des ressources disponibles
        if (nbGood >= CraftManager.Instance.crafts[selected].item.materials.Count)
        {
            Craftbtn.interactable = true;
        }
        else
        {
            Craftbtn.interactable = false;
        }
    }

    // Fonction de fabrication de l'objet
    public void Craft()
    {
        List<CraftMaterial> mats = CraftManager.Instance.crafts[selected].item.materials;
        foreach (CraftMaterial mat in mats)
        {
            InventoryManager.instance.RemoveItem(mat.item, mat.count); // Retire les ressources n�cessaires
        }
        StartCoroutine(CraftCountdown()); // Lance la fabrication avec une barre de progression
    }

    // Coroutine pour g�rer la fabrication avec une barre de progression
    IEnumerator CraftCountdown()
    {
        canQuit = false; // Emp�che de quitter l'UI pendant la fabrication
        progressBar.gameObject.SetActive(true); // Affiche la barre de progression
        float second = CraftManager.Instance.crafts[selected].item.timeToCraft - PlayerStats.instance.Strategie; // Dur�e de la fabrication
        progressBar.maxValue = second;
        progressBar.minValue = 0;
        float i = 0;
        progressBar.value = i;
        Craftbtn.enabled = false; // D�sactive le bouton pendant la fabrication

        while (i < second)
        {
            yield return new WaitForSeconds(0.01f); // Attente pour simuler le temps de fabrication
            i += 0.01f;
            progressBar.value = i; // Mise � jour de la barre de progression
        }

        SetRessource(); // Met � jour les ressources apr�s la fabrication
        canQuit = true; // Permet de quitter l'interface apr�s la fabrication
        progressBar.gameObject.SetActive(false); // Masque la barre de progression
        InventoryManager.instance.AddItem(CraftManager.Instance.crafts[selected].item, CraftManager.Instance.crafts[selected].item.craftQuantity); // Ajoute l'objet fabriqu� � l'inventaire
    }
}
