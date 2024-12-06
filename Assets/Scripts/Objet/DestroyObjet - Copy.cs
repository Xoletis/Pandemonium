using UnityEngine;
[AddComponentMenu("#Pandemonium/Objet/DestroyObjet")]
public class DestroyObjet : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 3);
    }
}
