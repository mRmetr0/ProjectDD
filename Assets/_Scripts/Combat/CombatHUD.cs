using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatHUD : MonoBehaviour
{
    [Header("PLAYER VARIABLES")] 
    [SerializeField] private TMP_Text skillTitle;
    [SerializeField] private GameObject skillButtonHolder;

    public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Button button;
        private CombatHUD hud;
        private Skill skill;

        public void SetUp(CombatHUD _hud)
        {
            hud = _hud;
            button = GetComponent<Button>();
        }

        public void SetSkill(Skill _skill)
        {
            skill = _skill;
        }

        public void OnPointerEnter(PointerEventData data)
        {
            
        }
        public void OnPointerExit(PointerEventData data)
        {
            
        }
    }
}