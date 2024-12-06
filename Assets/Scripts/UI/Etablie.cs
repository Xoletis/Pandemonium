using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/UI/Etablie")]
public class Etablie : MonoBehaviour
{
    // Références aux objets de l'UI
    public GameObject EtablieUI; // Le panneau d'artisanat (interface)
    public GameObject ItemContent; // Conteneur pour les objets à fabriquer
    public GameObject ItemPrefab; // Le prefab pour chaque objet à fabriquer
    public GameObject RessourcesPrefab; // Le prefab pour les ressources nécessaires à la fabrication
    public GameObject RessourcesContent; // Conteneur pour afficher les ressources
    public GameObject StatsGO; // Affiche les statistiques de l'objet
    public Image ItemImage; // Affiche l'image de l'objet sélectionné
    public Color baseColor; // Couleur de base des objets
    public Color selectdColor; // Couleur quand un objet est sélectionné
    public Color textColorNotNumber; // Couleur du texte pour les ressources manquantes
    public Color TextcolorNumber; // Couleur du texte pour les ressources suffisantes
    public Button Craftbtn; // Bouton de fabrication
    int selected = 0; // Indice de l'objet actuellement sélectionné
    public Slider progressBar; // Barre de progression pendant la fabrication
    bool canQuit = true; // Permet de vérifier si on peut quitter l'interface d'artisanat

    private InputForPlayer _playerInput; // Entrée du joueur

    List<GameObject> ItemsGO = new List<GameObject>(); // Liste des objets affichés à fabriquer
    List<GameObject> RessourcesGO = new List<GameObject>(); // Liste des ressources nécessaires

    private void Awake()
    {
        _playerInput = new InputForPlayer();

        // Lier l'action Escape à la fonction Quit
        _playerInput.Player.Escape.performed += Quit;
    }

    // Active les contrôles quand l'UI est activée
    private void OnEnable()
    {
        _playerInput.Enable();
    }

    // Désactive les contrôles quand l'UI est désactivée
    private void OnDisable()
    {
        _playerInput.Disable();
    }

    // Fonction pour quitter l'interface d'artisanat
    private void Quit(InputAction.CallbackContext context)
    {
        if (!canQuit) return; // Empêche de quitter si la fabrication est en cours
        EtablieUI.SetActive(false); // Désactive l'UI
        Cursor.lockState = CursorLockMode.Locked; // Verrouille le curseur
        Cursor.visible = false; // Cache le curseur
    }

    // Initialisation de l'UI
    void Start()
    {
        ItemsGO = new List<GameObject>(); // Initialise la liste des objets
        EtablieUI.SetActive(false); // Désactive l'interface d'artisanat au départ
    }

    // Fonction d'initialisation pour l'artisanat
    public void Initialize()
    {
        EtablieUI.SetActive(true); // Active l'interface d'artisanat
        Cursor.lockState = CursorLockMode.None; // Déverrouille le curseur
        Cursor.visible = true; // Affiche le curseur

        // Supprime tous les objets et ressources précédemment créés
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

        // Affiche tous les objets disponibles à fabriquer
        foreach (CraftBlueprint blueprint in CraftManager.Instance.crafts)
        {
            if (blueprint.craftUnlock) // Vérifie si l'objet est débloqué
            {
                GameObject item = Instantiate(ItemPrefab, ItemContent.transform);
                ObjetCraft obj = item.GetComponent<ObjetCraft>();

                obj.id = ItemsGO.Count; // ID unique pour l'objet
                obj.parent = this;

                ItemsGO.Add(item); // Ajoute l'objet à la liste
                obj.img.sprite = blueprint.item.image; // Définit l'image de l'objet
                obj.txt.text = blueprint.item.Nom + " (x" + blueprint.item.craftQuantity + ")"; // Définit le texte de l'objet
                obj.GetComponent<Image>().color = baseColor; // Applique la couleur de base
            }
        }

        // Réinitialise l'affichage des ressources et statistiques
        ItemImage.sprite = null;
        progressBar.gameObject.SetActive(false);
        StatsGO.SetActive(false);
    }

