using UnityEngine;

// Classe proxy pour gérer l'interaction des déclencheurs de la zone d'attaque
[AddComponentMenu("#Pandemonium/NePasAjouter/ChildTriggerProxy")]
public class ChildTriggerProxy : MonoBehaviour
{
    public PlayerAttack ParentHandler { get; set; } // Référence au gestionnaire d'attaque du joueur

    private void OnTriggerEnter(Collider other)
    {
        // Lorsque la zone d'attaque entre en collision avec un objet, appelle la méthode de gestion
        ParentHandler?.HandleChildTriggerEnter(other);
    }
}