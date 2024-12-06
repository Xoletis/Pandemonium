using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public PlayerInput input;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject); // Détruire l'objet en double
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        input = GetComponent<PlayerInput>();
    }
}
