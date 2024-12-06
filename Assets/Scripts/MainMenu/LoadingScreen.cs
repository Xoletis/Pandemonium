using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

[AddComponentMenu("#Pandemonium/MainMenu/LoadingScreen")]
public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;  // R�f�rence � la barre de progression
    public TextMeshProUGUI progressText;  // R�f�rence au texte affichant le pourcentage

    private void Start()
    {
        LoadScene("Map");  // Appelle la fonction LoadScene pour charger la sc�ne "Map" au d�marrage
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));  // D�marre le chargement de la sc�ne de mani�re asynchrone
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        // Charge la sc�ne de mani�re asynchrone
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // Emp�che l'activation automatique de la sc�ne d�s qu'elle est charg�e
        operation.allowSceneActivation = false;

        while (!operation.isDone)  // Tant que le chargement de la sc�ne n'est pas termin�
        {
            // Calcule la progression du chargement
            float progress = Mathf.Clamp01(operation.progress / 0.9f);  // Normalise la progression entre 0 et 1

            // Met � jour la barre de progression et le texte
            if (progressBar != null)
                progressBar.value = progress;  // Met � jour la valeur de la barre de progression
            if (progressText != null)
                progressText.text = $"{(progress * 100):0}%";  // Affiche le pourcentage dans le texte

            // Une fois le chargement atteint 90%, affiche un message pour que l'utilisateur appuie sur une touche pour continuer
            if (operation.progress >= 0.9f)
            {
                if (progressText != null)
                    progressText.text = "Appuyez sur une touche pour continuer";  // Texte d'instruction pour l'utilisateur

                // Si une touche est press�e, la sc�ne est activ�e
                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;  // Active la sc�ne
                }
            }

            yield return null;  // Attend la prochaine frame
        }
    }
}
