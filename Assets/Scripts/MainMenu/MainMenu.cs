using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/MainMenu/MainMenu")]
public class MainMenu : MonoBehaviour
{
    public GameObject OptionPanel;  // R�f�rence au panneau des options
    public Toggle fullScreen, vsync;  // R�f�rences aux cases � cocher pour plein �cran et V-Sync

    private void Start()
    {
        // D�finit la r�solution initiale et l'option plein �cran au d�marrage
        Screen.SetResolution(1920, 1080, true);  // D�finit la r�solution 1920x1080 en mode plein �cran
        OptionPanel.SetActive(false);  // Cache le panneau d'options au d�marrage
    }

    // Fonction pour quitter l'application
    public void Quit()
    {
        Application.Quit();  // Quitte le jeu
    }

    // Fonction pour commencer le jeu
    public void Play()
    {
        SceneManager.LoadScene(1);  // Charge la sc�ne avec l'index 1 (probablement la sc�ne de jeu)
    }

    // Fonction pour afficher ou masquer le panneau d'options
    public void Options()
    {
        OptionPanel.SetActive(!OptionPanel.activeSelf);  // Bascule l'�tat du panneau d'options (affich�/masqu�)
        fullScreen.isOn = Screen.fullScreen;  // Met � jour l'�tat de la case 'plein �cran' selon la configuration actuelle
        vsync.isOn = QualitySettings.vSyncCount > 0;  // Met � jour l'�tat de la case 'V-Sync' selon la configuration actuelle
    }

    // Fonction pour basculer entre le mode plein �cran ou non
    public void ToggleFullScreen(bool b)
    {
        Screen.fullScreen = b;  // Change le mode plein �cran en fonction du bool�en pass�
    }

    // Fonction pour activer/d�sactiver la synchronisation verticale (V-Sync)
    public void ToggleVSync(bool b)
    {
        QualitySettings.vSyncCount = b ? 1 : 0;  // Active ou d�sactive V-Sync selon l'�tat de la case � cocher
    }
}
