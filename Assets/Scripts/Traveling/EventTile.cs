using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventTile : RoadTile
{
    [SerializeField] private Transform camPos;
    [SerializeField] private EventData[] events;

    public Transform CamPos => camPos;
    public EventData eventData;

    private void Start()
    {
        eventData = GetEvent();
        eventData.Initialize();
        Instantiate(eventData, transform);
    }

    private EventData GetEvent()
    {
        float allOdds = 0;
        foreach (EventData data in events)
        {
            allOdds += data.appearanceOdds;
        }

        float n = Random.RandomRange(0, allOdds);
        foreach (EventData data in events)
        {
            if (data.appearanceOdds > n)
                return data;
            n -= data.appearanceOdds;
        }
        
        return events[events.Length-1];
    }
}
