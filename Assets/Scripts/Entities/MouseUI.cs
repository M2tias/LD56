using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MouseUI : MonoBehaviour
{
    [SerializeField]
    private MouseUIPanel mouseUIPanel;

    [SerializeField]
    private Canvas worldUICanvas;

    [SerializeField]
    private List<UISpriteItem> resourceUISprites = new List<UISpriteItem>();

    [SerializeField]
    private List<UISpriteItem> hpUISprites = new List<UISpriteItem>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        worldUICanvas.worldCamera = Camera.main;
    }

    public void Initialize(MouseUIPanel uiPanel)
    {
        mouseUIPanel = uiPanel;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateResource(int amount)
    {
        int clampedAmount = Math.Clamp(amount, 0, ResourceManager.instance.MaxResources());
        UISpriteItem item = resourceUISprites.FirstOrDefault(x => x.amount == clampedAmount);
        mouseUIPanel.resourceUI1.sprite = item.sprite1;
        mouseUIPanel.resourceUI2.sprite = item.sprite2;
    }

    public void UpdateHP(int amount)
    {
        int clampedAmount = Math.Clamp(amount, 10, 100);
        UISpriteItem item = hpUISprites.FirstOrDefault(x => x.amount == clampedAmount);
        mouseUIPanel.hpUI1.sprite = item.sprite1;
        mouseUIPanel.hpUI2.sprite = item.sprite2;
    }
}

[Serializable]
public class UISpriteItem
{
    public int amount;
    public Sprite sprite1;
    public Sprite sprite2;
}