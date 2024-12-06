using System.Collections;
using UnityEngine;

// Ajoute un menu personnalisé dans l'éditeur Unity pour cet objet en particulier.
// Utilisé pour gérer l'impact d'une arme à distance.
[AddComponentMenu("#Pandemonium/Player/RangeWeapon/Impact")]
public class Impact : MonoBehaviour
{
    // Méthode appelée au démarrage de l'objet.
    void Start()
    {
        // Démarre la coroutine pour détruire l'objet après un délai.
        StartCoroutine(destoryAfter());
    }

    // Coroutine qui attend une seconde avant de détruire l'objet.
    IEnumerator destoryAfter()
    {
        // Attend 1 seconde.
        yield return new WaitForSeconds(1);

        // Détruit l'objet courant.
        Destroy(gameObject);
    }
}
