using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class Hero : Entity
{
    [SerializeField] private Weapon primary, secondary;
    
    [Header("Weapon Slot Positions on Model")] 
    [SerializeField] private Transform primarySlot;
    [SerializeField] private Transform secondarySlot;
    [SerializeField] private Transform equipSlot;
    
    private Weapon equipedWeapon;
    private int _stress;

    private void Awake()
    {
        if (secondary != null && secondary.slot == Weapon.Slot.Primary)
        {
            secondary = null;
            Debug.LogWarning("NO PRIMARY IN SECONDARY SLOT");
        }

        if (primary != null)
        {
            primary = Instantiate(primary, equipSlot);
            equipedWeapon = primary;

            if (secondary != null)
                secondary = Instantiate(secondary, secondarySlot);
        } else if (secondary != null)
        {
            secondary = Instantiate(secondary, equipSlot);
            equipedWeapon = secondary;
        }
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

        _stress = 0;
        //TODO: GET QUIRK BASED ON RESOLVE
    }

    public override void ActTurn()
    {
        List<Skill> skillList = new List<Skill>();
        foreach (var skill in skills) skillList.Add(skill);
        if (equipedWeapon != null) skillList.Add(equipedWeapon.skill);
        Skill[] skillCheck = skillList.ToArray();
        
        for (int i = 0; i < skillCheck.Length; i++)
        {
            if (i > skillCheck.Length || skillCheck[i] == null) break;

            Skill skill = skillCheck[i];
            //Checks if the character is in the proper position + there is an enemy in the hit range:
            skillCheck[i].usable = skill.InPos(Position) ;//&& skill.CanHit(Skill.ToCheck.Enemies);
        }

        HUDManager.Instance.SetSkillButtons(skillCheck);
    }

    public void SwitchWeapons()
    {
        if (equipedWeapon == primary)
        {
            equipedWeapon = secondary;
            primary.transform.parent = primarySlot;
            secondary.transform.parent = equipSlot;
        }
        else
        {
            equipedWeapon = primary;
            primary.transform.parent = equipSlot;
            secondary.transform.parent = secondarySlot;
        }
        Animator.SetBool("IsPrimary", equipedWeapon.slot == Weapon.Slot.Primary);
        ActTurn();
    }

    public bool CanSwitch()
    {
        return (primary != null && secondary != null);
    }
}
