using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;
    
    [SerializeField] private Button[] buttons;
    [SerializeField] private Button switchButton;
    [SerializeField] private TMP_Text title, description;
    private SkillButton[] _skillButtons;
    private Skill _toUse;
    private Canvas _canvas;

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
        switchButton.onClick.AddListener(SwitchCharacterWeapon);

        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        SetText();
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
        Hero hero = BattleManager.CurrentPlayer as Hero;
        switchButton.gameObject.SetActive(hero.CanSwitch());
        
        _canvas.enabled = true;
    }

    public void OnSkillActivate(Skill pSkill)
    {
        //For now, just use the skill, even if it's useless:
        pSkill.Use( Skill.ToCheck.Enemies);
        _canvas.enabled = false;
        StartCoroutine(BattleManager.CurrentPlayer.TurnEnd());
    }
    public void SetText(string pTitle = "", string pDesc = "")
    {
        pTitle = pTitle.Replace("_", " ");
        pDesc = pDesc.Replace("_", " ");
        title.text = pTitle;
        description.text = pDesc;
    }

    private void SwitchCharacterWeapon()
    {
        (BattleManager.CurrentPlayer as Hero)?.SwitchWeapons();
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
