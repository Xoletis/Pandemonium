using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("#Pandemonium/Environement/Rune")]
public class Rune : MonoBehaviour
{
    private InputForPlayer _playerInput; // Entr�e du joueur

    public RuneUI ui;
    public Divinite divinite;

    bool interactable = false;

    private void Awake()
    {
        _playerInput = new InputForPlayer();

        _playerInput.Player.Interact.performed += Open;
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

    void Open(InputAction.CallbackContext context)
    {
        if (interactable)  // Si l'objet est interactif (le joueur est � proximit�).
        {
            ui.Initialize(divinite);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Si l'objet en collision est le joueur.
        {
            interactable = true;  // Rend l'�tabli interactif.
            InventoryManager.instance.setGrab(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // Si l'objet en collision est le joueur.
        {
            interactable = false;  // Rend l'�tabli non interactif.
            InventoryManager.instance.setGrab(false);  // D�sactive l'option d'interaction.
        }
    }
}
