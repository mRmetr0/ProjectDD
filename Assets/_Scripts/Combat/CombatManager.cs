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
    public enum Side
    {
        Heroes, 
        Enemies
    }
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
        for (int i = 0; i < PlayerPos.Length; i++)
        {
            GameObject pos = PlayerPos[i];

            CombatPosition comPos = pos.AddComponent<CombatPosition>();
            comPos.SetUp(this, i, Side.Heroes);

            CombatEntity entity = pos.GetComponentInChildren<CombatEntity>();
            if (entity is not null)
            {
                heroes.Add(entity);
                entity.Position = heroes.Count-1;
            }

        }

        for (int i = 0; i < EnemyPos.Length; i++)
        {
            GameObject pos = EnemyPos[i];

            CombatPosition comPos = pos.AddComponent<CombatPosition>();
            comPos.SetUp(this, i, Side.Enemies);
            CombatEntity entity = pos.GetComponentInChildren<CombatEntity>();
            if (entity is not null)
            {
                enemies.Add(entity);
                entity.Position = enemies.Count-1;
            }
        }
    }

    private void Start() {
        StartRound();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public List<CombatEntity> GetCombatSide(Side side)
    {
        return side == Side.Heroes ? heroes : enemies;
    }

    private void StartRound()
    {
        Debug.Log("New Round...");
        rounds++;
        CombatHUD.instance.UpdateRoundCount(rounds);
        allEntities = new();
        allEntities.AddRange(heroes);
        allEntities.AddRange(enemies);
        allEntities = allEntities.OrderBy(x => random.Next()).ToList();
        NextTurn(false);
    }

    public void NextTurn(bool removeLastPlayer = true)
    {
        Debug.Log("Next Turn...");
        if (removeLastPlayer)
            allEntities.RemoveAt(0);
        if (allEntities.Count == 0)
        {
            StartRound();
            return;
        }
        allEntities[0].StartTurn();
    }

    public void OnPosClick(Side side, int pos)
    {
        bool isValid = false;
        if (CombatHUD.instance.SelectedButton is null) return;
        Skill toUse = CombatHUD.instance.SelectedButton.Skill;
        switch (toUse.targets)
        {
            case Skill.Targets.opponent:
                if (side != Side.Enemies) return;
                if (!toUse.CanHitPos(pos)) return;
                toUse.UseSkill(allEntities[0], enemies[pos]);
                isValid = true;
                break;
            case Skill.Targets.team:
                if (side != Side.Heroes) return;
                if (!toUse.CanHitPos(pos)) return;
                toUse.UseSkill(allEntities[0], heroes[pos]);
                isValid = true;
                break;
            //TODO: ADD TARGET.SELF SKILLS
            case Skill.Targets.self:
                if (side != Side.Heroes) return;
                if (pos != allEntities[0].Position) return;
                toUse.UseSkill(allEntities[0], heroes[pos]);
                isValid = true;
                break;
        }
        if (isValid) allEntities[0].EndTurn();
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
        //Set positions of each entity
        int directionNormal = direction / Mathf.Abs(direction);
        for (int i = 0; i < Mathf.Abs(direction); i++)
        {
            if ((direction < 0 && entity.Position == 0) || (direction > 0 && entity.Position == team.Count - 1))
                break;
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

    public class CombatPosition : MonoBehaviour
    {
        private CombatManager manager;
        private int position;
        private Side side;
        public void SetUp(CombatManager _manager, int _position, Side _side)
        {
            manager = _manager;
            position = _position;
            side = _side;
        }

        private void OnMouseEnter() //TODO: Show Entity stats on hover
        {
            // Debug.Log("Mouse Entered");
        }

        private void OnMouseExit() //TODO: Hide Entity stats on hover
        {
            // Debug.Log("Mouse Exited");
        }

        private void OnMouseDown()
        {
            manager.OnPosClick(side, position);
        }
    }

}
