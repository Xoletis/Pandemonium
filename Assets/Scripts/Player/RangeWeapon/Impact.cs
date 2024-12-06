using System.Collections;
using UnityEngine;

// Ajoute un menu personnalis� dans l'�diteur Unity pour cet objet en particulier.
// Utilis� pour g�rer l'impact d'une arme � distance.
[AddComponentMenu("#Pandemonium/Player/RangeWeapon/Impact")]
public class Impact : MonoBehaviour
{
    // M�thode appel�e au d�marrage de l'objet.
    void Start()
    {
        // D�marre la coroutine pour d�truire l'objet apr�s un d�lai.
        StartCoroutine(destoryAfter());
    }

    // Coroutine qui attend une seconde avant de d�truire l'objet.
    IEnumerator destoryAfter()
    {
        // Attend 1 seconde.
        yield return new WaitForSeconds(1);

        // D�truit l'objet courant.
        Destroy(gameObject);
    }
}
