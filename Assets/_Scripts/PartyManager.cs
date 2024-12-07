using System;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance;
    public Party currentParty;
    public Party[] parties;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("ALREADY A PARTYMANAGER?");   
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void SetStartParty(Party pParty, int[] skinIndex)
    {
        //currentParty.heroes = pParty.heroes;
        System.Array.Copy(pParty.heroes, currentParty.heroes, 4);
        currentParty.car = pParty.car;
        currentParty.SkinIndexes = new int [4];
        for (int i = 0; i < currentParty.heroes.Length; i++)
        {
            currentParty.SkinIndexes[i] = skinIndex[i];
        }
    }
}


