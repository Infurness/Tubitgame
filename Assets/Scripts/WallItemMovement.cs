using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class WallItemMovement : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    [Inject] private SignalBus signalBus;
    
    private bool pointerDown;
    [SerializeField] private float speed = 0.5f;
    private Vector3 mospos;
    [SerializeField] private bool editMode = false;
    [SerializeField] private LayerMask mask;
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
        //    maxPos = transform.TransformVector(maxPos);
       //  minPos = transform.TransformVector(minPos);
       mospos = Input.mousePosition;
   

       rigidbody2D = GetComponent<Rigidbody2D>();
       collider.isTrigger = true;
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
        collider.isTrigger = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
        rigidbody2D.velocity=Vector2.zero;
        collider.isTrigger = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        rigidbody2D.velocity=Vector2.zero;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer==LayerMask.NameToLayer("RightWall"))
        {
            var scale =Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(-scale, scale, scale);
        }else if (other.gameObject.layer==LayerMask.NameToLayer("LeftWall"))
        {
            var scale =Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
