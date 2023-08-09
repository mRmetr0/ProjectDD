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
    [SerializeField] private TMP_Text title, description;
    private SkillButton[] _skills;
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

        _skills = new SkillButton[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];
            SkillButton skillButton = button.AddComponent<SkillButton>();
            skillButton.Initialize(button, this);
            _skills[i] = skillButton;
        }

        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        SetText();
    }

    public void SetSkillButtons(Skill[] pSkills)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            _skills[i].Assign(pSkills[i]);
        }

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
        HUDManager.Instance.SetText();
    }

    public void OnPointerEnter(PointerEventData Event)
    {
        HUDManager.Instance.SetText(_skill.name, _skill.description);
    }
    public void OnPointerExit(PointerEventData Event)
    {
        HUDManager.Instance.SetText();
    }
}
