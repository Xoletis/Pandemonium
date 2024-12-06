using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{
    // Tableau de Sliders repr�sentant les barres de sant�. Cela permet d'afficher plusieurs barres si n�cessaire (ex: sant�, bouclier, etc.).
    public Slider[] Slides;

    // Cette m�thode est appel�e au d�but du jeu, ou lorsque l'objet est activ�.
    void Start()
    {
        // Boucle � travers chaque Slider dans le tableau Slides
        foreach (var s in Slides)
        {
            // Initialise la valeur minimale de la barre de sant� � 0 (la sant� ne peut pas �tre inf�rieure � 0)
            s.minValue = 0;

            // D�finit la valeur maximale de la barre de sant� sur la sant� maximale du joueur, obtenue via le singleton PlayerStats
            s.maxValue = PlayerStats.instance.maxHealth;

            // D�finit la valeur actuelle de la barre de sant� sur la sant� actuelle du joueur
            s.value = PlayerStats.instance.health;
        }
    }
}
