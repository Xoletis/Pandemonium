using System.IO.Ports;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponItem))]
public class WeaponItemEditor : Editor
{
    private bool Generale = true;
    private bool Range = true;
    private bool Other = true;
    private bool RangeEffect = false;
    private bool Craft = true;

    public override void OnInspectorGUI()
    {
        WeaponItem weaponItem = target as WeaponItem;
        EditorUtils utils = new EditorUtils(serializedObject);

        serializedObject.Update();

        Generale = utils.AddClosableLabel("Général", Generale);
        if(Generale) utils.AddProperty(new string[] { "Nom", "image", "objet", "damage", "forceMagnitude" });

        utils.DrawHorizontalLine();

        Range = utils.AddClosableLabel("Range", Range);
        if(Range)
        {
            utils.AddProperty(new string[] { "isRangeWeapon" });
            EditorGUI.indentLevel++;
            utils.AddProperty(new string[] { "range", "ammo", "magazineSize" }, true, "", weaponItem.isRangeWeapon);
            utils.AddProperty(new string[] { "asBulletSpread" }, true, "", weaponItem.isRangeWeapon);
            EditorGUI.indentLevel++;
            utils.AddProperty(new string[] { "BulletSpreadVariance", }, true, "", weaponItem.asBulletSpread && weaponItem.isRangeWeapon);
            EditorGUI.indentLevel--;
            utils.AddProperty(new string[] { "isAutomaticWeapon", "ShootDelay" }, true, "", weaponItem.isRangeWeapon);
            RangeEffect = utils.AddClosableLabel("Visual Effect", RangeEffect);
            if (RangeEffect) {
                utils.AddProperty(new string[] { "ImpactParticuleSystem", "BulletTrail", "bulletSpeed" });
            }
        }

        utils.DrawHorizontalLine();

        Craft = utils.AddClosableLabel("Craft", Craft);
        if (Craft)
        {
            utils.AddProperty(new string[] { "isCraftable" });
            if (weaponItem.isCraftable)
            {
                utils.AddProperty(new string[] { "craftMethod", "timeToCraft", "craftQuantity" });
                SerializedProperty materialsProp = serializedObject.FindProperty("materials");
                utils.DrawCraftList(materialsProp);
            }
        }

        utils.DrawHorizontalLine();

        Other = utils.AddClosableLabel("Autre", Other);
        if (Other) {

            utils.AddProperty(new string[] { "rarity", "type" });
            utils.AddProperty(new string[] { "maxStack" }, false, "Pour une arme le stack est obligatoirement à 1");

        }


        serializedObject.ApplyModifiedProperties();
    }

}