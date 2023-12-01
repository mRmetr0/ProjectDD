using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hero : Entity
{
    [SerializeField] private Sprite playerPortrait;
    
    //private int _stress;
    private Renderer[] meshMaterials;
    
    public Material[] skins;
    [NonSerialized] public int skinIndex = 0;
    public Sprite Portrait => playerPortrait;
    public HealthBar hpBar => healthBar;

    protected override void Awake()
    {
        base.Awake();
        meshMaterials = GetComponentsInChildren<Renderer>();
    }

    public void TestResolve()
    {
        if (Random.Range(0, 2) == 0)
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

    public bool CanSkinChange()
    {
        return skins.Length > 1;
    }

    public int ChangeSkin()
    {
        if (skins.Length <= 1) return 0;
        skinIndex++;
        if (skinIndex >= skins.Length)
        {
            skinIndex = 0;
        }

        foreach (Renderer r in meshMaterials)
        {
            r.material = skins[skinIndex];
        }

        return skinIndex;
    }

    public void SetSkin(int index)
    {
        if (skins.Length <= 1) return;
        if (skinIndex >= skins.Length)
        {
            skinIndex = 0;
        }
        foreach (Renderer r in meshMaterials)
        {
            r.material = skins[index];
        }
    }
}
