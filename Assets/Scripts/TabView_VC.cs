using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TabView_VC : MonoBehaviour
{
    [SerializeField] private GameObject tabButtonPrefab;
    public GameObject TabsButtonsPanel,buttonsView;
    private List<GameObject> tabsButtons;
    private void Awake()
    {
        tabsButtons = new List<GameObject>();

    }

  
    public void InitTabView(string[] tabsNames,Action<string>  filterAction)
    {
        foreach (var tabsButton in tabsButtons)
        {
            Destroy(tabsButton);
        }
        tabsButtons.Clear();

        for (int i = 0; i < tabsNames.Length; i++)
        {
            var filterName = tabsNames[i];
            var tb= Instantiate(tabButtonPrefab, TabsButtonsPanel.transform);
            tb.GetComponentInChildren<TMP_Text>().text =filterName;
            
            tb.GetComponent<Button>().onClick.AddListener(()=>filterAction(filterName));
            tabsButtons.Add(tb);
        }
    }

    public void SetButtonsTabVisibly(bool state)
    {
        buttonsView.gameObject.SetActive(state);
    }
    void Start()
    {
    }

    void Update()
    {
        
    }
}
