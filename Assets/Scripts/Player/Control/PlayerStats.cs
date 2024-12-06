using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/Player/Control/PlayerStats")]
public class PlayerStats : MonoBehaviour
{
    // Instance statique pour acc�der aux PlayerStats depuis d'autres scripts
    public static PlayerStats instance;

    // La sant� actuelle du joueur
    [HideInInspector] public int health;

    // La sant� maximale du joueur
    public int maxHealth = 100;

    // Tableau de sliders qui affichent la barre de vie
    public Slider[] Slides;

    // M�thode d'initialisation appel�e au d�marrage
    private void Awake()
    {
        // V�rifie s'il y a d�j� une instance existante de PlayerStats
        if (instance != null)
        {
            Debug.LogError("Il y a plusieurs PlayerStats dans la scene");
        }

        // Assigne cette instance � la variable statique pour un acc�s global
        instance = this;
    }

    // M�thode Start appel�e apr�s Awake() pour initialiser la sant� et les barres
    void Start()
    {
        health = maxHealth; // Initialise la sant� � la valeur maximale

        // Initialise chaque Slider pour qu'il refl�te la sant� du joueur
        foreach (var s in Slides)
        {
            s.minValue = 0; // La valeur minimale de la barre est 0
            s.maxValue = PlayerStats.instance.maxHealth; // La valeur maximale est la sant� maximale du joueur
            s.value = PlayerStats.instance.health; // D�finit la barre de vie initiale selon la sant� actuelle
        }
    }

    // M�thode pour appliquer des d�g�ts au joueur
    public void Damage(int damage)
    {
        health -= damage; // R�duit la sant� du joueur en fonction des d�g�ts re�us

        UpdateBars(); // Met � jour les barres de vie apr�s les d�g�ts

        // Si la sant� du joueur est inf�rieure ou �gale � z�ro, le joueur est d�fait
        if (health <= 0)
        {
            GameManager.instance.Defete(); // Appelle la m�thode pour g�rer la d�faite du joueur
            Destroy(gameObject); // D�truit l'objet du joueur
        }
    }

    // M�thode pour mettre � jour les barres de vie affich�es
    public void UpdateBars()
    {
        // Met � jour la valeur de chaque barre de vie
        foreach (var s in Slides)
        {
            s.value = PlayerStats.instance.health;
        }
    }
}
