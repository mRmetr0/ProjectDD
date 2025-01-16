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
    }

    private void OnDestroy()
    {
        if (instance == this) 
            instance = null;
    }

    public void SetPlayerUI(CombatCharacter combatCharacter = null)
    {
        bool show = combatCharacter is null;
        PlayerUIHolder.SetActive(show);
        if (!show)
            return;
        for (int i = skillButtons.Count - 1; i >= 0; i--)
        {
            SkillButton button = skillButtons[i];
            //If more buttons then skills, stop
            bool showButton = i > combatCharacter.Skills.Count - 1;
            button.gameObject.SetActive(showButton);
            if (showButton)
                continue;
        }
    }

    public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Button button;
        private CombatHUD hud;
        private Skill skill;
        private Image image;
        
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

        public void OnPointerEnter(PointerEventData data)
        {
            
        }
        public void OnPointerExit(PointerEventData data)
        {
            
        }
    }
}