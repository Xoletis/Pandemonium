using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Spawner))]
public class SpawnZoneEditor : Editor
{
    private void OnSceneGUI()
    {
        // R�f�rence au script associ�
        Spawner spawnZone = (Spawner)target;

        // D�termine le centre de la zone avec l'offset
        Vector3 center = spawnZone.transform.position;
        Vector3 size = new Vector3(spawnZone.areaSize.x, 0, spawnZone.areaSize.y);

        // D�place le centre avec un Handle
        EditorGUI.BeginChangeCheck();
        Vector3 newCenter = Handles.PositionHandle(center, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spawnZone, "Move Spawn Zone");
            Vector3 offset = newCenter - spawnZone.transform.position;
        }

        // Manipule les coins pour ajuster la taille
        Vector3[] corners = new Vector3[4];
        corners[0] = center + new Vector3(-size.x / 2, 0, -size.z / 2); // Bas-gauche
        corners[1] = center + new Vector3(-size.x / 2, 0, size.z / 2);  // Haut-gauche
        corners[2] = center + new Vector3(size.x / 2, 0, size.z / 2);   // Haut-droit
        corners[3] = center + new Vector3(size.x / 2, 0, -size.z / 2);  // Bas-droit

        for (int i = 0; i < corners.Length; i++)
        {
            EditorGUI.BeginChangeCheck();
            var fmh_36_68_638689019579598823 = Quaternion.identity; Vector3 newCorner = Handles.FreeMoveHandle(corners[i], 0.5f, Vector3.zero, Handles.SphereHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spawnZone, "Resize Spawn Zone");

                // Met � jour la taille en fonction du coin d�plac�
                Vector3 delta = newCorner - center;
                spawnZone.areaSize = new Vector2(
                    Mathf.Abs(delta.x * 2), // Double la distance pour obtenir la taille compl�te
                    Mathf.Abs(delta.z * 2)
                );
                break;
            }
        }

        // Dessine les lignes de la zone
        Handles.color = Color.green;
        Handles.DrawLine(corners[0], corners[1]);
        Handles.DrawLine(corners[1], corners[2]);
        Handles.DrawLine(corners[2], corners[3]);
        Handles.DrawLine(corners[3], corners[0]);
    }

    public override void OnInspectorGUI()
    {
        // Référence au script Spawner
        Spawner spawner = (Spawner)target;

        serializedObject.Update();

        EditorUtils utils = new EditorUtils(serializedObject);

        // Liste des spawns
        EditorGUILayout.LabelField("Monster Spawns", EditorStyles.boldLabel);

        if (spawner.spawns == null)
            spawner.spawns = new List<MonsterSpawn>();

        int totalPercentage = 0;

        for (int i = 0; i < spawner.spawns.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            // Étiquette pour chaque entrée
            EditorGUILayout.LabelField($"Spawn {i + 1}", EditorStyles.boldLabel);

            // Champ pour l'objet GameObject
            spawner.spawns[i].monster = (GameObject)EditorGUILayout.ObjectField("Monster", spawner.spawns[i].monster, typeof(GameObject), true);

            // Champ modifiable pour le pourcentage
            spawner.spawns[i].percentage = EditorGUILayout.IntSlider("Percentage", spawner.spawns[i].percentage, 0, 100);
            totalPercentage += spawner.spawns[i].percentage;

            // Double slider pour Min et Max
            float min = spawner.spawns[i].min;
            float max = spawner.spawns[i].max;
            EditorGUILayout.MinMaxSlider("Min/Max", ref min, ref max, 0, 10);

            spawner.spawns[i].min = Mathf.RoundToInt(min);
            spawner.spawns[i].max = Mathf.RoundToInt(max);

            // Affichage des valeurs Min et Max
            EditorGUILayout.LabelField($"Min: {spawner.spawns[i].min}, Max: {spawner.spawns[i].max}");

            // Bouton pour supprimer cette entrée
            if (GUILayout.Button("Remove Spawn"))
            {
                spawner.spawns.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        // Bouton pour ajouter une nouvelle entrée
        if (GUILayout.Button("Add Spawn"))
        {
            spawner.spawns.Add(new MonsterSpawn());
        }

        // Vérification si le total des pourcentages n'est pas 100
        if (totalPercentage != 100)
        {
            EditorGUILayout.HelpBox($"Total Percentage is {totalPercentage}%. Please adjust to reach 100%.", MessageType.Info);
        }

        // Appliquer les changements si nécessaires
        if (GUI.changed)
        {
            EditorUtility.SetDirty(spawner);
        }

        utils.AddProperty(new string[] { "secondsBetweenSpawns", "ActivateVisual" });

        serializedObject.ApplyModifiedProperties();
    }
}
