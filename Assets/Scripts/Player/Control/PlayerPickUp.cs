using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("#Pandemonium/Player/Control/PlayerPickUp")]
public class PlayerPickUp : MonoBehaviour
{
    public LayerMask pickableLayer; // Définir les objets qui peuvent être ramassés avec un Layer Mask
    private InputForPlayer input; // Référence aux entrées du joueur

    public GameObject objectIntercatible; // Référence à l'objet sur lequel le joueur peut interagir

    // Méthode d'initialisation
    private void Awake()
    {
        input = new InputForPlayer(); // Initialisation des entrées du joueur

        // Associer l'action d'interaction (de la manette/clavier) à la méthode Interact
        input.Player.Interact.performed += ctx => Interact(ctx);
    }

    // Méthode appelée quand le script est activé (lorsque le joueur peut interagir avec un objet)
    private void OnEnable()
    {
        input.Enable(); // Activer les entrées du joueur
    }

    // Méthode appelée quand le script est désactivé (lorsque le joueur ne peut plus interagir)
    private void OnDisable()
    {
        input.Disable(); // Désactiver les entrées du joueur
    }

    // Méthode d'interaction : appelée lorsqu'une action d'interaction est effectuée
    private void Interact(InputAction.CallbackContext context)
    {
        // Si un objet interactif est détecté
        if (objectIntercatible != null)
        {
            // Envoie un message à l'objet pour qu'il réponde à l'interaction (ex: ramasser l'objet)
            objectIntercatible.SendMessage("Interact");
        }
    }

    // Détecte lorsque le joueur entre dans le collider d'un objet
    private void OnTriggerEnter(Collider other)
    {
        // Vérifie si l'objet fait partie du layer "pickableLayer"
        if ((pickableLayer & (1 << other.gameObject.layer)) != 0)
        {
            objectIntercatible = other.gameObject; // Associe l'objet interactif à la variable
            InventoryManager.instance.setGrab(true); // Active l'état de saisie d'objet dans l'inventaire
        }
    }

    // Détecte lorsque le joueur quitte le collider d'un objet
    private void OnTriggerExit(Collider other)
    {
        // Vérifie si l'objet fait partie du layer "pickableLayer"
        if ((pickableLayer & (1 << other.gameObject.layer)) != 0)
        {
            objectIntercatible = null; // Désactive l'objet interactif
            InventoryManager.instance.setGrab(false); // Désactive l'état de saisie d'objet dans l'inventaire
        }
    }
}
