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
