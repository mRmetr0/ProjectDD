using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObject/Skill")]
public class Skill : ScriptableObject
{
    public enum Targets
    {
        opponent,
        team,
        self
    }

    public string title;
    [ShowAssetPreview] public Sprite logo;
    public int cooldown;
    private int cooldownCounter = 0;
    public Targets targets;
    [SerializeField] private bool cleave;

    [Header ("SELF VARIABLES")] 
    [SerializeField]
    private bool[] usableRange = new[] { false, false, false, false};
    public StatusEffect[] selfEffects;
    [Header("TARGET VARIABLES")]
    [SerializeField]
    private bool[] targetRange = new[] { false, false, false, false};
    public StatusEffect[] targetEffects;

    public void UseSkill(CombatEntity user, CombatEntity target)
    {
        foreach(StatusEffect effect in targetEffects)
        {
            effect.ApplyEffect(target);
        }
        foreach(StatusEffect effect in selfEffects)
        {
            effect.ApplyEffect(user);
        }
    }

    public bool CanUseSkill(int userPosition)
    {
        return usableRange[userPosition];
    }

    public bool CanHitPos(int targetPos)
    {
        return targetRange[targetPos];
    }

    // public bool CanHitPos(CombatManager.Side side)
    // {
    //     List<CombatEntity> toCheck = null;
    //     CombatManager.Side opposite = side == CombatManager.Side.Heroes ? CombatManager.Side.Enemies : CombatManager.Side.Heroes;
    //     switch (targets)
    //     {
    //         case Targets.opponent:
    //             toCheck = CombatManager.instance.GetCombatSide(opposite);
    //             break;
    //         case Targets.team:
    //             toCheck  = CombatManager.instance.GetCombatSide(side);
    //             break;
    //     }
    //     if (toCheck == null) return false;
    //     foreach(CombatEntity entity in toCheck)
    //     {
    //         if (targetRange[entity.Position]) return true;
    //     }
    //     return false;
    // }

    [Serializable]
    public class StatusEffect
    {
        public enum enumEffect
        {
            damage,
            heal,

            bleed,
            // poison,
            // burn,
            
            stun,
            buff,
            debuff,
            
            move,
            applySynergy
        }

        public enumEffect effect;
        
        //Variables
        [ShowIf("effect", enumEffect.damage)] [AllowNesting]
        public int rawDamage; //raw damage done
        [ShowIf("effect", enumEffect.bleed)] [AllowNesting]
        public int dotDamage, duration; //DOT values
        [ShowIf("effect", enumEffect.move)] [AllowNesting]
        public int direction;

        public void ApplyEffect(CombatEntity target)
        {
            switch (effect)
            {
                case (enumEffect.damage):
                    target.TakeDamage(rawDamage);
                    break;
                case (enumEffect.bleed):
                    target.AddEffect(new BleedEffect(target, duration, dotDamage));
                    break;
                case (enumEffect.move):
                    CombatManager.instance.MovePerson(target, -direction);
                    break;
            }
        }
    }
}
