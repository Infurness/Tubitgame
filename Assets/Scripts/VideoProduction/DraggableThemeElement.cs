using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableThemeElement : MonoBehaviour, IPointerDownHandler
{
    ThemeSelectionPopUp_VC visualController;
    int indexInVisualController = 0;
    bool isEnabled;
    private void Start()
    {
        isEnabled = true;
    }
    public void OnPointerDown (PointerEventData eventData)
    {
        if (!isEnabled)
            return;
        visualController.StartDraggingTheme (indexInVisualController);
    }

    public void SetVisualController(ThemeSelectionPopUp_VC vC, int draggableIndexInVC)
    {
        visualController = vC;
        indexInVisualController = draggableIndexInVC;
    }

    public void DisableForTutorial()
    {
        isEnabled = false;
    }
}
