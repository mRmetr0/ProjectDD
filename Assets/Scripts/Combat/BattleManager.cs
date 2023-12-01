using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public static Entity CurrentPlayer;
    [SerializeField] private Entity commonEnemy;
    [SerializeField] private Entity[] heroes;
    [SerializeField] private Entity[] enemies;
   
    private int _round;
    private int _turn;

    private List<Entity> _turnOrder;
    public Entity[] Heroes => heroes;
    public Entity[] Enemies => enemies;
    
    [Header("IN-SCENE VARIABLES")]
    [SerializeField] private Transform[] heroPos;
    [SerializeField] private Transform[] enemyPos;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("MORE THEN ONE BATTLEMANAGER!!!!");
            return;
        }
        
        Instance = this;
        if (PartyManager.Instance.currentParty == null) return;
        
        for (int i = heroes.Length - 1; i >= 0; i--)
        {
            if (heroes[i] == null) continue;
            Destroy(heroes[i].gameObject);
        }
        for (int i = enemies.Length - 1; i >= 0; i--)
        {
            if (enemies[i] == null) continue;
            Destroy(enemies[i].gameObject);
        }
        
        heroes = new Entity[4];
        enemies = new Entity[PartyManager.Instance.currentParty.EventData.infestation];
        for (int i = 0; i < heroes.Length; i++)
        {
            if (PartyManager.Instance.currentParty.heroes[i] == null)
                heroes[i] = null;
            else
            {
                heroes[i] = Instantiate(PartyManager.Instance.currentParty.heroes[i], heroPos[i].position, Quaternion.identity);
                (heroes[i] as Hero).SetSkin(PartyManager.Instance.currentParty.SkinIndexes[i]);
            }

        }

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = Instantiate(commonEnemy, enemyPos[i].position, Quaternion.identity);
        }
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
        if (EndGame(_turnOrder)) return;
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

    public bool EndGame(List<Entity> pEntities)
    {
        bool enemyLost = true;
        bool heroLost = true;
        foreach (Entity e in pEntities)
        {
            if (e is Enemy) enemyLost = false;
            if (e is Hero) heroLost = false;
        }

        if (heroLost) HUDManager.Instance.SwitchUI(HUDManager.UIType.Lose);
        if (enemyLost) HUDManager.Instance.SwitchUI(HUDManager.UIType.Win);
        return heroLost || enemyLost;
    }
}
