using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Pandemonium/Item", order = 1)]
public class Item : ScriptableObject
{
    public string Nom;
    public Sprite image;
    public int maxStack = 1;
    public GameObject objet;
    public Rarity rarity;

    public bool isCraftable;
    public CraftType craftMethod;
    public List<CraftMaterial> materials;
    public float timeToCraft;
    public int craftQuantity = 1;
}