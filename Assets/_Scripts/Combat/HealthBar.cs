using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image health, bar, dot, dodge, stun, mark;
    [SerializeField] private Color basic, active;
    private float maxLength = 0.9f;

    private void OnValidate()
    {
        bar.color = basic;
    }

    private void Awake()
    {
        Camera cam = Camera.main;
        transform.LookAt(transform.position + cam.transform.forward);
    }

    public void UpdateUI(int pHealth, int pMax, bool pDot, bool pDodging, bool pStunned, bool pMarked)
    {
        if (pHealth <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
        
        dot.gameObject.SetActive(pDot);
        dodge.gameObject.SetActive(pDodging);
        stun.gameObject.SetActive(pStunned);
        mark.gameObject.SetActive(pMarked);
        
        var localScale = health.transform.localScale;
        float newX = ((float)pHealth / pMax) * maxLength;
        Vector3 newScale = new Vector3(newX, localScale.y,
            localScale.z);
        health.transform.localScale = newScale;
    }

    public void SetBarActive(bool pActive)
    {
        bar.color = pActive ? active : basic;
    }
}
