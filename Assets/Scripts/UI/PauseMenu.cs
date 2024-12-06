using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[AddComponentMenu("#Pandemonium/UI/PauseMenu")]
public class PauseMenu : MonoBehaviour
{
    // Références aux objets du menu de pause et autres éléments UI
    public GameObject PauseGO; // Le GameObject contenant l'UI de pause
    public ThirdPersonController controller; // Le contrôleur du personnage
    public GameObject inventory, craft, victory, defete; // Références aux menus d'inventaire, craft, victoire et défaite

    // Référence à l'objet d'entrée pour le joueur (gère la saisie des commandes)
    private InputForPlayer _playerInput;

    // Variable pour savoir si le jeu est en pause ou non
    bool isPause = false;

    // Méthode d'initialisation où l'on prépare les entrées du joueur
    private void Awake()
    {
        _playerInput = new InputForPlayer();

        // Ajout d'un listener pour la touche 'Escape' (généralement utilisée pour la pause)
        _playerInput.Player.Escape.performed += PauseKey;
    }

    // Activation des entrées du joueur lorsqu'on active le script
    private void OnEnable()
    {
        _playerInput.Enable();
    }

    // Désactivation des entrées du joueur lorsqu'on désactive le script
    private void OnDisable()
    {
        _playerInput.Disable();
    }

    // Méthode appelée au démarrage du jeu pour configurer l'UI de pause
    void Start()
    {
        // L'UI de pause est cachée au départ
        PauseGO.SetActive(false);
    }

    // Méthode qui gère l'appui sur la touche de pause (Escape)
    private void PauseKey(InputAction.CallbackContext context)
    {
        Pause(); // Appelle la méthode Pause pour basculer entre pause et reprise
    }

    // Méthode pour quitter la scène actuelle et revenir au menu principal (scène 0)
    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    // Méthode pour recharger la scène actuelle (scène 1)
    public void Realod()
    {
        SceneManager.LoadScene(1);
    }

    // Méthode pour gérer la mise en pause et la reprise du jeu
    public void Pause()
    {
        // Empêche de mettre en pause si un autre menu comme l'inventaire ou le menu de victoire/échec est ouvert
        if (craft.activeSelf || inventory.activeSelf || victory.activeSelf || defete.activeSelf) return;

        if (isPause) // Si le jeu est déjà en pause, on le reprend
        {
            PauseGO.SetActive(false); // Cache l'UI de pause
            Cursor.lockState = CursorLockMode.Locked; // Verrouille le curseur au centre de l'écran
            Cursor.visible = false; // Cache le curseur
            Time.timeScale = 1; // Reprend le temps (le jeu continue)
            isPause = false; // Le jeu n'est plus en pause
            controller.LockCameraPosition = false; // Déverrouille la caméra du joueur
        }
        else // Si le jeu est en cours, on le met en pause
        {
            PauseGO.SetActive(true); // Affiche l'UI de pause
            Cursor.lockState = CursorLockMode.None; // Libère le curseur de l'écran
            Cursor.visible = true; // Affiche le curseur
            Time.timeScale = 0; // Met le temps en pause (arrête les mouvements et actions)
            isPause = true; // Le jeu est en pause
            controller.LockCameraPosition = true; // Verrouille la caméra du joueur (pour éviter qu'elle bouge pendant la pause)
        }
    }
}
