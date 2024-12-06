using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategiePowerUp : MonoBehaviour
{
    public ParticleSystem strategie;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats.instance.strategienPU();

            if (strategie != null)
            {
                Instantiate(strategie, PlayerStats.instance.gameObject.transform);
                Destroy(strategie, 5);
            }
            Destroy(gameObject);
        }
    }
}
