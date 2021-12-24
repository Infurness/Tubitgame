using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class FloorItemMovement : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    [Inject] private SignalBus signalBus;
    private bool pointerDown;
    [SerializeField] private float speed = 0.5f;
    private Vector3 mospos;
    [SerializeField] private bool editMode = false;
    private Rigidbody2D rigidbody2D;
    [SerializeField] private Collider2D collider;
   private void Awake()
   {
       signalBus.Subscribe<RoomZoomStateChangedSignal>(((signal) =>
       {
           editMode = !signal.ZoomIn;
           print("Room Edit Mode ON");
       }));
   }



   
   void Start()
    {

       mospos = Input.mousePosition;
 
       rigidbody2D = GetComponent<Rigidbody2D>();
       // if (!collider)
       // {
       //     collider = GetComponent<Collider2D>();
       // }

       rigidbody2D.bodyType = RigidbodyType2D.Static;
      // collider.isTrigger = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        if (editMode)
        {
            if (pointerDown)
            {
                var delta = Input.mousePosition - mospos;


                rigidbody2D.AddForce(delta.normalized * speed, ForceMode2D.Force);
            }
        }
        mospos = Input.mousePosition;


                
            }

        




public void OnPointerDown(PointerEventData eventData)
    {
        mospos = Input.mousePosition;
        pointerDown = true;
     //   collider.isTrigger = false;
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
        rigidbody2D.velocity=Vector2.zero;
       // collider.isTrigger = true;
        rigidbody2D.bodyType = RigidbodyType2D.Static;

    }



    private void OnCollisionEnter2D(Collision2D other)
    {
        rigidbody2D.velocity=Vector2.zero;
        
    }
}
