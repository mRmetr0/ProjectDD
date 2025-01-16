using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;


public class CombatManager : MonoBehaviour
{
    static public CombatManager instance;
    private static Random random = new();

    //Game variables:
    private int rounds = 0;
    
    //Entitiy varables
    [SerializeField] private GameObject[] PlayerPos;
    [SerializeField] private GameObject[] EnemyPos;

    private List<CombatEntity> heroes = new ();
    private List<CombatEntity> enemies = new ();
    private List<CombatEntity> allEntities = new ();

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

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void StartRound()
    {
        rounds++;
        allEntities = heroes;
        allEntities.AddRange(enemies);
        allEntities = allEntities.OrderBy(x => random.Next()).ToList();
        NextTurn(false);
    }

    private void NextTurn(bool removeLastPlayer = true)
    {
        if (removeLastPlayer)
            allEntities.RemoveAt(0);
        allEntities[0].StartTurn();
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
        foreach (CombatEntity member in team)
        {
            member.transform.DOMove(teamPos[member.Position].transform.position, 0.7f);
        }   
    }

    public void MoveToPosition(CombatEntity entity, int newPos)
    {
        int direction = entity.Position - newPos;
        MovePerson(entity, direction);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (GameObject obj in PlayerPos)
        {
            Gizmos.DrawSphere(obj.transform.position, 1);
        }

        Gizmos.color = Color.red;
        foreach (GameObject obj in EnemyPos)
        {
            Gizmos.DrawSphere(obj.transform.position, 1);
        }
    }
}
