using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryManager))]
public class InventoryManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        InventoryManager inventoryManager = (InventoryManager)target;

        if (GUILayout.Button("Ajoute les Slots"))
        {
            inventoryManager.PopulateSlots();
            EditorUtility.SetDirty(inventoryManager);
        }
    }
}
