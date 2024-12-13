using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("#Pandemonium/Player/Control/PlayerAttack")]
[RequireComponent(typeof(ThirdPersonController))]
public class PlayerAttack : MonoBehaviour
{
    ThirdPersonController controller; // Référence au contrôleur du joueur
    [HideInInspector] public bool canAttack = true; // Booléen qui vérifie si le joueur peut attaquer
    public Collider attackZone; // Zone de détection de l'attaque (environ un collider)

    PlayerStats playerStats;

    InputForPlayer _playerInput; // Référence à l'objet qui gère les entrées du joueur

    float initiatialFOV; // Stocke le FOV initial pour la caméra
    public CinemachineVirtualCamera cam; // Référence à la caméra virtuelle (Cinemachine)
    public float ZoomFOV = 15f; // Valeur du FOV lors du zoom (arme à distance)

    [SerializeField]
    WeaponItem weapon; // Référence à l'arme équipée par le joueur

    // Méthode d'initialisation
    void Awake()
    {
        _playerInput = new InputForPlayer(); // Crée une nouvelle instance des entrées pour le joueur

        // Abonne les actions aux méthodes correspondantes
        _playerInput.Player.Attack1.started += AttackStart;
        _playerInput.Player.Attack1.canceled += AttackStop;
        _playerInput.Player.Zoom.started += ZoomStart;
        _playerInput.Player.Zoom.canceled += ZoomStop;
        _playerInput.Player.Reload.performed += Reload;
    }

    // Méthode Start : initialisation de certains éléments
    private void Start()
    {
        controller = GetComponent<ThirdPersonController>(); // Récupère la référence du contrôleur du joueur
        InventoryManager.instance.attack = this; // Associe ce script à l'instance de gestion d'inventaire
        initiatialFOV = cam.m_Lens.FieldOfView; // Sauvegarde le FOV initial de la caméra
        playerStats = GetComponent<PlayerStats>();
    }

    // Méthodes pour activer/désactiver les entrées du joueur
    private void OnEnable()
    {
        _playerInput.Enable(); // Active les entrées du joueur

        // Ajoute un proxy pour gérer l'interaction de la zone d'attaque
        if (attackZone != null)
        {
            attackZone.gameObject.AddComponent<ChildTriggerProxy>().ParentHandler = this;
        }
    }

    private void OnDisable()
    {
        _playerInput.Disable(); // Désactive les entrées du joueur
    }

    // Mise à jour des animations et états en fonction de l'arme équipée
    private void Update()
    {
        // Si aucune arme n'est équipée, joue l'animation par défaut
        if (weapon == null)
        {
            controller._animator.SetFloat("WeaponType", 0);
        }
        // Si une arme à distance est équipée
        else if (weapon.isRangeWeapon)
        {
            // Définir le type d'arme en fonction du type de l'arme (légère, lourde, etc.)
            switch (weapon.type)
            {
                case WeaponType.Legere: controller._animator.SetFloat("WeaponType", 1); break;
                case WeaponType.Lourde: controller._animator.SetFloat("WeaponType", 0.5f); break;
                default: controller._animator.SetFloat("WeaponType", 1); break;
            }
        }
        // Si une arme de mêlée est équipée
        else
        {
            controller._animator.SetFloat("WeaponType", 0);
        }
    }

    // Méthode pour changer l'arme du joueur
    public void changeWeapon(WeaponItem _weapon)
    {
        weapon = _weapon; // Change l'arme actuelle
    }

