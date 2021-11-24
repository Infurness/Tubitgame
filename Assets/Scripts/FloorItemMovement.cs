using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class FloorItemMovement : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    [Inject] private SignalBus signalBus;
    private bool pointerDown;
    [SerializeField] private Vector2 maxPos, minPos;
   [SerializeField] private float speed=1;
   private Vector3 mospos;
   private bool editMode=false;

   private void Awake()
   {
       signalBus.Subscribe<RoomZoomStateChangedSignal>(((signal) =>
       {
           editMode = !signal.ZoomIn;
       }));
   }

   void Start()
    {
        //    maxPos = transform.TransformVector(maxPos);
       //  minPos = transform.TransformVector(minPos);
       mospos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (editMode)
        {
            if (pointerDown)
            {
                var delta = Input.mousePosition - mospos;

                var mouseScreenPos = mospos;

                var newPos = transform.localPosition + (delta * speed * Time.deltaTime);
                transform.localPosition = new Vector3(Mathf.Clamp(newPos.x, minPos.x, maxPos.x),
                    Mathf.Clamp(newPos.y, minPos.y, maxPos.y), newPos.z);


            }

            mospos = Input.mousePosition;
        }
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;

    }
}
