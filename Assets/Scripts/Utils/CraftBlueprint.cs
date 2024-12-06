[System.Serializable]
public class CraftBlueprint
{
    public Item item;
    public bool craftUnlock;

    public CraftBlueprint(Item item, bool craftUnlock)
    {
        this.item = item;
        this.craftUnlock = craftUnlock;
    }
}
