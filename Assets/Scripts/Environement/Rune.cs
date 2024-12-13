using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("#Pandemonium/Environement/Rune")]
public class Rune : MonoBehaviour
{
    private InputForPlayer _playerInput; // Entrée du joueur

    public RuneUI ui;
    public Divinite divinite;

    bool interactable = false;

    private void Awake()
    {
        _playerInput = new InputForPlayer();

        _playerInput.Player.Interact.performed += Open;
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

    void Open(InputAction.CallbackContext context)
    {
        if (interactable)  // Si l'objet est interactif (le joueur est à proximité).
        {
            ui.Initialize(divinite);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Si l'objet en collision est le joueur.
        {
            interactable = true;  // Rend l'établi interactif.
            InventoryManager.instance.setGrab(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // Si l'objet en collision est le joueur.
        {
            interactable = false;  // Rend l'établi non interactif.
            InventoryManager.instance.setGrab(false);  // Désactive l'option d'interaction.
        }
    }
}
