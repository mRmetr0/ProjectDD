using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

public class Hero : Entity
{
    [SerializeField] private Sprite playerPortrait;
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform weaponSlot;
    [SerializeField] private bool meleeBaseAnimation;
    //private int _stress;

    public Sprite Portrait => playerPortrait;

    protected override void Awake()
    {
        base.Awake();
        Animator.SetBool("IsRanged", !meleeBaseAnimation);

        if (weapon == null) return;
        Animator.SetBool("IsRanged", weapon.type == Weapon.Type.Ranged);
        weapon = Instantiate(weapon, weaponSlot);
        List<Skill> list = skills.ToList(); 
        list.Add(weapon.skill);
        skills = list.ToArray();
    }

    public void TestResolve()
    {
        if (new Random().Next(0, 1) == 0)
        {
            //MELTDOWN:
            int lowestHealth = (int)(maxHealth * 0.3f);
            Health = Mathf.Min(Health, lowestHealth);
        }
        else
        {
            //RESOLUTE:
            int healthBoost = (int)(maxHealth * 0.2f);
            Health = Mathf.Max(maxHealth, Health += healthBoost);
        }

        //_stress = 0;
    }

    public override void ActTurn()
    {
        Skill[] skillCheck = skills;
        for (int i = 0; i < skillCheck.Length; i++)
        {
            if (i > skillCheck.Length || skillCheck[i] == null) break;

            Skill skill = skillCheck[i];
            //Checks if the character is in the proper position + there is an enemy in the hit range:
            skillCheck[i].usable = skill.InPos(Position) ;//&& skill.CanHit(Skill.ToCheck.Enemies);
        }

        HUDManager.Instance.SetSkillButtons(skillCheck);
    }
}
