using UnityEngine;

// Ajoute un menu personnalis� dans l'�diteur Unity pour cet objet en particulier.
// Utilis� pour g�rer l'orientation d'un objet en fonction de la cam�ra.
[AddComponentMenu("#Pandemonium/Player/RangeWeapon/ItemOrientation")]
public class ItemOrientation : MonoBehaviour
{
    // R�f�rence � la cam�ra principale pour aligner l'orientation de l'objet avec celle de la cam�ra.
    public Transform cameraTransform;

    // M�thode appel�e au d�marrage du script.
    private void Start()
    {
        // R�cup�re la transformation de la cam�ra principale dans la sc�ne.
        cameraTransform = Camera.main.transform;
    }

    // M�thode appel�e � chaque frame.
    void Update()
    {
        // V�rifie si la r�f�rence � la cam�ra est valide.
        if (cameraTransform != null)
        {
            // Aligne l'orientation de l'objet avec la direction vers laquelle regarde la cam�ra.
            // `cameraTransform.forward` correspond � la direction de la cam�ra.
            transform.forward = cameraTransform.forward;
        }
    }
}
