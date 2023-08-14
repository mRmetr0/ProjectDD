using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Enemy : Entity
{
    public override void ActTurn()
    {
        Skill skill = SelectSkill();
        HUDManager.Instance.SetText(skill.name);
        StartCoroutine(UseSkill(skill));
    }

    private IEnumerator UseSkill(Skill pSkill)
    {
        yield return new WaitForSeconds(1);
        
        if (pSkill != null)
            pSkill.Use(Skill.ToCheck.Heroes);
        
        StartCoroutine(TurnEnd());
    }

    private Skill SelectSkill()
    { 
        List<Skill> usable = skills.ToList();
        for (int i = usable.Count-1; i >= 0; i--)
        {
            Skill skill = usable[i];
            if (!skill.InPos(Position))
                usable.Remove(skill);
        }

        if (usable.Count == 0) return null;

        Random r = new Random();
        int n = r.Next(0, usable.Count - 1);
        return skills[n];
    }
}
