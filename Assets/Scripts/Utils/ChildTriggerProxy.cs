using UnityEngine;

// Classe proxy pour g�rer l'interaction des d�clencheurs de la zone d'attaque
[AddComponentMenu("#Pandemonium/NePasAjouter/ChildTriggerProxy")]
public class ChildTriggerProxy : MonoBehaviour
{
    public PlayerAttack ParentHandler { get; set; } // R�f�rence au gestionnaire d'attaque du joueur

    private void OnTriggerEnter(Collider other)
    {
        // Lorsque la zone d'attaque entre en collision avec un objet, appelle la m�thode de gestion
        ParentHandler?.HandleChildTriggerEnter(other);
    }
}