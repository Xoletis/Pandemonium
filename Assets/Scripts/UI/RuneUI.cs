using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[AddComponentMenu("#Pandemonium/UI/RuneUI")]
public class RuneUI : MonoBehaviour
{
    public GameObject ui, divinitylvlPrefab, content;

    private InputForPlayer _playerInput; // Entrée du joueur

    public TextMeshProUGUI nom, level, sangNb;
    public Image fond, progressBar;
    public Slider sangSlider;
    public Button prierBTN;

    public Item ressources;

    bool canQuit = true;

    Divinite divinite;

    int SangValue;

    List<GameObject> lvlObject = new List<GameObject>();

    private void Awake()
    {
        _playerInput = new InputForPlayer();

        // Lier l'action Escape à la fonction Quit
        _playerInput.Player.Escape.performed += Quit;
    }

    // Active les contrôles quand l'UI est activée
    private void OnEnable()
    {
        _playerInput.Enable();
    }

    // Désactive les contrôles quand l'UI est désactivée
    private void OnDisable()
    {
        _playerInput.Disable();
    }

    private void Quit(InputAction.CallbackContext context)
    {
        if (!canQuit) return; // Empêche de quitter si la fabrication est en cours
        ui.SetActive(false); // Désactive l'UI
        Cursor.lockState = CursorLockMode.Locked; // Verrouille le curseur
        Cursor.visible = false; // Cache le curseur
    }

    private void Start()
    {
        ui.SetActive(false);
    }

    public void Initialize(Divinite divin)
    {
        foreach (GameObject lvlObject in lvlObject)
        {
            Destroy(lvlObject);
        }
        lvlObject = new List<GameObject>();

        SangValue = 0;
        divinite = divin;
        DiviniteLevel lvl = DiviniteManager.Instance.GetDiviniteLevlWithDivinity(divinite);
        ui.SetActive(true);
        Cursor.lockState = CursorLockMode.None; // Déverrouille le curseur
        Cursor.visible = true; // Affiche le curseur

        nom.text = divinite.Nom;
        level.text = lvl.level + "";
        fond.sprite = divinite.image;
        progressBar.sprite = divinite.image;
        progressBar.fillAmount = lvl.percentageLevel/100;
        int qantRessource = InventoryManager.instance.GetNbItemInInventory(ressources);
        if(qantRessource > 0)
        {
            sangSlider.gameObject.SetActive(true);
            sangSlider.minValue = 0;
            sangSlider.maxValue = qantRessource;
            sangSlider.value = 0;
        }
        else
        {
            sangSlider.gameObject.SetActive(false);
        }

        sangNb.text = "0";
        prierBTN.interactable = false;

        foreach (Level level in divin.levels)
        {
            GameObject lvlObj = Instantiate(divinitylvlPrefab, content.transform);
            lvlObject.Add(lvlObj);
            DivinityLvl divinityLvl = lvlObj.GetComponent<DivinityLvl>();
            divinityLvl.lvlText.text = "Niv." + level.level ;
            divinityLvl.descText.text = level.Desc;
            if(lvl.level > level.level - 1)
            {
               divinityLvl.foreground.fillAmount = 1;
            }
            else if (lvl.level == level.level - 1)
            {
                divinityLvl.foreground.fillAmount = lvl.percentageLevel / 100;
            }
            else
            {
                divinityLvl.foreground.fillAmount = 0;
            }
        }


        if (lvl.level >= divinite.levels.Count)
        {
            prierBTN.interactable = false;
            sangSlider.interactable = false;
        }
    }

    public void ModifieSlider(float value)
    {
        sangNb.text = value + "";
        SangValue = (int)value;
        prierBTN.interactable = value > 0;
    }

    public void Prier()
    {
        StartCoroutine(GvieBlood());
    }

    IEnumerator GvieBlood()
    {
        DiviniteLevel lvl = DiviniteManager.Instance.GetDiviniteLevlWithDivinity(divinite);

        canQuit = false;
        sangSlider.interactable = false;
        prierBTN.interactable = false;

        while (SangValue > 0)
        {
            yield return new WaitForSeconds(0.1f);
            SangValue--;
            sangNb.text = SangValue + "";
            sangSlider.value = SangValue;
            lvl.nbSang++;
            lvl.percentageLevel = (lvl.nbSang * 100) / divinite.levels[lvl.level].NbSang;
            progressBar.fillAmount = lvl.percentageLevel / 100;
            lvlObject[lvl.level].GetComponent<DivinityLvl>().foreground.fillAmount = lvl.percentageLevel / 100;
            InventoryManager.instance.RemoveItem(ressources, 1);
            if (lvl.nbSang >= divinite.levels[lvl.level].NbSang)
            {
                lvl.level++;
                lvl.nbSang = 0;
                lvl.percentageLevel = 0;
                level.text = lvl.level + "";
                switch (divinite.levels[lvl.level - 1].type)
                {
                    case AugmentationType.Damage: PlayerStats.instance.Puissance += (int)divinite.levels[lvl.level - 1].Amount; break;
                    case AugmentationType.Health: PlayerStats.instance.maxHealth += (int)divinite.levels[lvl.level - 1].Amount; PlayerStats.instance.UpdateBars(); break;
                    case AugmentationType.Strat: PlayerStats.instance.Strategie += divinite.levels[lvl.level - 1].Amount; break;
                    case AugmentationType.Protection: PlayerStats.instance.Protection += (int)divinite.levels[lvl.level - 1].Amount; break;
                    default: break;
                }
                if(lvl.level >= divinite.levels.Count)
                {
                    SangValue = 0;
                }
            }

        }
        canQuit = true;
        sangSlider.interactable = true;
        prierBTN.interactable = true;
        Initialize(divinite);
    }
}
