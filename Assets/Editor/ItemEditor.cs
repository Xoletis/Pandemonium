using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    private bool Generale = true;
    private bool Other = true;
    private bool Craft = true;

    public override void OnInspectorGUI()
    {
        Item item = target as Item;
        EditorUtils utils = new EditorUtils(serializedObject);

        serializedObject.Update();
        Generale = utils.AddClosableLabel("Général", Generale);
        if (Generale) utils.AddProperty(new string[] { "Nom", "image", "objet", "maxStack" });

        utils.DrawHorizontalLine();

        Craft = utils.AddClosableLabel("Craft", Craft);
        if (Craft)
        {
            utils.AddProperty(new string[] { "isCraftable" });
            if (item.isCraftable)
            {
                utils.AddProperty(new string[] { "craftMethod", "timeToCraft", "craftQuantity" });
                SerializedProperty materialsProp = serializedObject.FindProperty("materials");
                utils.DrawCraftList(materialsProp);
            }
        }

        utils.DrawHorizontalLine();

        Other = utils.AddClosableLabel("Autre", Other);
        if (Other) utils.AddProperty(new string[] { "rarity" });

        

        serializedObject.ApplyModifiedProperties();
    }
}
