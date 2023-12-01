using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

[CreateAssetMenu(fileName = "party", menuName = "ScriptableObjects/Party")]
public class Party : ScriptableObject
{
    public Hero[] heroes;
    public Car car;
    public bool unlocked;
    public int [] SkinIndexes = new int [4];
    [NonSerialized] public EventData EventData = null;

    private void OnValidate()
    {
        if (heroes.Length == 4) return;
        //Debug.LogWarning("PARTY HAS TO BE 4 SLOTS LONG!");
        Array.Resize(ref heroes, 4);
    }

    private void OnEnable()
    {
        EventBus<OnRewardCar>.Subscribe(SetCar);
    }

    private void OnDisable()
    {
        EventBus<OnRewardCar>.Unsubscribe(SetCar);
    }
    
    public void SwitchPos(int pos1, int pos2)
    {
        (heroes[pos1], heroes[pos2]) = (heroes[pos2], heroes[pos1]);
    }

    public void AddHero(Hero pHero)
    {

        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i] == null)
            {
                heroes[i] = pHero;
                SkinIndexes[i] = pHero.skinIndex;
                return;
            }
        }
        Debug.Log("WHOOPS, PARTY IS ALREADY FULL");
    }

    private void SetCar(OnRewardCar pEvent)
    {
        car = pEvent.Car;
        Debug.Log("YAY NEW CAR :3");
    }
}