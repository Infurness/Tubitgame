using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CarInventoryButton : MonoBehaviour
{
    [SerializeField] private Image carImage;
    [SerializeField] private TMP_Text carNameText;
    private Button button;


    public void SetButtonData(Sprite carSprite, string carName, UnityAction buttonAction)
    {
        carImage.sprite = carSprite;
        carNameText.text = carName;
        button.onClick.AddListener(buttonAction);
    }
    void Awake()
    {
        button = GetComponent<Button>();
    }

}
