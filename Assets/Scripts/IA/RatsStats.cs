using UnityEngine;

[AddComponentMenu("#Pandemonium/IA/Rats Stats")]
public class RatsStats : MonoBehaviour
{
    public int health;  // Santé du rat
    public int damage = 5;  // Dégâts infligés par le rat
    public float attackPerSecond = 1;  // Fréquence d'attaque par seconde

    private AudioSource BruitCaillouLac;  // Source audio pour les bruits du rat
    public AudioClip BruitCaillou;  // Clip audio pour le bruit du rat lorsqu'il subit des dégâts
    public Item sang, peau;  // Références aux objets "sang" et "peau" que le rat peut laisser tomber
    public GameObject RatCadavre;  // Préfabriqué du cadavre du rat

    private Rigidbody rb;  // Référence au Rigidbody du rat

    private float countdown;  // Timer pour gérer la fréquence d'attaque
    private float timer;  // Timer pour incrémenter le temps écoulé

    void Start()
    {
        // Initialisation des variables
        BruitCaillouLac = GetComponent<AudioSource>();  // Récupérer la source audio
        rb = GetComponent<Rigidbody>();  // Récupérer le Rigidbody du rat

        // Calculer le timer d'attaque en fonction de la fréquence d'attaque
        countdown = 1f / attackPerSecond;
        timer = 0f;
    }

    void Update()
    {
        // Incrémenter le timer avec le temps écoulé depuis la dernière frame
        timer += Time.deltaTime;
    }

    // Méthode pour mettre à jour la santé du rat
    public void UpdateHealth(int amount, Vector3 contact, float forceMagnitude)
    {
        // Calculer la direction opposée à partir du contact (appliquer une force dans la direction opposée à l'impact)
        Vector3 directionOpposee = transform.position - contact;
        directionOpposee = directionOpposee.normalized;
        directionOpposee.y += 0.5f;  // Ajouter une petite élévation sur l'axe Y pour simuler un rebond
        directionOpposee = directionOpposee.normalized;  // Normaliser la direction

        // Appliquer la force au Rigidbody
        rb.AddForce(directionOpposee * forceMagnitude);

        // Mettre à jour la santé du rat
        health += amount;

        // Jouer un son de bruit lorsque le rat subit des dégâts
        BruitCaillouLac.PlayOneShot(BruitCaillou);

        // Vérifier si la santé du rat est inférieure ou égale à zéro, ce qui signifie qu'il est mort
        if (health <= 0)
        {
            // Créer un cadavre du rat à la position actuelle
            Instantiate(RatCadavre, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            // Détruire l'objet rat (le faire disparaître du jeu)
            Destroy(gameObject);
        }
    }

    // Méthode pour gérer l'attaque du rat
    public void Attack()
    {
        // Si le timer a dépassé le délai entre les attaques (c'est-à-dire que c'est le moment d'attaquer)
        if (timer >= countdown)
        {
            // Infliger des dégâts au joueur
            PlayerStats.instance.Damage(damage);
            // Réinitialiser le timer en soustrayant la durée du délai d'attaque
            timer -= countdown;
        }
    }
}
