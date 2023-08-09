using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image health, bar;
    [SerializeField] private Color basic, active;
    private Canvas _canvas;
    private float maxLength;
    private float currentLength;

    private void OnValidate()
    {
        bar.color = basic;
    }

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        Camera cam = Camera.main;
        transform.LookAt(transform.position + cam.transform.forward);
    }

    public void UpdateUI(int pHealth, int pMax)
    {
        if (pHealth <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        var localScale = health.transform.localScale;
        float newX = ((float)pHealth / pMax) * localScale.x;
        Vector3 newScale = new Vector3(newX, localScale.y,
            localScale.z);
        health.transform.localScale = newScale;
    }

    public void SetBarActive(bool pActive)
    {
        bar.color = pActive ? active : basic;
    }
}
