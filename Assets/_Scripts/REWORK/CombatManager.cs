using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    static public CombatManager instance;
    
    [SerializeField] private GameObject[] PlayerPos;
    [SerializeField] private GameObject[] EnemyPos;

    private List<CombatEntity> heroes = new ();
    private List<CombatEntity> enemies = new ();

    private void Awake()
    {
        if (instance is not null)
        {
            Debug.LogWarning("MORE THEN ONE COMBATMANAGER");
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        //TODO: Have entity data load in through overworld scene.
        foreach (GameObject pos in PlayerPos)
        {
            CombatEntity entity = pos.GetComponentInChildren<CombatEntity>();
            if (entity is not null)
            {
                heroes.Add(entity);
                entity.Position = heroes.Count-1;
            }

        }
        foreach (GameObject pos in EnemyPos)
        {
            CombatEntity entity = pos.GetComponentInChildren<CombatEntity>();
            if (entity is not null)
            {
                enemies.Add(entity);
                entity.Position = enemies.Count-1;
            }
        }
    }

    /// <summary>
    /// Moves a character into a certain direction
    /// </summary>
    /// <param name="entity">person to be moved</param>
    /// <param name="direction">positive is push, negative is pull.</param>
    public void MovePerson(CombatEntity entity, int direction)
    {
        List<CombatEntity> team = heroes.Contains(entity) ? heroes : enemies;
        GameObject[] teamPos = team == heroes ? PlayerPos : EnemyPos;
        if (team.Count == 1) return;
        // Debug.Log("CAN MOVE IN TEAM");
        //Set positions of each entity
        int directionNormal = direction / Mathf.Abs(direction);
        for (int i = 0; i < Mathf.Abs(direction); i++)
        {
            if ((direction < 0 && entity.Position == 0) || (direction > 0 && entity.Position == team.Count - 1))
                break;
            // Debug.Log("CAN MOVE IN POSITION");
            CombatEntity entity2 = team[entity.Position + directionNormal];
            if (entity2 is not null)
            {
                (entity.Position, entity2.Position) = (entity2.Position, entity.Position);
                team[entity.Position] = entity;
                team[entity2.Position] = entity2;
            }

        }
        //Move each entity
        //TODO: lerp movement
        foreach (CombatEntity member in team)
        {
            member.transform.DOMove(teamPos[member.Position].transform.position, 0.7f);
            // member.transform.position = teamPos[member.Position].transform.position;
        }   
    }
}
