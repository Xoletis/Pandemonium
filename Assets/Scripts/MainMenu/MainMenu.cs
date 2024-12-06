using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/MainMenu/MainMenu")]
public class MainMenu : MonoBehaviour
{
    public GameObject OptionPanel;  // Référence au panneau des options
    public Toggle fullScreen, vsync;  // Références aux cases à cocher pour plein écran et V-Sync

    private void Start()
    {
        // Définit la résolution initiale et l'option plein écran au démarrage
        Screen.SetResolution(1920, 1080, true);  // Définit la résolution 1920x1080 en mode plein écran
        OptionPanel.SetActive(false);  // Cache le panneau d'options au démarrage
    }

    // Fonction pour quitter l'application
    public void Quit()
    {
        Application.Quit();  // Quitte le jeu
    }

    // Fonction pour commencer le jeu
    public void Play()
    {
        SceneManager.LoadScene(1);  // Charge la scène avec l'index 1 (probablement la scène de jeu)
    }

    // Fonction pour afficher ou masquer le panneau d'options
    public void Options()
    {
        OptionPanel.SetActive(!OptionPanel.activeSelf);  // Bascule l'état du panneau d'options (affiché/masqué)
        fullScreen.isOn = Screen.fullScreen;  // Met à jour l'état de la case 'plein écran' selon la configuration actuelle
        vsync.isOn = QualitySettings.vSyncCount > 0;  // Met à jour l'état de la case 'V-Sync' selon la configuration actuelle
    }

    // Fonction pour basculer entre le mode plein écran ou non
    public void ToggleFullScreen(bool b)
    {
        Screen.fullScreen = b;  // Change le mode plein écran en fonction du booléen passé
    }

    // Fonction pour activer/désactiver la synchronisation verticale (V-Sync)
    public void ToggleVSync(bool b)
    {
        QualitySettings.vSyncCount = b ? 1 : 0;  // Active ou désactive V-Sync selon l'état de la case à cocher
    }
}
