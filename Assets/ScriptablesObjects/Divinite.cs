using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Pandemonium/Divinite", order = 1)]
public class Divinite : ScriptableObject
{
    public string Nom;
    public Sprite image;
    public List<Level> levels;
    public GameObject LastLevelPower;
}

[System.Serializable]
public class Level
{
    public int level;
    public int NbSang;
    public string Desc;
    public AugmentationType type;
    public float Amount;
}

public enum AugmentationType
{
    Damage,
    Health,
    Strat,
    Protection,
    None
}