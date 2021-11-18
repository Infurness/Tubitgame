using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterCustomizationsSlot : MonoBehaviour
{
    [SerializeField] private Image rarenessImage;
    [SerializeField] private Image iconImage;
    private Button slotButton;

    private void Awake()
    {
        slotButton = GetComponent<Button>();
    }

    public void SetRarenessSprite(Sprite rarenessSprite)
    {
        rarenessImage.sprite = rarenessSprite;
        
    }
    public void SetIconSprite(Sprite iconSprite)
    {
        iconImage.sprite = iconSprite;
        
    }

    public void SetButtonAction(UnityAction buttonAction)
    {
        slotButton.onClick.AddListener(buttonAction);
    }
}
