using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("#Pandemonium/Player/Control/PlayerAttack")]
[RequireComponent(typeof(ThirdPersonController))]
public class PlayerAttack : MonoBehaviour
{
    ThirdPersonController controller; // R�f�rence au contr�leur du joueur
    [HideInInspector] public bool canAttack = true; // Bool�en qui v�rifie si le joueur peut attaquer
    public Collider attackZone; // Zone de d�tection de l'attaque (environ un collider)

    PlayerStats playerStats;

    InputForPlayer _playerInput; // R�f�rence � l'objet qui g�re les entr�es du joueur

    float initiatialFOV; // Stocke le FOV initial pour la cam�ra
    public CinemachineVirtualCamera cam; // R�f�rence � la cam�ra virtuelle (Cinemachine)
    public float ZoomFOV = 15f; // Valeur du FOV lors du zoom (arme � distance)

    [SerializeField]
    WeaponItem weapon; // R�f�rence � l'arme �quip�e par le joueur

    // M�thode d'initialisation
    void Awake()
    {
        _playerInput = new InputForPlayer(); // Cr�e une nouvelle instance des entr�es pour le joueur

        // Abonne les actions aux m�thodes correspondantes
        _playerInput.Player.Attack1.started += AttackStart;
        _playerInput.Player.Attack1.canceled += AttackStop;
        _playerInput.Player.Zoom.started += ZoomStart;
        _playerInput.Player.Zoom.canceled += ZoomStop;
        _playerInput.Player.Reload.performed += Reload;
    }

    // M�thode Start : initialisation de certains �l�ments
    private void Start()
    {
        controller = GetComponent<ThirdPersonController>(); // R�cup�re la r�f�rence du contr�leur du joueur
        InventoryManager.instance.attack = this; // Associe ce script � l'instance de gestion d'inventaire
        initiatialFOV = cam.m_Lens.FieldOfView; // Sauvegarde le FOV initial de la cam�ra
        playerStats = GetComponent<PlayerStats>();
    }

    // M�thodes pour activer/d�sactiver les entr�es du joueur
    private void OnEnable()
    {
        _playerInput.Enable(); // Active les entr�es du joueur

        // Ajoute un proxy pour g�rer l'interaction de la zone d'attaque
        if (attackZone != null)
        {
            attackZone.gameObject.AddComponent<ChildTriggerProxy>().ParentHandler = this;
        }
    }

    private void OnDisable()
    {
        _playerInput.Disable(); // D�sactive les entr�es du joueur
    }

    // Mise � jour des animations et �tats en fonction de l'arme �quip�e
    private void Update()
    {
        // Si aucune arme n'est �quip�e, joue l'animation par d�faut
        if (weapon == null)
        {
            controller._animator.SetFloat("WeaponType", 0);
        }
        // Si une arme � distance est �quip�e
        else if (weapon.isRangeWeapon)
        {
            // D�finir le type d'arme en fonction du type de l'arme (l�g�re, lourde, etc.)
            switch (weapon.type)
            {
                case WeaponType.Legere: controller._animator.SetFloat("WeaponType", 1); break;
                case WeaponType.Lourde: controller._animator.SetFloat("WeaponType", 0.5f); break;
                default: controller._animator.SetFloat("WeaponType", 1); break;
            }
        }
        // Si une arme de m�l�e est �quip�e
        else
        {
            controller._animator.SetFloat("WeaponType", 0);
        }
    }

    // M�thode pour changer l'arme du joueur
    public void changeWeapon(WeaponItem _weapon)
    {
        weapon = _weapon; // Change l'arme actuelle
    }

