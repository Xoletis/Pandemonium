using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SearchableDropdown : PopupWindowContent
{
    private Item[] items;
    private System.Action<Item> onSelect;
    private string searchText = string.Empty;
    private Vector2 scrollPos;

    public SearchableDropdown(Item[] items, System.Action<Item> onSelect)
    {
        this.items = items;
        this.onSelect = onSelect;
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(300, 400); // Taille du dropdown
    }

    public override void OnGUI(Rect rect)
    {
        // Champ de recherche
        searchText = EditorGUILayout.TextField("Search", searchText);

        // Filtrer les items
        List<Item> filteredItems = new List<Item>();
        foreach (var item in items)
        {
            if (string.IsNullOrEmpty(searchText) || item.name.ToLower().Contains(searchText.ToLower()))
            {
                filteredItems.Add(item);
            }
        }

        // Liste défilante pour afficher les items
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var item in filteredItems)
        {
            EditorGUILayout.BeginHorizontal();

            // Afficher l'icône du sprite à gauche du texte
            if (item.image != null)
            {
                GUILayout.Label(item.image.texture, GUILayout.Width(20), GUILayout.Height(20)); // Afficher le sprite
            }

            // Créer un bouton avec le nom de l'item
            if (GUILayout.Button(item.name, EditorStyles.miniButton))
            {
                onSelect?.Invoke(item); // Appeler l'action de sélection
                EditorApplication.delayCall += () => editorWindow.Close(); // Fermer le popup après sélection
                GUIUtility.ExitGUI(); // Redessiner l'interface utilisateur
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }
}