    // Méthode pour démarrer l'attaque (dépend des entrées du joueur)
    void AttackStart(InputAction.CallbackContext context)
    {
        // Si l'attaque est autorisée
        if (!canAttack) return;

        // Si une arme est équipée
        if (weapon != null)
        {
            // Si l'arme est une arme à distance
            if (weapon.isRangeWeapon)
            {
                RangeWeapon rangeWeapon = InventoryManager.instance.weaponInstance.GetComponent<RangeWeapon>();
                // Si l'arme n'est pas automatique, gère l'attaque semi-automatique
                if (!weapon.isAutomaticWeapon)
                {
                    rangeWeapon.HandleSemiAutomaticWeapon();
                }
                // Si l'arme est automatique, commence à tirer
                else
                {
                    rangeWeapon.fireing = true;
                }
            }
            else
            {
                // Si l'arme est une arme de mêlée, joue l'animation d'attaque
                controller._animator.SetTrigger(controller._animIDAttack);
                StartCaCAttack(); // Démarre l'attaque en corps à corps
            }
        }
    }

    // Méthode pour arrêter l'attaque (dépend des entrées du joueur)
    void AttackStop(InputAction.CallbackContext context)
    {
        // Si une arme est équipée et si c'est une arme à distance
        if (weapon != null)
        {
            if (weapon.isRangeWeapon)
            {
                RangeWeapon rangeWeapon = InventoryManager.instance.weaponInstance.GetComponent<RangeWeapon>();
                rangeWeapon.fireing = false; // Arrête de tirer
            }
        }
    }

    // Méthode pour démarrer le zoom (uniquement pour les armes à distance)
    void ZoomStart(InputAction.CallbackContext context)
    {
        if (weapon.isRangeWeapon)
        {
            controller._animator.SetBool("Zoom", true); // Déclenche l'animation de zoom
            cam.m_Lens.FieldOfView = ZoomFOV; // Change le FOV de la caméra pour le zoom
        }
    }

    // Méthode pour arrêter le zoom (uniquement pour les armes à distance)
    void ZoomStop(InputAction.CallbackContext context)
    {
        if (weapon.isRangeWeapon)
        {
            controller._animator.SetBool("Zoom", false); // Désactive l'animation de zoom
            cam.m_Lens.FieldOfView = initiatialFOV; // Restaure le FOV initial
        }
    }

    // Méthode pour recharger l'arme (uniquement pour les armes à distance)
    void Reload(InputAction.CallbackContext context)
    {
        if (weapon.isRangeWeapon)
        {
            Slots slot;
            // Sélectionne le bon slot en fonction de l'arme équipée
            switch (InventoryManager.instance.SlotUse)
            {
                case 0: slot = InventoryManager.instance.heavyWeapon; break;
                case 1: slot = InventoryManager.instance.mediumWeapon; break;
                case 2: slot = InventoryManager.instance.lightWeapon; break;
                default: slot = new Slots(); break;
            }
            WeaponItem item = slot.itemEntry.itemType as WeaponItem;

            // Calcule le nombre de balles restantes et ajuste le nombre de balles en conséquence
            int nb = item.magazineSize - slot.itemEntry.number;

            slot.itemEntry.number = item.magazineSize - InventoryManager.instance.RemoveItem(item.ammo, nb);
            InventoryManager.instance.RefreshUIAmmo(); // Actualise l'interface utilisateur de la munition
        }
    }

    // Gère l'interaction avec un objet qui peut être endommagé (par exemple un rat)
    public void HandleChildTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damagable"))
        {
            other.GetComponent<RatsStats>().UpdateHealth(-1 * (weapon.damage + playerStats.Puissance), other.ClosestPoint(transform.position), weapon.forceMagnitude);
        }
    }

    // Démarre une attaque en corps à corps
    public void StartCaCAttack()
    {
        controller.CanMove = false; // Désactive le mouvement du joueur pendant l'attaque
        StartCoroutine(StopAttack()); // Attends un certain temps avant de permettre à nouveau le mouvement
    }

    // Coroutine pour arrêter l'attaque et réactiver le mouvement
    IEnumerator StopAttack()
    {
        yield return new WaitForSeconds(0.7f); // Attente de 0.7 secondes
        controller.CanMove = true; // Permet au joueur de se déplacer à nouveau après l'attaque
    }
}
