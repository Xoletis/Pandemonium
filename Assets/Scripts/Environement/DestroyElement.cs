using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/Environement/DestroyElement")]
public class DestroyElement : MonoBehaviour
{
    public Item itemToOpen;  // Item n�cessaire pour ouvrir l'�l�ment destructible.

    bool interactable = false;  // Bool�en pour v�rifier si l'�l�ment est interactif avec le joueur.

    private InputForPlayer _playerInput;  // R�f�rence aux entr�es du joueur.

    public float ExploseTime = 0f;  // Temps avant l'explosion.
    public ParticleSystem explosion;  // Syst�me de particules pour l'explosion.

    public Slider slider;  // Barre de progression pour le temps d'explosion.

    private void Awake()
    {
        // Initialise les entr�es du joueur et assigne l'action d'interaction.
        _playerInput = new InputForPlayer();
        _playerInput.Player.Interact.performed += Open;  // Appel de la m�thode Open lorsque le joueur interagit.
    }

    private void OnEnable()
    {
        _playerInput.Enable();  // Active les entr�es du joueur lorsque l'objet est activ�.
    }

    private void OnDisable()
    {
        _playerInput.Disable();  // D�sactive les entr�es du joueur lorsque l'objet est d�sactiv�.
    }

    // M�thode appel�e lorsque le joueur interagit avec l'�l�ment.
    void Open(InputAction.CallbackContext context)
    {
        if (interactable)  // V�rifie si l'�l�ment est interactif.
        {
            // V�rifie si le joueur a l'item n�cessaire dans l'inventaire.
            if (InventoryManager.instance.GetNbItemInInventory(itemToOpen) >= 1)
            {
                // Retire l'item de l'inventaire et lance la coroutine de destruction.
                InventoryManager.instance.RemoveItem(itemToOpen, 1);
                StartCoroutine(TimeToDestroy());
            }
        }
    }

    // Coroutine pour g�rer le temps avant la destruction de l'�l�ment.
    IEnumerator TimeToDestroy()
    {
        Spawner spawn = GetComponentInChildren<Spawner>();  // R�cup�re le spawner enfant (si pr�sent).
        if (spawn != null) spawn.Spawn(2);  // Appelle le spawn d'un nouvel �l�ment de type 2.

        // Initialisation de la barre de progression.
        float i = 0;

        if (slider != null)
        {
            slider.gameObject.SetActive(true);  // Active la barre de progression si pr�sente.

            slider.maxValue = ExploseTime;  // D�finit la valeur maximale de la barre.
            slider.minValue = 0;  // Valeur minimale de la barre.
            slider.value = i;  // Initialise la barre � 0.
        }

        // Mise � jour de la barre de progression en fonction du temps d'explosion.
        while (i <= ExploseTime)
        {
            yield return new WaitForSeconds(0.01f);  // Attente avant la prochaine mise � jour.
            i += 0.01f;  // Incr�mente le temps de l'explosion.
            if (slider != null) slider.value = i;  // Met � jour la valeur de la barre.
        }

        // Cr�e l'explosion � la position de l'�l�ment.
        if (explosion != null)
        {
            Instantiate(explosion, transform.position + new Vector3(0, 4.25f, 0), Quaternion.identity);
        }

        // Appelle le spawn de type 3 et supprime l'�l�ment du jeu.
        if (spawn != null)
        {
            spawn.Spawn(3);  // Appelle le spawn d'un �l�ment de type 3.
            GameManager.instance.RemovePortal(gameObject);  // Supprime le portail de la sc�ne.
        }

        Destroy(gameObject);  // D�truit l'objet de la sc�ne.
    }

    // M�thode appel�e lorsque le joueur entre en collision avec l'�l�ment.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = true;  // Rend l'�l�ment interactif.
            InventoryManager.instance.setGrab(true);  // Active l'option pour attraper l'�l�ment.
        }
    }

    // M�thode appel�e lorsque le joueur quitte la zone de collision de l'�l�ment.
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = false;  // Rend l'�l�ment non interactif.
            InventoryManager.instance.setGrab(false);  // D�sactive l'option pour attraper l'�l�ment.
        }
    }
}
