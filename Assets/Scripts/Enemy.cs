using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Enemy : Entity
{
    public override void ActTurn()
    {
        StartCoroutine(UseSkill());
    }

    private IEnumerator UseSkill()
    {
        yield return new WaitForSeconds(1);
        ActSkill();
        StartCoroutine(TurnEnd());
    }

    private void ActSkill()
    { 
        List<Skill> usable = skills.ToList();
        for (int i = usable.Count-1; i >= 0; i--)
        {
            Skill skill = usable[i];
            if (!skill.InPos(Position))
                usable.Remove(skill);
        }

        if (usable.Count == 0) return;

        Random r = new System.Random();
        int n = r.Next(0, usable.Count - 1);
        Skill toUSe = skills[n];
        toUSe.Use(Skill.ToCheck.Heroes);
    }
}
