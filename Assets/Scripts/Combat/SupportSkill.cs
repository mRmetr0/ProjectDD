using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SupportSkill", menuName = "ScriptableObjects/Skills/SupportSkill")]
public class SupportSkill : Skill
{
    [Header("Support Effects")] 
    public bool cleanses;

    public override void Use()
    {
        ToCheck toCheck = BattleManager.Instance.Heroes.Contains(BattleManager.CurrentPlayer)
        ? ToCheck.Heroes
        : ToCheck.Enemies;
        
        Entity[] toHit = GetToHit(toCheck);
        
        for (int i = 0; i < toHit.Length; i++) 
        {
            Entity e = toHit[i];
            e.TakeHealing(CalcValue(), cleanses);
        }
        if (dodges) BattleManager.CurrentPlayer.GiveMod(Entity.Modifier.Dodge);
        if (marksSelf) BattleManager.CurrentPlayer.GiveMod(Entity.Modifier.Marks);
        BattleManager.CurrentPlayer.Animate(type);
        BattleManager.CurrentPlayer.Move(movePlayer);
    }
}
