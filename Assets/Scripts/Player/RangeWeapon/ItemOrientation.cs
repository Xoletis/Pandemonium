using UnityEngine;

// Ajoute un menu personnalisé dans l'éditeur Unity pour cet objet en particulier.
// Utilisé pour gérer l'orientation d'un objet en fonction de la caméra.
[AddComponentMenu("#Pandemonium/Player/RangeWeapon/ItemOrientation")]
public class ItemOrientation : MonoBehaviour
{
    // Référence à la caméra principale pour aligner l'orientation de l'objet avec celle de la caméra.
    public Transform cameraTransform;

    // Méthode appelée au démarrage du script.
    private void Start()
    {
        // Récupère la transformation de la caméra principale dans la scène.
        cameraTransform = Camera.main.transform;
    }

    // Méthode appelée à chaque frame.
    void Update()
    {
        // Vérifie si la référence à la caméra est valide.
        if (cameraTransform != null)
        {
            // Aligne l'orientation de l'objet avec la direction vers laquelle regarde la caméra.
            // `cameraTransform.forward` correspond à la direction de la caméra.
            transform.forward = cameraTransform.forward;
        }
    }
}
