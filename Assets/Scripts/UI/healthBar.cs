using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{
    // Tableau de Sliders représentant les barres de santé. Cela permet d'afficher plusieurs barres si nécessaire (ex: santé, bouclier, etc.).
    public Slider[] Slides;

    // Cette méthode est appelée au début du jeu, ou lorsque l'objet est activé.
    void Start()
    {
        // Boucle à travers chaque Slider dans le tableau Slides
        foreach (var s in Slides)
        {
            // Initialise la valeur minimale de la barre de santé à 0 (la santé ne peut pas être inférieure à 0)
            s.minValue = 0;

            // Définit la valeur maximale de la barre de santé sur la santé maximale du joueur, obtenue via le singleton PlayerStats
            s.maxValue = PlayerStats.instance.maxHealth;

            // Définit la valeur actuelle de la barre de santé sur la santé actuelle du joueur
            s.value = PlayerStats.instance.health;
        }
    }
}