    // Sélectionne un objet à fabriquer en fonction de son ID
    public void Selected(int id)
    {
        int i = 0;
        foreach (GameObject obj in ItemsGO)
        {
            if (i == id) // Si l'objet est sélectionné, change sa couleur
            {
                obj.GetComponent<Image>().color = selectdColor;
            }
            else
            {
                obj.GetComponent<Image>().color = baseColor;
            }
            i++;
        }
        selected = id; // Met à jour l'objet sélectionné
        ItemImage.sprite = CraftManager.Instance.crafts[selected].item.image; // Affiche l'image de l'objet sélectionné
        SetRessource(); // Met à jour les ressources nécessaires pour cet objet
    }

    // Met à jour l'affichage des ressources nécessaires pour l'objet sélectionné
    public void SetRessource()
    {
        StatsGO.SetActive(true); // Affiche les statistiques
        StatsGO.GetComponent<ItemStatUpdate>().UpdateStats(CraftManager.Instance.crafts[selected].item); // Met à jour les statistiques de l'objet

        int nbGood = 0;
        foreach (GameObject go in RessourcesGO)
        {
            Destroy(go); // Supprime les anciennes ressources
        }

        RessourcesGO = new List<GameObject>();

        // Affiche chaque ressource nécessaire
        foreach (CraftMaterial mat in CraftManager.Instance.crafts[selected].item.materials)
        {
            GameObject item = Instantiate(RessourcesPrefab, RessourcesContent.transform);
            ObjetCraft obj = item.GetComponent<ObjetCraft>();
            RessourcesGO.Add(item);

            obj.img.sprite = mat.item.image; // Affiche l'image de la ressource
            obj.txt.color = textColorNotNumber; // Applique la couleur de texte pour les ressources manquantes
            obj.txt.text = InventoryManager.instance.GetNbItemInInventory(mat.item) + "/" + mat.count; // Affiche la quantité de la ressource nécessaire / en inventaire
            if (InventoryManager.instance.GetNbItemInInventory(mat.item) >= mat.count)
            {
                obj.txt.color = TextcolorNumber; // Si on a suffisamment de ressources, change la couleur du texte
                nbGood++; // Augmente le compteur de ressources suffisantes
            }
        }

        // Active ou désactive le bouton de fabrication en fonction des ressources disponibles
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
            InventoryManager.instance.RemoveItem(mat.item, mat.count); // Retire les ressources nécessaires
        }
        StartCoroutine(CraftCountdown()); // Lance la fabrication avec une barre de progression
    }

    // Coroutine pour gérer la fabrication avec une barre de progression
    IEnumerator CraftCountdown()
    {
        canQuit = false; // Empêche de quitter l'UI pendant la fabrication
        progressBar.gameObject.SetActive(true); // Affiche la barre de progression
        float second = CraftManager.Instance.crafts[selected].item.timeToCraft - PlayerStats.instance.Strategie; // Durée de la fabrication
        progressBar.maxValue = second;
        progressBar.minValue = 0;
        float i = 0;
        progressBar.value = i;
        Craftbtn.enabled = false; // Désactive le bouton pendant la fabrication

        while (i < second)
        {
            yield return new WaitForSeconds(0.01f); // Attente pour simuler le temps de fabrication
            i += 0.01f;
            progressBar.value = i; // Mise à jour de la barre de progression
        }

        SetRessource(); // Met à jour les ressources après la fabrication
        canQuit = true; // Permet de quitter l'interface après la fabrication
        progressBar.gameObject.SetActive(false); // Masque la barre de progression
        InventoryManager.instance.AddItem(CraftManager.Instance.crafts[selected].item, CraftManager.Instance.crafts[selected].item.craftQuantity); // Ajoute l'objet fabriqué à l'inventaire
    }
}
