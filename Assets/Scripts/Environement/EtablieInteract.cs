using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("#Pandemonium/Environement/EtablieInteract")]
public class EtablieInteract : MonoBehaviour
{
    public Etablie ui;  // R�f�rence � l'interface utilisateur de l'�tabli (probablement un objet UI pour l'interaction avec l'�tabli).

    bool interactable = false;  // Bool�en pour savoir si l'objet est interactif avec le joueur.

    private InputForPlayer _playerInput;  // R�f�rence aux entr�es du joueur pour g�rer les actions d'interaction.

    private void Awake()
    {
        // Initialisation des entr�es du joueur et assignation de l'action "Interact" � la m�thode "Open".
        _playerInput = new InputForPlayer();
        _playerInput.Player.Interact.performed += Open;
    }

    private void OnEnable()
    {
        _playerInput.Enable();  // Active les entr�es du joueur lorsque l'objet est activ�.
    }

    private void OnDisable()
    {
        _playerInput.Disable();  // D�sactive les entr�es du joueur lorsque l'objet est d�sactiv�.
    }

    // M�thode appel�e lors de l'interaction du joueur (par exemple, lorsqu'il appuie sur un bouton d'interaction).
    void Open(InputAction.CallbackContext context)
    {
        if (interactable)  // Si l'objet est interactif (le joueur est � proximit�).
        {
            ui.Initialize();
        }
    }

    // M�thode appel�e lorsque le joueur entre dans la zone de collision de l'�tabli (probablement une zone Trigger).
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Si l'objet en collision est le joueur.
        {
            interactable = true;  // Rend l'�tabli interactif.
            InventoryManager.instance.setGrab(true);  // Permet au joueur d'interagir avec l'�tabli (ou d'autres actions li�es).
        }
    }

    // M�thode appel�e lorsque le joueur quitte la zone de collision de l'�tabli.
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // Si l'objet en collision est le joueur.
        {
            interactable = false;  // Rend l'�tabli non interactif.
            InventoryManager.instance.setGrab(false);  // D�sactive l'option d'interaction.
        }
    }
}
