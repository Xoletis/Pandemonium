using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/UI/ItemField")]
public class ItemField : MonoBehaviour
{
    // R�f�rence � l'image de l'objet � afficher dans l'UI
    public Image image;

    // R�f�rence au texte qui affichera le nom et la quantit� de l'objet
    public TextMeshProUGUI text;

    // M�thode qui permet de cr�er un champ d'objet avec son image et son nombre
    // Param�tres : un objet (Item) et un nombre (nb)
    public void Create(Item item, int nb)
    {
        // Affectation de l'image de l'objet � l'image du UI
        image.sprite = item.image;

        // Mise � jour du texte pour afficher le nom de l'objet et la quantit�
        text.text = item.Nom + " (x" + nb + ")";
    }

    // M�thode appel�e pour d�truire cet objet lorsque l'animation est termin�e
    public void EndAnim()
    {
        // D�truit l'objet actuel (le GameObject auquel ce script est attach�)
        Destroy(gameObject);
    }
}
