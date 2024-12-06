using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

[AddComponentMenu("#Pandemonium/MainMenu/LoadingScreen")]
public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;  // Référence à la barre de progression
    public TextMeshProUGUI progressText;  // Référence au texte affichant le pourcentage

    private void Start()
    {
        LoadScene("Map");  // Appelle la fonction LoadScene pour charger la scène "Map" au démarrage
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));  // Démarre le chargement de la scène de manière asynchrone
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        // Charge la scène de manière asynchrone
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // Empêche l'activation automatique de la scène dès qu'elle est chargée
        operation.allowSceneActivation = false;

        while (!operation.isDone)  // Tant que le chargement de la scène n'est pas terminé
        {
            // Calcule la progression du chargement
            float progress = Mathf.Clamp01(operation.progress / 0.9f);  // Normalise la progression entre 0 et 1

            // Met à jour la barre de progression et le texte
            if (progressBar != null)
                progressBar.value = progress;  // Met à jour la valeur de la barre de progression
            if (progressText != null)
                progressText.text = $"{(progress * 100):0}%";  // Affiche le pourcentage dans le texte

            // Une fois le chargement atteint 90%, affiche un message pour que l'utilisateur appuie sur une touche pour continuer
            if (operation.progress >= 0.9f)
            {
                if (progressText != null)
                    progressText.text = "Appuyez sur une touche pour continuer";  // Texte d'instruction pour l'utilisateur

                // Si une touche est pressée, la scène est activée
                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;  // Active la scène
                }
            }

            yield return null;  // Attend la prochaine frame
        }
    }
}
