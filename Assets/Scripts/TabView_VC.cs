using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabView_VC : MonoBehaviour
{
    [SerializeField] private int numOfTabs=1;
    [SerializeField] private GameObject tabButtonPrefab,tabPrefab;
    [SerializeField] private GameObject buttonsPanel;
    private GameObject[] tabsButtons;
    private void Awake()
    {
        tabsButtons = new GameObject[numOfTabs];
        for (int i = 0; i < numOfTabs; i++)
        {
            tabsButtons[i] = Instantiate(tabButtonPrefab, buttonsPanel.transform);
        }
    }

    public void CreateTab(string tabName,Action<string> itemAction)
    {
        
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
