using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorUtils
{
    SerializedObject obj;

    public EditorUtils(SerializedObject obj)
    {
        this.obj = obj;
    }

    public bool AddClosableLabel(string label, bool foldout = true)
    {
        bool b = EditorGUILayout.Foldout(foldout, label);
        EditorGUI.indentLevel++;
        return b;

        
    }

    public void AddLabel(string label)
    {
        EditorGUILayout.LabelField(label);
        EditorGUI.indentLevel++;
    }

    public void AddProperty(string[] labels, bool interactable = true, string msg = "", bool show = true)
    {
        if (!show) return;

        if(!interactable) GUI.enabled = false;

        foreach (string label in labels) {
            EditorGUILayout.PropertyField(obj.FindProperty(label));
        }

        if (!interactable) GUI.enabled = true;
        if(msg != "") EditorGUILayout.HelpBox(msg, MessageType.Info);
    }

    public void DrawHorizontalLine()
    {
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    public void DrawCraftList(SerializedProperty listProperty)
    {
        EditorGUILayout.LabelField("Craft Materials", EditorStyles.boldLabel);

        // Récupérer tous les `Item` dans le dossier "Assets/Items"
        Item[] allItems = Resources.LoadAll<Item>("Items");

        for (int i = 0; i < listProperty.arraySize; i++)
        {
            SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
            SerializedProperty itemProp = element.FindPropertyRelative("item");
            SerializedProperty countProp = element.FindPropertyRelative("count");

            EditorGUILayout.BeginHorizontal();

            // Afficher un bouton pour ouvrir le dropdown
            if (GUILayout.Button(itemProp.objectReferenceValue != null ? itemProp.objectReferenceValue.name : "Select Item", EditorStyles.popup, GUILayout.Width(200)))
            {
                // Ouvrir le dropdown
                SearchableDropdown dropdown = new SearchableDropdown(
                    allItems,
                    selectedItem =>
                    {
                        itemProp.objectReferenceValue = selectedItem; // Mettre à jour la propriété sérialisée
                        obj.ApplyModifiedProperties();   // Appliquer les modifications
                    }
                );
                PopupWindow.Show(new Rect(Event.current.mousePosition, Vector2.zero), dropdown);
            }

            // Champ pour "count"
            EditorGUILayout.PropertyField(countProp, GUIContent.none, GUILayout.Width(50));

            // Bouton pour supprimer un élément
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                listProperty.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        // Ajouter un bouton pour ajouter de nouveaux éléments
        if (GUILayout.Button("Add Material"))
        {
            listProperty.InsertArrayElementAtIndex(listProperty.arraySize);
        }
    }
}
