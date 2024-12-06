using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("#Pandemonium/Environement/EtablieInteract")]
public class EtablieInteract : MonoBehaviour
{
    public Etablie ui;  // Référence à l'interface utilisateur de l'établi (probablement un objet UI pour l'interaction avec l'établi).

    bool interactable = false;  // Booléen pour savoir si l'objet est interactif avec le joueur.

    private InputForPlayer _playerInput;  // Référence aux entrées du joueur pour gérer les actions d'interaction.

    private void Awake()
    {
        // Initialisation des entrées du joueur et assignation de l'action "Interact" à la méthode "Open".
        _playerInput = new InputForPlayer();
        _playerInput.Player.Interact.performed += Open;
    }

    private void OnEnable()
    {
        _playerInput.Enable();  // Active les entrées du joueur lorsque l'objet est activé.
    }

    private void OnDisable()
    {
        _playerInput.Disable();  // Désactive les entrées du joueur lorsque l'objet est désactivé.
    }

    // Méthode appelée lors de l'interaction du joueur (par exemple, lorsqu'il appuie sur un bouton d'interaction).
    void Open(InputAction.CallbackContext context)
    {
        if (interactable)  // Si l'objet est interactif (le joueur est à proximité).
        {
            ui.Initialize();
        }
    }

    // Méthode appelée lorsque le joueur entre dans la zone de collision de l'établi (probablement une zone Trigger).
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Si l'objet en collision est le joueur.
        {
            interactable = true;  // Rend l'établi interactif.
            InventoryManager.instance.setGrab(true);  // Permet au joueur d'interagir avec l'établi (ou d'autres actions liées).
        }
    }

    // Méthode appelée lorsque le joueur quitte la zone de collision de l'établi.
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // Si l'objet en collision est le joueur.
        {
            interactable = false;  // Rend l'établi non interactif.
            InventoryManager.instance.setGrab(false);  // Désactive l'option d'interaction.
        }
    }
}
