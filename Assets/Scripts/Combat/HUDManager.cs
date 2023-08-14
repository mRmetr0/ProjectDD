using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class HUDManager : MonoBehaviour
{
    public enum UIType
    {
        Combat,
        Win,
        Lose,
        None
    }

    public static HUDManager Instance;
    
    [SerializeField] private Button[] buttons;
    [SerializeField] private TMP_Text title, description;
    [SerializeField] private GameObject combatUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;
    
    private SkillButton[] _skillButtons;
    private Skill _toUse;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("MORE THEN ONE HUDMANAGER???!?!!?");
            return;
        }
        Instance = this;

        _skillButtons = new SkillButton[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];
            SkillButton skillButton = button.AddComponent<SkillButton>();
            skillButton.Initialize(button, this);
            _skillButtons[i] = skillButton;
            try
            {
                TMP_Text text = button.GetComponentInChildren<TMP_Text>();
                Destroy(text);
            }
            catch (Exception e){
                Debug.Log($"failed :(, Exception: {e}");
            }
        }
        
        SetText();
        SwitchUI(UIType.None);
        winUI.GetComponentInChildren<Button>().onClick.AddListener(WinButton);
        loseUI.GetComponentInChildren<Button>().onClick.AddListener(LoseButton);
    }
    
    public void SetSkillButtons(Skill[] pSkills)
    {
        for (int i = 0; i < _skillButtons.Length; i++)
        {
            if (i + 1 > pSkills.Length)
            {
                _skillButtons[i].Assign(null);
                continue;
            }

            _skillButtons[i].Assign(pSkills[i]);
        }
        SwitchUI(UIType.Combat);
    }

    public void OnSkillActivate(Skill pSkill)
    {
        //For now, just use the skill, even if it's useless:
        pSkill.Use( Skill.ToCheck.Enemies);
        SwitchUI(UIType.None);
        StartCoroutine(BattleManager.CurrentPlayer.TurnEnd());
    }
    public void SetText(string pTitle = "", string pDesc = "")
    {
        pTitle = pTitle.Replace("_", " ");
        pDesc = pDesc.Replace("_", " ");
        title.text = pTitle;
        description.text = pDesc;
        title.ForceMeshUpdate(true);
    }

    public void SwitchUI(UIType type)
    {
        combatUI.gameObject.SetActive(type == UIType.Combat);
        winUI.gameObject.SetActive(type == UIType.Win);
        loseUI.gameObject.SetActive(type == UIType.Lose);
    }

    public void WinButton()
    {
        SceneManager.LoadScene("TravelScene");
    }

    public void LoseButton()
    {
        Application.Quit();
    }
}

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private HUDManager _manager;
    private Button _button;
    private Image _buttonImage;
    private Skill _skill;
    
    public void Initialize(Button pButton, HUDManager pManager)
    {
        _manager = pManager;
        _button = pButton;
        _buttonImage = pButton.GetComponent<Image>();
        
        pButton.onClick.RemoveAllListeners();
        pButton.onClick.AddListener(OnButtonPress);
    }

    public void Assign(Skill pSkill)
    {
        bool isNull = (pSkill == null);
        _button.gameObject.SetActive(!isNull);
        if (isNull) return;

        _buttonImage.sprite = pSkill.sprite;
        _button.interactable = pSkill.usable;
        _skill = pSkill;
    }
    private void OnButtonPress()
    {
        _manager.OnSkillActivate(_skill);
        _manager.SetText();
    }

    public void OnPointerEnter(PointerEventData Event)
    {
        _manager.SetText(_skill.name, _skill.description);
    }
    public void OnPointerExit(PointerEventData Event)
    {
        _manager.SetText();
    }
}
