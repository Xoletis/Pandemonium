using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/UI/ItemField")]
public class ItemField : MonoBehaviour
{
    // Référence à l'image de l'objet à afficher dans l'UI
    public Image image;

    // Référence au texte qui affichera le nom et la quantité de l'objet
    public TextMeshProUGUI text;

    // Méthode qui permet de créer un champ d'objet avec son image et son nombre
    // Paramètres : un objet (Item) et un nombre (nb)
    public void Create(Item item, int nb)
    {
        // Affectation de l'image de l'objet à l'image du UI
        image.sprite = item.image;

        // Mise à jour du texte pour afficher le nom de l'objet et la quantité
        text.text = item.Nom + " (x" + nb + ")";
    }

    // Méthode appelée pour détruire cet objet lorsque l'animation est terminée
    public void EndAnim()
    {
        // Détruit l'objet actuel (le GameObject auquel ce script est attaché)
        Destroy(gameObject);
    }
}
