using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObject/Skill")]
public class Skill : ScriptableObject
{
    public string title;
    public int cooldown;

    public StatusEffect[] selfEffects;
    public StatusEffect[] targetEffects;


    [Serializable]
    public class StatusEffect
    {
        public enum enumEffect
        {
            damage,
            
            bleed,
            poison,
            // burn,
            
            stun,
            buff,
            debuff,
            
            move,
            applySynergy
        }

        public enumEffect effect;
        //TODO FIX THIS, make sure that showif works for every single status
        // [ShowIf("effect", enumEffect.damage)] 
        public int damage;
    }
}
