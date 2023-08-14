using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill")]
public class Skill : ScriptableObject
{
    public enum ToCheck
    {
        Heroes,
        Enemies
    }

    public enum Type
    {
        Melee,
        Ranged,
        Support
    }

    public enum Selection
    {
        Select,
        Cleave,
        Random
    }

    public Sprite sprite;
    public Vector2Int values;
    public bool[] positionsToUse = {false, false,false,false};
    public bool[] positionsToHit = {false, false,false,false};
    public bool usable;
    public Type type;
    public Selection selectType;
    public int movePlayer;
    public int moveEnemy;
    public bool bleeds;
    public bool stuns;
    public bool dodges;
    public string description;
    private System.Random rand = new ();

    private void OnValidate()
    {
        if (positionsToUse.Length != 4 || positionsToHit.Length != 4)
        {
            Debug.LogWarning("DONT CHANGE POSITIONS LENGTH");
            Array.Resize(ref positionsToUse, 4);
            Array.Resize(ref positionsToHit, 4);
        }

        if (values.x > values.y)
        {
            Debug.LogWarning("MAX DAMANGE HAS TO BE LARGER THEN MINIMUM DAMAGE");
            values.y = values.x + 1;
        }

        SetDescription();
    }
    
    private void SetDescription()
    {
        description = type.ToString();
        if (values.x == values.y) description += $", {values.x}\n";
        else description += $", {values.x}-{values.y}\n";
            string m = "Usable: ";
        for (int i = positionsToUse.Length-1; i >= 0; i--)
        {
            m += positionsToUse[i] ? "0 " : "- ";
        }
        m += "\nHit in: ";
        foreach (bool b in positionsToHit)
        {
            m += b ? "0 " : "- ";
        }
        description += m + "\n";

        if (movePlayer > 0) description += "Forward " + movePlayer + " ";
        if (movePlayer < 0) description += "Back " + Mathf.Abs(movePlayer)+ " ";
        if (moveEnemy > 0) description += "Pull " + moveEnemy+ " ";
        if (moveEnemy < 0) description += "Push " + Mathf.Abs(moveEnemy)+ " ";
        description += "\n";
        if (bleeds) description += "Bleeds ";
        if (stuns) description += "Stuns ";
    }
    
    public bool InPos(int posUser)
    {
        return positionsToUse[posUser];
    }

    public bool CanHit(ToCheck opponent)
    {
        Entity[] opponents =
            opponent == ToCheck.Heroes ? BattleManager.Instance.Heroes : BattleManager.Instance.Enemies;
        for (int i = 0; i < opponents.Length; i++)
        {
            if (positionsToHit[i])
                return true;
        }
        return false;
    }
    
    public void Use(ToCheck pOpponent)
    {
        Entity[] toHit = GetToHit(pOpponent);
        for (int i = 0; i < toHit.Length; i++) 
        {
            Entity e = toHit[i];
            e.TakeDamage(CalcDamage(), moveEnemy);
            if (bleeds) e.GiveMod(Entity.Modifier.Bleed);
            if (stuns) e.GiveMod(Entity.Modifier.Stun);
        }
        if (dodges) BattleManager.CurrentPlayer.GiveMod(Entity.Modifier.Dodge);
        BattleManager.CurrentPlayer.Animate(type);
        BattleManager.CurrentPlayer.Move(movePlayer);
    }

    private Entity[] GetToHit(ToCheck pOpponent)
    {
        List<Entity> toHit = new List<Entity>();
        Entity[] opponents = 
            pOpponent == ToCheck.Heroes ? BattleManager.Instance.Heroes : BattleManager.Instance.Enemies;
        switch (selectType)
        {
            case(Selection.Cleave):
                for (int i = 0; i < opponents.Length; i++)
                {
                    Entity opponent = opponents[i];
                    if (!positionsToHit[opponent.Position]) continue;
                    toHit.Add(opponent);
                }
                break;
            case (Selection.Select):
            case(Selection.Random):
                List<Entity> hitable = new List<Entity>();
                for (int i = 0; i < opponents.Length; i++)
                {
                    if (positionsToHit[opponents[i].Position])
                        hitable.Add(opponents[i]);
                }
                if (hitable.Count == 0) break;
                toHit.Add(hitable[rand.Next(0, hitable.Count)]);
                break;
        }
        return toHit.ToArray();
    }

    private int CalcDamage()
    {
        return rand.Next(values.x, values.y);
    }
}
