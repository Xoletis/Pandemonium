using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPowerUp : MonoBehaviour
{
    public ParticleSystem health;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats.instance.health = PlayerStats.instance.maxHealth;
            if (health != null)
            {
                Instantiate(health, PlayerStats.instance.gameObject.transform);
                Destroy(health, 5);
            }
            Destroy(gameObject);
        }
    }
}
