using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TitleHUD : MonoBehaviour
{
    [SerializeField] private Button nextButton, selectButton;
    [SerializeField] private Button[] skinButtons;
    [SerializeField] private TMP_Text title;
    [SerializeField] private Transform[] showPos;
    
    private Party[] availableParties;
    private List<GameObject> models = new();
    private List<PaintButton> paintButtons = new();
    private Canvas canvas;
    
    private int currentParty = 0;
    private int currentPos = 0;
    public int[] skinIndex = new int [4];
    
    private void Start()
    {
        for(int i = 0; i < skinButtons.Length; i++)
        {
            Button b = skinButtons[i];
            PaintButton pB = b.AddComponent<PaintButton>();
            pB.Initiate(i, this);
            paintButtons.Add(pB);
        }

        List<Party> list = new ();
        foreach (Party party in PartyManager.Instance.parties)
        {
            if (party.unlocked) list.Add(party);
        }
        availableParties = list.ToArray();

        if (availableParties.Length == 0)
        {
            Debug.LogError("NO PARTIES IN THE MANAGER");
            return;
        }

        if (availableParties.Length == 1)
        {
            nextButton.interactable = false;
        }
        selectButton.onClick.AddListener(SelectParty);
        nextButton.onClick.AddListener(NextParty);
        ShowParty();
    }

    private void ShowParty()
    {
        for (int i = models.Count - 1; i >= 0; i--)
        {
            Destroy(models[i]);
        }
        models.Clear();

        List<Transform> pos = new();
        foreach (Transform child in showPos[currentPos].GetComponentInChildren<Transform>())
        {
            if (child.CompareTag("MainCamera"))
            {
                Camera.main.transform.position = child.position;
                Camera.main.transform.rotation = child.rotation;
            }
            else
            {
                pos.Add(child);
            }
        }

        Party toShow = availableParties[currentParty];
        for (int i = 0; i < toShow.heroes.Length; i++)
        {
            Hero hero = toShow.heroes[i];
            if (hero == null)
            {
                skinButtons[i].gameObject.SetActive(false);
                continue;
            }
            skinButtons[i].gameObject.SetActive(true);
            
            Hero showHero = Instantiate(hero, pos[i]);
            showHero.hpBar.gameObject.SetActive(false);
            showHero.transform.Rotate(new Vector3(0, -120, 0));
            models.Add(showHero.gameObject);

            Debug.Log($"pbs {paintButtons.Count}");
            if (showHero.CanSkinChange())
            {
                paintButtons[i].button.interactable = true;
                paintButtons[i].Assign(showHero);
            } else
                paintButtons[i].button.interactable = false;
            
        }
        title.text = toShow.name;
    }

    private void NextParty()
    {
        skinIndex = new int [4];
        currentPos++;
        if (currentPos > showPos.Length - 1) currentPos = 0;
        currentParty++;
        if (currentParty > availableParties.Length - 1) currentParty = 0;
        ShowParty();
    }

    private void SelectParty()
    {
        PartyManager.Instance.SetStartParty(availableParties[currentParty], skinIndex);
        SceneManager.LoadScene("TravelScene");
    }
}

public class PaintButton : MonoBehaviour
{
    private TitleHUD manager;
    public Button button;
    private Hero hero;
    private int index;
    public void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Initiate(int pIndex, TitleHUD pManager)
    {
        index = pIndex;
        manager = pManager;
    }

    public void Assign(Hero pHero)
    {
        hero = pHero;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonPress);
    }

    private void OnButtonPress()
    {
        manager.skinIndex[index] = hero.ChangeSkin();
    }
}