    // M�thode pour d�marrer l'attaque (d�pend des entr�es du joueur)
    void AttackStart(InputAction.CallbackContext context)
    {
        // Si l'attaque est autoris�e
        if (!canAttack) return;

        // Si une arme est �quip�e
        if (weapon != null)
        {
            // Si l'arme est une arme � distance
            if (weapon.isRangeWeapon)
            {
                RangeWeapon rangeWeapon = InventoryManager.instance.weaponInstance.GetComponent<RangeWeapon>();
                // Si l'arme n'est pas automatique, g�re l'attaque semi-automatique
                if (!weapon.isAutomaticWeapon)
                {
                    rangeWeapon.HandleSemiAutomaticWeapon();
                }
                // Si l'arme est automatique, commence � tirer
                else
                {
                    rangeWeapon.fireing = true;
                }
            }
            else
            {
                // Si l'arme est une arme de m�l�e, joue l'animation d'attaque
                controller._animator.SetTrigger(controller._animIDAttack);
                StartCaCAttack(); // D�marre l'attaque en corps � corps
            }
        }
    }

    // M�thode pour arr�ter l'attaque (d�pend des entr�es du joueur)
    void AttackStop(InputAction.CallbackContext context)
    {
        // Si une arme est �quip�e et si c'est une arme � distance
        if (weapon != null)
        {
            if (weapon.isRangeWeapon)
            {
                RangeWeapon rangeWeapon = InventoryManager.instance.weaponInstance.GetComponent<RangeWeapon>();
                rangeWeapon.fireing = false; // Arr�te de tirer
            }
        }
    }

    // M�thode pour d�marrer le zoom (uniquement pour les armes � distance)
    void ZoomStart(InputAction.CallbackContext context)
    {
        if (weapon.isRangeWeapon)
        {
            controller._animator.SetBool("Zoom", true); // D�clenche l'animation de zoom
            cam.m_Lens.FieldOfView = ZoomFOV; // Change le FOV de la cam�ra pour le zoom
        }
    }

    // M�thode pour arr�ter le zoom (uniquement pour les armes � distance)
    void ZoomStop(InputAction.CallbackContext context)
    {
        if (weapon.isRangeWeapon)
        {
            controller._animator.SetBool("Zoom", false); // D�sactive l'animation de zoom
            cam.m_Lens.FieldOfView = initiatialFOV; // Restaure le FOV initial
        }
    }

    // M�thode pour recharger l'arme (uniquement pour les armes � distance)
    void Reload(InputAction.CallbackContext context)
    {
        if (weapon.isRangeWeapon)
        {
            Slots slot;
            // S�lectionne le bon slot en fonction de l'arme �quip�e
            switch (InventoryManager.instance.SlotUse)
            {
                case 0: slot = InventoryManager.instance.heavyWeapon; break;
                case 1: slot = InventoryManager.instance.mediumWeapon; break;
                case 2: slot = InventoryManager.instance.lightWeapon; break;
                default: slot = new Slots(); break;
            }
            WeaponItem item = slot.itemEntry.itemType as WeaponItem;

            // Calcule le nombre de balles restantes et ajuste le nombre de balles en cons�quence
            int nb = item.magazineSize - slot.itemEntry.number;

            slot.itemEntry.number = item.magazineSize - InventoryManager.instance.RemoveItem(item.ammo, nb);
            InventoryManager.instance.RefreshUIAmmo(); // Actualise l'interface utilisateur de la munition
        }
    }

    // G�re l'interaction avec un objet qui peut �tre endommag� (par exemple un rat)
    public void HandleChildTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damagable"))
        {
            other.GetComponent<RatsStats>().UpdateHealth(-1 * (weapon.damage + playerStats.Puissance), other.ClosestPoint(transform.position), weapon.forceMagnitude);
        }
    }

    // D�marre une attaque en corps � corps
    public void StartCaCAttack()
    {
        controller.CanMove = false; // D�sactive le mouvement du joueur pendant l'attaque
        StartCoroutine(StopAttack()); // Attends un certain temps avant de permettre � nouveau le mouvement
    }

    // Coroutine pour arr�ter l'attaque et r�activer le mouvement
    IEnumerator StopAttack()
    {
        yield return new WaitForSeconds(0.7f); // Attente de 0.7 secondes
        controller.CanMove = true; // Permet au joueur de se d�placer � nouveau apr�s l'attaque
    }
}
