using UnityEngine;

[System.Serializable]
public class MoreRessources
{
    public Item item;
    [Range(1, 100)]
    public int percentage;
    [Min(1)]
    public int quantity;
}
