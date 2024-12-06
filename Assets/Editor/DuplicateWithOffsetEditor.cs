using UnityEngine;
using UnityEditor;

public class DuplicateWithOffsetEditor : MonoBehaviour
{
    [MenuItem("GameObject/Duplicate With Z Offset %#d", false, 0)]
    public static void DuplicateWithOffset()
    {
        // Vérifier si un objet est sélectionné
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Veuillez sélectionner un objet à dupliquer.");
            return;
        }

        // Décalage en Z
        float zOffset = 2f;

        // Récupérer l'objet sélectionné
        GameObject selectedObject = Selection.activeGameObject;

        // Dupliquer l'objet
        GameObject duplicatedObject = Instantiate(selectedObject);

        // Définir un nom pour l'objet dupliqué
        duplicatedObject.name = selectedObject.name;

        // Appliquer le décalage
        Vector3 newPosition = selectedObject.transform.position;
        newPosition.z += zOffset;
        duplicatedObject.transform.position = newPosition;

        // Associer au même parent, si applicable
        duplicatedObject.transform.SetParent(selectedObject.transform.parent);

        // Sélectionner le nouvel objet dans l'éditeur
        Selection.activeGameObject = duplicatedObject;

        // Marquer la scène comme modifiée
        Undo.RegisterCreatedObjectUndo(duplicatedObject, "Duplicate With Z Offset");
    }
}
