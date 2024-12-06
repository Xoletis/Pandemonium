using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("#Pandemonium/GameManager/DiviniteManager")]
public class DiviniteManager : MonoBehaviour
{
    public List<Divinite> allDivinite;

    public static DiviniteManager Instance;

    public List<DiviniteLevel> levelList;

    private void Awake()
    {
        // V�rifie s'il existe d�j� une instance de ItemManager dans la sc�ne
        if (Instance != null)
        {
            Debug.LogError("Il y a plusieurs instances de DiviniteManager dans la sc�ne");  // Message d'erreur si une instance existe d�j�
        }

        Instance = this;  // Assigne cette instance comme l'instance unique du gestionnaire d'items
        Load();
    }

    private void Load()
    {
        // Charge tous les objets de type Item situ�s dans le dossier Resources/Items
        Divinite[] divin = Resources.LoadAll<Divinite>("Divinite");

        // Convertit le tableau d'items en une liste pour une gestion plus facile
        allDivinite = new List<Divinite>(divin);

        // Affiche dans la console le nombre d'items charg�s
        Debug.Log($"Ajout� {allDivinite.Count} items � la liste.");

        foreach (Divinite div in allDivinite)
        {
            DiviniteLevel divlev = new DiviniteLevel(div);
            levelList.Add(divlev);
        }
    }

    public DiviniteLevel GetDiviniteLevlWithDivinity(Divinite divinite)
    {
        foreach (DiviniteLevel divlev in levelList)
        {
            if(divlev.divinite == divinite)
            {
                return divlev;
            }
        }
        return null;
    }
}

[System.Serializable]
public class DiviniteLevel
{
    public Divinite divinite;
    public int level;
    public float percentageLevel;
    public int nbSang;

    public DiviniteLevel(Divinite divinite)
    {
        this.divinite = divinite;
    }
}
