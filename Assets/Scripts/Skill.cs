using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill", order = 0)]
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
    public int value;
    public bool[] positionsToUse = {false, false,false,false};
    public bool[] positionsToHit = {false, false,false,false};
    public bool usable;
    public Type type;
    public Selection selectType;
    public int movePlayer;
    public int moveEnemy;
    public string description;

    private void OnValidate()
    {
        if (positionsToUse.Length != 4 || positionsToHit.Length != 4)
        {
            Debug.LogWarning("DONT CHANGE POSITIONS LENGTH");
            Array.Resize(ref positionsToUse, 4);
            Array.Resize(ref positionsToHit, 4);
        }
        SetDescription();
    }
    
    private void SetDescription()
    {
        description = type.ToString() + "\n";
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
        switch (selectType)
        {
            case (Selection.Select):
            case(Selection.Cleave):
                HitCleave(pOpponent);
                break;
            case(Selection.Random):
                HitRandom(pOpponent);
                break;
        }
        BattleManager.CurrentPlayer.Move(movePlayer);
    }

    private void HitSelect(ToCheck pOpponent)
    {
    }

    private void HitCleave(ToCheck pOpponent)
    {
        Entity[] opponents =
            pOpponent == ToCheck.Heroes ? BattleManager.Instance.Heroes : BattleManager.Instance.Enemies;
        for (int i = 0; i < opponents.Length; i++)
        {
            Entity opponent = opponents[i];
            if (!positionsToHit[opponent.Position]) continue;
            opponent.TakeDamage(value, moveEnemy);
            BattleManager.CurrentPlayer.Animate(type);
        }
    }

    private void HitRandom(ToCheck pOpponent)
    {
        Entity[] opponents =
            pOpponent == ToCheck.Heroes ? BattleManager.Instance.Heroes : BattleManager.Instance.Enemies;
        List<Entity> hitable = new List<Entity>();
        for (int i = 0; i < opponents.Length; i++)
        {
            if (positionsToHit[opponents[i].Position])
                hitable.Add(opponents[i]);
        }
        if (opponents.Length == 0) return;
        
        System.Random r = new System.Random();
        Entity opponent = hitable[r.Next(0, hitable.Count)];
        opponent.TakeDamage(value, moveEnemy);
        BattleManager.CurrentPlayer.Animate(type);
    }
}
