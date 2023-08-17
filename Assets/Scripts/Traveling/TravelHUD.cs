using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Events;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField] private PartyManager party;
    [Header("Camera")]
    [SerializeField] private Transform travelCamPos;
    [SerializeField] private float camMoveTime;
    [SerializeField] private float camMoveMod;

    [Header("HUD")] 
    [SerializeField] private GameObject travelView;
    [SerializeField] private GameObject eventView;
    [SerializeField] private Slider infestationSlider;
    [SerializeField] private Button acceptButton, declineButton;
    [SerializeField] private Button[] playerDisplay;

    private int switchPos = -1;
    private Camera _cam;

    public PartyManager Party => party;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("MORE THEN 1 TRAVELHUD!?!?!??!?!?!?");
            return;
        }
        Instance = this;

        for (int i = 0; i < playerDisplay.Length; i++)
        {
            Button b = playerDisplay[i];
            HeroButton hb = b.gameObject.AddComponent<HeroButton>();
            hb.Initialize(i, b);
        }

        _cam = Camera.main;
        SwitchHud(View.Travel);
        acceptButton.onClick.AddListener(AcceptEvent);
        declineButton.onClick.AddListener(DeclineEvent);
    }

    private void Start()
    {
        if (party.EventData != null && party.EventData.rewards.Length > 0)
            party.EventData.GiveRewards();
        UpdateHeroDisplay();
    }

    private void OnEnable()
    {
        EventBus<OnShowEvent>.Subscribe(ShowEvent);
        if (party != null) BackgroundManager.Instance.ReplaceCar(party.car);
    }
    private void OnDisable()
    {
        EventBus<OnShowEvent>.Unsubscribe(ShowEvent);
    }

    private void ShowEvent(OnShowEvent pEvent)
    {
        SwitchHud(View.Null);
        infestationSlider.value = pEvent.Data.infestation;
        StartCoroutine(MoveCamEvent(pEvent.CamPos));
    }

    private void AcceptEvent()
    {
        int threat = BackgroundManager.EventTile.eventData.infestation;
        if (threat == 0)
        {
            BackgroundManager.EventTile.eventData.GiveRewards();
            BackgroundManager.EventTile = null;
        }
        else
        {
            SetFight();
        }
    }
    private void DeclineEvent()
    {
        MoveCamTravel();
        BackgroundManager.EventTile = null;
    }

    private void SetFight()
    {
        party.EventData = BackgroundManager.EventTile.eventData;
        SceneManager.LoadScene("BattleScene");
    }
    public void SwitchButton(int pPos)
    {
        if (switchPos < 0 || switchPos == pPos)
        {
            switchPos = pPos;
        }
        else
        {
            party.SwitchPos(pPos, switchPos);
            UpdateHeroDisplay();
            switchPos = -1;
        }
    }

    private void UpdateHeroDisplay()
    {
        for (int i = 0; i < playerDisplay.Length;i++)
        {
            if (party.heroes[i] == null)
            {
                playerDisplay[i].gameObject.SetActive(false);
                continue;
            }
            playerDisplay[i].gameObject.GetComponent<Image>().sprite = party.heroes[i].Portrait; 
        }
    }

    //These are more so tools for the methods above:
    public void SwitchHud(View view)
    {
        travelView.SetActive(view == View.Travel);
        eventView.SetActive(view == View.Event);
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

public class HeroButton : MonoBehaviour
{
    private int pos;
    public void Initialize(int pPos, Button pButton)
    {
        pos = pPos;
        pButton.onClick.AddListener(OnButtonpress);
    }

    private void OnButtonpress()
    {
        TravelHUD.Instance.SwitchButton(pos);
    }
}
