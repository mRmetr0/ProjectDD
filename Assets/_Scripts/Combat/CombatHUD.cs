using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatHUD : MonoBehaviour
{
    public static CombatHUD instance;

    [Header("PLAYER VARIABLES")] 
    [SerializeField] private GameObject PlayerUIHolder;
    [SerializeField] private TMP_Text skillTitle;
    [SerializeField] private GameObject SkillButtonHolder;
    private List<SkillButton> skillButtons = new();
    private TMP_Text roundCounter;
    private SkillButton selectedButton = null;

    public SkillButton SelectedButton => selectedButton;

    private void Awake()
    {
        if (instance is not null)
        {
            Debug.LogError("MORE THEN ONE HUD MANAGER!!!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        foreach (Transform child in SkillButtonHolder.transform)
        {
            SkillButton skillButton = child.gameObject.AddComponent<SkillButton>();
            skillButton.SetUp(this);
            skillButtons.Add(skillButton);
        }
        roundCounter = GetComponentInChildren<TMP_Text>();
    }

    private void OnDestroy()
    {
        if (instance == this) 
            instance = null;
    }

    public void SetPlayerUI(CombatCharacter combatCharacter = null)
    {
        selectedButton = null;
        HoverSkillButton();
        SelectSkillButton(null);
        bool show = combatCharacter is not null;
        PlayerUIHolder.SetActive(show);
        if (!show) return;
        for (int i = 0; i < skillButtons.Count; i++)
        {
            SkillButton button = skillButtons[i];
            //If more buttons then skills, stop
            bool showButton = i <= combatCharacter.Skills.Count - 1;
            button.gameObject.SetActive(showButton);
            if (!showButton) continue;
            //TODO: Check if player can use skill, deactivate if can't
            button.SetSkill(combatCharacter.Skills[i]);
        }
    }

    public void HoverSkillButton(SkillButton button = null)
    {
        if (button is not null)
            skillTitle.text = button.Skill.name;
        else
            skillTitle.text = "";
    }

    public void SelectSkillButton(SkillButton button)
    {
        if (selectedButton == button) selectedButton = null;
        else selectedButton = button;

        foreach (SkillButton child in skillButtons)
        {
            child.SetMarked(child == selectedButton);
        }
    }

    public void UpdateRoundCount(int newCount)
    {
        roundCounter.text = "Round: " + newCount;
    }

    public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private Button button;
        private CombatHUD hud;
        private Skill skill;
        private Image image;
        private Color unmarked = Color.white;
        private Color marked = Color.gray;

        public Skill Skill => skill;
        
        public void SetUp(CombatHUD _hud)
        {
            hud = _hud;
            button = GetComponent<Button>();
            image = GetComponent<Image>();
        }

        public void SetSkill(Skill _skill)
        {
            skill = _skill;
            image.sprite = _skill.logo;
        }

        public void SetMarked(bool isMarked)
        {
            image.color = isMarked ? marked : unmarked;
        }

        public void OnPointerEnter(PointerEventData data)
        {
            hud.HoverSkillButton(this);
        }
        public void OnPointerExit(PointerEventData data)
        {
            hud.HoverSkillButton(null);
        }
        public void OnPointerClick(PointerEventData data)
        {
            hud.SelectSkillButton(this);
        }
    }
}