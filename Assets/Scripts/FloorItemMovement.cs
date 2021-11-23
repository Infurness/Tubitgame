using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloorItemMovement : MonoBehaviour
{
    private Camera mainCam;
    private bool pointerDown;
   [SerializeField] private Vector2 maxPos, minPos;
   [SerializeField] private float speed=1;
   private Vector3 mospos;
   void Start()
    {
        mainCam=Camera.main;
        //    maxPos = transform.TransformVector(maxPos);
       //  minPos = transform.TransformVector(minPos);
       mospos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (pointerDown)
        {
            var delta = Input.mousePosition - mospos;
            
        var mouseScreenPos =mospos;

        var newPos = transform.localPosition + (delta*speed*Time.deltaTime);
        transform.localPosition = new Vector3(Mathf.Clamp(newPos.x,minPos.x,maxPos.x),Mathf.Clamp(newPos.y,minPos.y,maxPos.y),newPos.z);
            
          
        }
        mospos = Input.mousePosition;

    }

    private void OnMouseDown()
    {
        pointerDown = true;
    }

    private void OnMouseUp()
    {
        pointerDown = false;
    }
}
