using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableThemeElement : MonoBehaviour, IPointerDownHandler
{
    ThemeSelectionPopUp_VC visualController;
    int indexInVisualController = 0;

    public void OnPointerDown (PointerEventData eventData)
    {
        visualController.StartDraggingTheme (indexInVisualController);
    }

    public void SetVisualController(ThemeSelectionPopUp_VC vC, int draggableIndexInVC)
    {
        visualController = vC;
        indexInVisualController = draggableIndexInVC;
    }

    
}
