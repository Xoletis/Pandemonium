using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("#Pandemonium/IA/Rats Stats")]
public class RatsStats : MonoBehaviour
{
    public int health;  // Sant� du rat
    public int damage = 5;  // D�g�ts inflig�s par le rat
    public float attackPerSecond = 1;  // Fr�quence d'attaque par seconde

    private AudioSource BruitCaillouLac;  // Source audio pour les bruits du rat
    public AudioClip BruitCaillou;  // Clip audio pour le bruit du rat lorsqu'il subit des d�g�ts
    public Item sang, peau;  // R�f�rences aux objets "sang" et "peau" que le rat peut laisser tomber
    public GameObject RatCadavre;  // Pr�fabriqu� du cadavre du rat

    private Rigidbody rb;  // R�f�rence au Rigidbody du rat

    private float countdown;  // Timer pour g�rer la fr�quence d'attaque
    private float timer;  // Timer pour incr�menter le temps �coul�

    void Start()
    {
        // Initialisation des variables
        BruitCaillouLac = GetComponent<AudioSource>();  // R�cup�rer la source audio
        rb = GetComponent<Rigidbody>();  // R�cup�rer le Rigidbody du rat

        // Calculer le timer d'attaque en fonction de la fr�quence d'attaque
        countdown = 1f / attackPerSecond;
        timer = 0f;
    }

    void Update()
    {
        // Incr�menter le timer avec le temps �coul� depuis la derni�re frame
        timer += Time.deltaTime;
    }

    // M�thode pour mettre � jour la sant� du rat
    public void UpdateHealth(int amount, Vector3 contact, float forceMagnitude)
    {
        // Calculer la direction oppos�e � partir du contact (appliquer une force dans la direction oppos�e � l'impact)
        Vector3 directionOpposee = transform.position - contact;
        directionOpposee = directionOpposee.normalized;
        directionOpposee.y += 0.5f;  // Ajouter une petite �l�vation sur l'axe Y pour simuler un rebond
        directionOpposee = directionOpposee.normalized;  // Normaliser la direction

        // Appliquer la force au Rigidbody
        rb.AddForce(directionOpposee * forceMagnitude);

        // Mettre � jour la sant� du rat
        health += amount;

        // Jouer un son de bruit lorsque le rat subit des d�g�ts
        BruitCaillouLac.PlayOneShot(BruitCaillou);

        if (health <= 30f)
        {
            GetComponent<RatsIA>().TriggerFlee();
        }

        // V�rifier si la sant� du rat est inf�rieure ou �gale � z�ro, ce qui signifie qu'il est mort
        if (health <= 0)
        {
            // Cr�er un cadavre du rat � la position actuelle
            Instantiate(RatCadavre, transform.position + new Vector3(0, 1, 0), Quaternion.identity);

            int n = Random.Range(0, 7);

            if(n == 1)
            {
                List<GameObject> effet = new List<GameObject>();
                foreach (DiviniteLevel lvl in DiviniteManager.Instance.levelList)
                {
                    if (lvl.level >= lvl.divinite.levels.Count)
                    {
                        effet.Add(lvl.divinite.LastLevelPower);
                    }
                }

                if (effet.Count != 0)
                {
                    int a = Random.Range(0, effet.Count);
                    Instantiate(effet[a], transform.position, Quaternion.identity);
                }
            }
            

            // D�truire l'objet rat (le faire dispara�tre du jeu)
            Destroy(gameObject);
        }
    }

    // M�thode pour g�rer l'attaque du rat
    public void Attack()
    {
        // Si le timer a d�pass� le d�lai entre les attaques (c'est-�-dire que c'est le moment d'attaquer)
        if (timer >= countdown)
        {
            // Infliger des d�g�ts au joueur
            PlayerStats.instance.Damage(damage);
            // R�initialiser le timer en soustrayant la dur�e du d�lai d'attaque
            timer -= countdown;
        }
    }
}
