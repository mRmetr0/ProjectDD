using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventData : MonoBehaviour
{
    [SerializeField] private GameObject[] zombies;

    public MonoBehaviour[] rewards;
    public float appearanceOdds;
    [Range(0, 4)] public int infestation;


    public void Initialize()
    {
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].gameObject.SetActive(infestation>0 && i < infestation*2);
        }
    }

    public void GiveRewards()
    {
        foreach (MonoBehaviour obj in rewards)
        {
            switch (obj)
            {
                case (Car):
                    Debug.Log("JUP THATS A CAR :)");
                    EventBus<OnRewardCar>.Publish(new OnRewardCar(obj as Car));
                    break;
                case (Hero):
                    Debug.Log("THATS A STRAIGTUP HUMAN BEING");
                    TravelHUD.Instance.Party.AddHero(obj as Hero);
                    break;
            }
        }
        TravelHUD.Instance.MoveCamTravel();
    }
}
