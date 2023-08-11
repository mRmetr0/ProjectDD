using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventData : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] rewards;
    [SerializeField] [Range(0, 4)] int infestation;
    public float appearanceOdds;
    [SerializeField] private GameObject[] zombies;
    

    public void Initialize()
    {
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].gameObject.SetActive(infestation>0 && i < infestation*2);
        }
    }

    public void AcceptEvent()
    {
        if (infestation == 0)
        {
            GiveReward();
            return;
        }
    }

    public void GiveReward()
    {
        foreach (MonoBehaviour obj in rewards)
        {
            if (obj is Car)
            {
                BackgroundManager.Instance.ReplaceCar(obj as Car);
            }
        }
        TravelHUD.Instance.MoveCamTravel();
    }
}
