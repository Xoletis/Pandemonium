using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/Environement/DestroyElement")]
public class DestroyElement : MonoBehaviour
{
    public Item itemToOpen;  // Item nécessaire pour ouvrir l'élément destructible.

    bool interactable = false;  // Booléen pour vérifier si l'élément est interactif avec le joueur.

    private InputForPlayer _playerInput;  // Référence aux entrées du joueur.

    public float ExploseTime = 0f;  // Temps avant l'explosion.
    public ParticleSystem explosion;  // Système de particules pour l'explosion.

    public Slider slider;  // Barre de progression pour le temps d'explosion.

    private void Awake()
    {
        // Initialise les entrées du joueur et assigne l'action d'interaction.
        _playerInput = new InputForPlayer();
        _playerInput.Player.Interact.performed += Open;  // Appel de la méthode Open lorsque le joueur interagit.
    }

    private void OnEnable()
    {
        _playerInput.Enable();  // Active les entrées du joueur lorsque l'objet est activé.
    }

    private void OnDisable()
    {
        _playerInput.Disable();  // Désactive les entrées du joueur lorsque l'objet est désactivé.
    }

    // Méthode appelée lorsque le joueur interagit avec l'élément.
    void Open(InputAction.CallbackContext context)
    {
        if (interactable)  // Vérifie si l'élément est interactif.
        {
            // Vérifie si le joueur a l'item nécessaire dans l'inventaire.
            if (InventoryManager.instance.GetNbItemInInventory(itemToOpen) >= 1)
            {
                // Retire l'item de l'inventaire et lance la coroutine de destruction.
                InventoryManager.instance.RemoveItem(itemToOpen, 1);
                StartCoroutine(TimeToDestroy());
            }
        }
    }

    // Coroutine pour gérer le temps avant la destruction de l'élément.
    IEnumerator TimeToDestroy()
    {
        Spawner spawn = GetComponentInChildren<Spawner>();  // Récupère le spawner enfant (si présent).
        if (spawn != null) spawn.Spawn(2);  // Appelle le spawn d'un nouvel élément de type 2.

        // Initialisation de la barre de progression.
        float i = 0;

        if (slider != null)
        {
            slider.gameObject.SetActive(true);  // Active la barre de progression si présente.

            slider.maxValue = ExploseTime;  // Définit la valeur maximale de la barre.
            slider.minValue = 0;  // Valeur minimale de la barre.
            slider.value = i;  // Initialise la barre à 0.
        }

        // Mise à jour de la barre de progression en fonction du temps d'explosion.
        while (i <= ExploseTime)
        {
            yield return new WaitForSeconds(0.01f);  // Attente avant la prochaine mise à jour.
            i += 0.01f;  // Incrémente le temps de l'explosion.
            if (slider != null) slider.value = i;  // Met à jour la valeur de la barre.
        }

        // Crée l'explosion à la position de l'élément.
        if (explosion != null)
        {
            Instantiate(explosion, transform.position + new Vector3(0, 4.25f, 0), Quaternion.identity);
        }

        // Appelle le spawn de type 3 et supprime l'élément du jeu.
        if (spawn != null)
        {
            spawn.Spawn(3);  // Appelle le spawn d'un élément de type 3.
            GameManager.instance.RemovePortal(gameObject);  // Supprime le portail de la scène.
        }

        Destroy(gameObject);  // Détruit l'objet de la scène.
    }

    // Méthode appelée lorsque le joueur entre en collision avec l'élément.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = true;  // Rend l'élément interactif.
            InventoryManager.instance.setGrab(true);  // Active l'option pour attraper l'élément.
        }
    }

    // Méthode appelée lorsque le joueur quitte la zone de collision de l'élément.
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = false;  // Rend l'élément non interactif.
            InventoryManager.instance.setGrab(false);  // Désactive l'option pour attraper l'élément.
        }
    }
}
