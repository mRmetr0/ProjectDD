using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TravelHUD : MonoBehaviour
{
    public static TravelHUD Instance;

    public enum View
    {
        Travel,
        Event,
        Null
    }

    [Header("Camera")]
    [SerializeField] private Transform travelCamPos;
    [SerializeField] private float camMoveTime;
    [SerializeField] private float camMoveMod;

    [Header("HUD")] 
    [SerializeField] private GameObject travelView;
    [SerializeField] private GameObject eventView;
    [SerializeField] private Button acceptButton, declineButton;
    
    private Camera _cam;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("MORE THEN 1 TRAVELHUD!?!?!??!?!?!?");
            return;
        }
        Instance = this;
        _cam = Camera.main;
        
        SwitchHud(View.Travel);
        acceptButton.onClick.AddListener(AcceptEvent);
        declineButton.onClick.AddListener(DeclineEvent);
    }

    public void SwitchHud(View view)
    {
        travelView.SetActive(view == View.Travel);
        eventView.SetActive(view == View.Event);
    }

    private void AcceptEvent()
    {
        BackgroundManager.EventTile.eventData.AcceptEvent();
        BackgroundManager.EventTile = null;
    }

    private void DeclineEvent()
    {
        MoveCamTravel();
        BackgroundManager.EventTile = null;
    }

    public void MoveCamTravel()
    {
        SwitchHud(View.Null);
        StartCoroutine(MoveCamEvent(travelCamPos));
    }
    public IEnumerator MoveCamEvent(Transform newPos)
    {
        float elapsedTime = 0, waitTime = camMoveTime;
        Transform oldPos = _cam.transform;
        while (elapsedTime < waitTime)
        {
            _cam.transform.position = Vector3.Lerp(oldPos.position, newPos.position, (camMoveMod * elapsedTime/waitTime));
            _cam.transform.rotation = Quaternion.Lerp(oldPos.rotation, newPos.rotation, (camMoveMod * elapsedTime/waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _cam.transform.position = newPos.position;
        _cam.transform.rotation = newPos.rotation;
        SwitchHud(newPos == travelCamPos ? View.Travel : View.Event);
        yield return null;
    }
}
