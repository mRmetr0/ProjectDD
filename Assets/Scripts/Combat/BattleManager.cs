using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public static Entity CurrentPlayer;
   [SerializeField] private Entity[] heroes;
   [SerializeField] private Entity[] enemies;
   
    private int _round;
    private int _turn;

    private List<Entity> _turnOrder;
    public Entity[] Heroes => heroes;
    public Entity[] Enemies => enemies;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("MORE THEN ONE BATTLEMANAGER!!!!");
            return;
        }
        
        Instance = this;
    }

    private void Start()
    {
        BattleStart();
    }

    private void BattleStart()
    {
        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i] == null) break;
            heroes[i].Position = i;
        }
        heroes = heroes.Where(c => c != null).ToArray();
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null) break;
            enemies[i].Position = i;
        }
        enemies = enemies.Where(c => c != null).ToArray();

        List<Entity> turnOrder = new List<Entity>();
        Entity[] allEntities = heroes.Union(enemies).ToArray();
        /**
        turnOrder.Add(allEntities[0]);
        for (int i = 1; i < allEntities.Length; i++)
        {
            for (int j = 0; j < turnOrder.Count(); j++)
            {
                if (allEntities[i].Speed >= turnOrder[j].Speed)
                {
                    turnOrder.Insert(j, allEntities[i]);
                    break;
                }
            }
            turnOrder.Add(allEntities[i]);
        }
        _turnOrder = turnOrder;
        /**/
        /**/
        Random r = new Random();
        _turnOrder = allEntities.OrderBy(x => r.Next()).ToList();
        /**/
        _turn = -1;
        NextTurn();
    }

    public void NextTurn()
    {
        _turn++;
        if (_turn > _turnOrder.Count - 1)
        {
            _round++;
            _turn = 0;
        }

        CurrentPlayer = _turnOrder[_turn];
        CurrentPlayer.TurnStart();
    }

    public void RemoveCharacter(Entity pEntity)
    {
        _turnOrder.Remove(pEntity);
    }
}
