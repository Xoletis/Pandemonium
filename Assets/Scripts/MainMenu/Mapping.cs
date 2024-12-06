using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("#Pandemonium/MainMenu/Mapping")]
public class Mapping : MonoBehaviour
{
    //Non fonctionel
    [SerializeField] private InputActionReference jumpAction = null;
    [SerializeField] private TMP_Text bindingDisplayNameText = null;
    [SerializeField] private GameObject startRebindObject = null;
    [SerializeField] private GameObject waitingForInputObject = null;

    public void StartRebinding()
    {
        startRebindObject.SetActive(false);
        waitingForInputObject.SetActive(true);

        
    }
}
