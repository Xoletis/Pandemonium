using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("#Pandemonium/Player/Control/PlayerPickUp")]
public class PlayerPickUp : MonoBehaviour
{
    public LayerMask pickableLayer; // D�finir les objets qui peuvent �tre ramass�s avec un Layer Mask
    private InputForPlayer input; // R�f�rence aux entr�es du joueur

    public GameObject objectIntercatible; // R�f�rence � l'objet sur lequel le joueur peut interagir

    // M�thode d'initialisation
    private void Awake()
    {
        input = new InputForPlayer(); // Initialisation des entr�es du joueur

        // Associer l'action d'interaction (de la manette/clavier) � la m�thode Interact
        input.Player.Interact.performed += ctx => Interact(ctx);
    }

    // M�thode appel�e quand le script est activ� (lorsque le joueur peut interagir avec un objet)
    private void OnEnable()
    {
        input.Enable(); // Activer les entr�es du joueur
    }

    // M�thode appel�e quand le script est d�sactiv� (lorsque le joueur ne peut plus interagir)
    private void OnDisable()
    {
        input.Disable(); // D�sactiver les entr�es du joueur
    }

    // M�thode d'interaction : appel�e lorsqu'une action d'interaction est effectu�e
    private void Interact(InputAction.CallbackContext context)
    {
        // Si un objet interactif est d�tect�
        if (objectIntercatible != null)
        {
            // Envoie un message � l'objet pour qu'il r�ponde � l'interaction (ex: ramasser l'objet)
            objectIntercatible.SendMessage("Interact");
        }
    }

    // D�tecte lorsque le joueur entre dans le collider d'un objet
    private void OnTriggerEnter(Collider other)
    {
        // V�rifie si l'objet fait partie du layer "pickableLayer"
        if ((pickableLayer & (1 << other.gameObject.layer)) != 0)
        {
            objectIntercatible = other.gameObject; // Associe l'objet interactif � la variable
            InventoryManager.instance.setGrab(true); // Active l'�tat de saisie d'objet dans l'inventaire
        }
    }

    // D�tecte lorsque le joueur quitte le collider d'un objet
    private void OnTriggerExit(Collider other)
    {
        // V�rifie si l'objet fait partie du layer "pickableLayer"
        if ((pickableLayer & (1 << other.gameObject.layer)) != 0)
        {
            objectIntercatible = null; // D�sactive l'objet interactif
            InventoryManager.instance.setGrab(false); // D�sactive l'�tat de saisie d'objet dans l'inventaire
        }
    }
}
