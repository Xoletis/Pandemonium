using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[AddComponentMenu("#Pandemonium/UI/PauseMenu")]
public class PauseMenu : MonoBehaviour
{
    // R�f�rences aux objets du menu de pause et autres �l�ments UI
    public GameObject PauseGO; // Le GameObject contenant l'UI de pause
    public ThirdPersonController controller; // Le contr�leur du personnage
    public GameObject inventory, craft, victory, defete; // R�f�rences aux menus d'inventaire, craft, victoire et d�faite

    // R�f�rence � l'objet d'entr�e pour le joueur (g�re la saisie des commandes)
    private InputForPlayer _playerInput;

    // Variable pour savoir si le jeu est en pause ou non
    bool isPause = false;

    // M�thode d'initialisation o� l'on pr�pare les entr�es du joueur
    private void Awake()
    {
        _playerInput = new InputForPlayer();

        // Ajout d'un listener pour la touche 'Escape' (g�n�ralement utilis�e pour la pause)
        _playerInput.Player.Escape.performed += PauseKey;
    }

    // Activation des entr�es du joueur lorsqu'on active le script
    private void OnEnable()
    {
        _playerInput.Enable();
    }

    // D�sactivation des entr�es du joueur lorsqu'on d�sactive le script
    private void OnDisable()
    {
        _playerInput.Disable();
    }

    // M�thode appel�e au d�marrage du jeu pour configurer l'UI de pause
    void Start()
    {
        // L'UI de pause est cach�e au d�part
        PauseGO.SetActive(false);
    }

    // M�thode qui g�re l'appui sur la touche de pause (Escape)
    private void PauseKey(InputAction.CallbackContext context)
    {
        Pause(); // Appelle la m�thode Pause pour basculer entre pause et reprise
    }

    // M�thode pour quitter la sc�ne actuelle et revenir au menu principal (sc�ne 0)
    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    // M�thode pour recharger la sc�ne actuelle (sc�ne 1)
    public void Realod()
    {
        SceneManager.LoadScene(1);
    }

    // M�thode pour g�rer la mise en pause et la reprise du jeu
    public void Pause()
    {
        // Emp�che de mettre en pause si un autre menu comme l'inventaire ou le menu de victoire/�chec est ouvert
        if (craft.activeSelf || inventory.activeSelf || victory.activeSelf || defete.activeSelf) return;

        if (isPause) // Si le jeu est d�j� en pause, on le reprend
        {
            PauseGO.SetActive(false); // Cache l'UI de pause
            Cursor.lockState = CursorLockMode.Locked; // Verrouille le curseur au centre de l'�cran
            Cursor.visible = false; // Cache le curseur
            Time.timeScale = 1; // Reprend le temps (le jeu continue)
            isPause = false; // Le jeu n'est plus en pause
            controller.LockCameraPosition = false; // D�verrouille la cam�ra du joueur
        }
        else // Si le jeu est en cours, on le met en pause
        {
            PauseGO.SetActive(true); // Affiche l'UI de pause
            Cursor.lockState = CursorLockMode.None; // Lib�re le curseur de l'�cran
            Cursor.visible = true; // Affiche le curseur
            Time.timeScale = 0; // Met le temps en pause (arr�te les mouvements et actions)
            isPause = true; // Le jeu est en pause
            controller.LockCameraPosition = true; // Verrouille la cam�ra du joueur (pour �viter qu'elle bouge pendant la pause)
        }
    }
}
