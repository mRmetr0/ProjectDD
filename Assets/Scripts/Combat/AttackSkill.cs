using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackSkill", menuName = "ScriptableObjects/Skills/AttackSkill")]
public class AttackSkill : Skill
{
    [Header("Effects To Enemy")]
    public bool bleeds;
    public bool stuns;
    public bool marks;

    public override void Use()
    {
        ToCheck toCheck = BattleManager.Instance.Heroes.Contains(BattleManager.CurrentPlayer)
            ? ToCheck.Enemies
            : ToCheck.Heroes;
        Entity[] toHit = GetToHit(toCheck);
        
        for (int i = 0; i < toHit.Length; i++) 
        {
            Entity e = toHit[i];
            e.TakeDamage(CalcValue(), moveEnemy);
            if (bleeds) e.GiveMod(Entity.Modifier.Bleed);
            if (stuns) e.GiveMod(Entity.Modifier.Stun);
            if (marks) e.GiveMod(Entity.Modifier.Marks);
        }
        if (dodges) BattleManager.CurrentPlayer.GiveMod(Entity.Modifier.Dodge);
        if (marksSelf) BattleManager.CurrentPlayer.GiveMod(Entity.Modifier.Marks);
        BattleManager.CurrentPlayer.Animate(type);
        BattleManager.CurrentPlayer.Move(movePlayer);
    }
}
