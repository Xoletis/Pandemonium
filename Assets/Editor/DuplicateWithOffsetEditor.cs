using UnityEngine;
using UnityEditor;

public class DuplicateWithOffsetEditor : MonoBehaviour
{
    [MenuItem("GameObject/Duplicate With Z Offset %#d", false, 0)]
    public static void DuplicateWithOffset()
    {
        // V�rifier si un objet est s�lectionn�
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Veuillez s�lectionner un objet � dupliquer.");
            return;
        }

        // D�calage en Z
        float zOffset = 2f;

        // R�cup�rer l'objet s�lectionn�
        GameObject selectedObject = Selection.activeGameObject;

        // Dupliquer l'objet
        GameObject duplicatedObject = Instantiate(selectedObject);

        // D�finir un nom pour l'objet dupliqu�
        duplicatedObject.name = selectedObject.name;

        // Appliquer le d�calage
        Vector3 newPosition = selectedObject.transform.position;
        newPosition.z += zOffset;
        duplicatedObject.transform.position = newPosition;

        // Associer au m�me parent, si applicable
        duplicatedObject.transform.SetParent(selectedObject.transform.parent);

        // S�lectionner le nouvel objet dans l'�diteur
        Selection.activeGameObject = duplicatedObject;

        // Marquer la sc�ne comme modifi�e
        Undo.RegisterCreatedObjectUndo(duplicatedObject, "Duplicate With Z Offset");
    }
}
