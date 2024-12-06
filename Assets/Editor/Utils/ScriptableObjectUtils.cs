using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ScriptableObjectUtils
{
    public static List<Item> GetAllItems()
    {
        List<Item> items = new List<Item>();

        string folderPath = "Assets/Resources/Items";
        string[] guids = AssetDatabase.FindAssets("t:Resources/Item", new[] { folderPath });
        Debug.Log(guids);
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Item item = AssetDatabase.LoadAssetAtPath<Item>(assetPath);
            if (item != null)
            {
                items.Add(item);
            }
            else
            {
                Debug.LogWarning($"Asset at path {assetPath} is not an Item.");
            }
        }

        return items;
    }

}
