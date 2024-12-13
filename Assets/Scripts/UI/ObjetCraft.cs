using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/UI/ObjetCraft")]
public class ObjetCraft : MonoBehaviour
{
    public Image img;
    public TextMeshProUGUI txt;
    public int id;
    public Etablie parent;

    public void Click()
    {
        parent.SendMessage("Selected", id);
    }
}
