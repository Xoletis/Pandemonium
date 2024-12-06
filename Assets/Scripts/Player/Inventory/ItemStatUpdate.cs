using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/Player/Inventory/ItemStatUpdate")]
public class ItemStatUpdate : MonoBehaviour
{
    // Références aux éléments d'interface utilisateur (UI)
    public TextMeshProUGUI Nom;         // Nom de l'item
    public TextMeshProUGUI Stack;       // Taille maximale de la pile (stack)
    public Image image;                 // Image de l'item

    [Header("Arme")]
    public Sprite[] PoidImages;         // Images pour indiquer le poids des armes (Lourde, Moyenne, Légère)
    public Image Poid;                  // Image affichant le poids de l'arme
    public GameObject OtherStat;        // Conteneur pour les autres statistiques des armes
    public Image ArmeType;              // Image pour afficher le type d'arme (à distance ou non)
    public Sprite[] armeTypeImage;      // Sprites pour distinguer les types d'armes
    public TextMeshProUGUI damage;      // Texte pour afficher les dégâts de l'arme
    public TextMeshProUGUI knockback;   // Texte pour afficher la force de recul (knockback)

    [Header("Arme à distance")]
    public GameObject[] Ranges;         // Objets pour afficher les statistiques des armes à distance
    public TextMeshProUGUI range;       // Texte pour afficher la portée des armes à distance

    // Met à jour les statistiques affichées pour un objet donné.
    public void UpdateStats(Item item)
    {
        // Met à jour le nom, l'image et la taille de pile maximale
        Nom.text = item.Nom;
        image.sprite = item.image;
        Stack.text = item.maxStack.ToString();

        // Si l'item est une arme
        if (item as WeaponItem)
        {
            // Active les éléments liés aux armes
            Poid.gameObject.SetActive(true);
            OtherStat.SetActive(true);

            // Caste l'item en `WeaponItem`
            WeaponItem weapon = (WeaponItem)item;

            // Met à jour le type d'arme (à distance ou non)
            ArmeType.sprite = weapon.isRangeWeapon ? armeTypeImage[0] : armeTypeImage[1];

            // Affiche les dégâts et la force de recul
            damage.text = weapon.damage.ToString();
            knockback.text = (weapon.forceMagnitude / 10) + "%";

            // Si l'arme est à distance, active les éléments correspondants et met à jour la portée
            if (weapon.isRangeWeapon)
            {
                RangeActive(true);
                range.text = weapon.range + " m";
            }
            else
            {
                RangeActive(false);
            }

            // Change l'image du poids selon le type de l'arme
            switch (weapon.type)
            {
                case WeaponType.Lourde:
                    Poid.sprite = PoidImages[0];
                    break;
                case WeaponType.Moyenne:
                    Poid.sprite = PoidImages[1];
                    break;
                case WeaponType.Legere:
                    Poid.sprite = PoidImages[2];
                    break;
            }
        }
        else
        {
            // Si ce n'est pas une arme, désactive les éléments liés aux armes
            Poid.gameObject.SetActive(false);
            OtherStat.SetActive(false);
            RangeActive(false);
        }
    }

    // Active ou désactive les éléments liés à la portée des armes à distance.
    void RangeActive(bool active)
    {
        // Parcourt tous les éléments liés aux armes à distance et ajuste leur visibilité
        foreach (GameObject item in Ranges)
        {
            item.SetActive(active);
        }
    }
}
