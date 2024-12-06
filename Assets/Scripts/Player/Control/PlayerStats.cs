using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/Player/Control/PlayerStats")]
public class PlayerStats : MonoBehaviour
{
    // Instance statique pour accéder aux PlayerStats depuis d'autres scripts
    public static PlayerStats instance;

    // La santé actuelle du joueur
    [HideInInspector] public int health;

    // La santé maximale du joueur
    public int maxHealth = 100;

    // Tableau de sliders qui affichent la barre de vie
    public Slider[] Slides;

    // Méthode d'initialisation appelée au démarrage
    private void Awake()
    {
        // Vérifie s'il y a déjà une instance existante de PlayerStats
        if (instance != null)
        {
            Debug.LogError("Il y a plusieurs PlayerStats dans la scene");
        }

        // Assigne cette instance à la variable statique pour un accès global
        instance = this;
    }

    // Méthode Start appelée après Awake() pour initialiser la santé et les barres
    void Start()
    {
        health = maxHealth; // Initialise la santé à la valeur maximale

        // Initialise chaque Slider pour qu'il reflète la santé du joueur
        foreach (var s in Slides)
        {
            s.minValue = 0; // La valeur minimale de la barre est 0
            s.maxValue = PlayerStats.instance.maxHealth; // La valeur maximale est la santé maximale du joueur
            s.value = PlayerStats.instance.health; // Définit la barre de vie initiale selon la santé actuelle
        }
    }

    // Méthode pour appliquer des dégâts au joueur
    public void Damage(int damage)
    {
        health -= damage; // Réduit la santé du joueur en fonction des dégâts reçus

        UpdateBars(); // Met à jour les barres de vie après les dégâts

        // Si la santé du joueur est inférieure ou égale à zéro, le joueur est défait
        if (health <= 0)
        {
            GameManager.instance.Defete(); // Appelle la méthode pour gérer la défaite du joueur
            Destroy(gameObject); // Détruit l'objet du joueur
        }
    }

    // Méthode pour mettre à jour les barres de vie affichées
    public void UpdateBars()
    {
        // Met à jour la valeur de chaque barre de vie
        foreach (var s in Slides)
        {
            s.value = PlayerStats.instance.health;
        }
    }
}
