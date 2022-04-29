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

    public Collider2D Collider => collider;

    [SerializeField] private Collider2D collider;
    [SerializeField] private Collider2D overlapCollider;
    public int StackOrder => stackOrder;

    [SerializeField] private int stackOrder;
   [SerializeField] private int stackCount;
    private void Awake()
   {
       signalBus.Subscribe<RoomZoomStateChangedSignal>(((signal) =>
       {
           editMode = !signal.ZoomIn;
       }));
   }

   void Start()
    {

       mospos = Input.mousePosition;
 
       rigidbody2D = GetComponent<Rigidbody2D>();
       if (!collider)
       {
           collider = GetComponent<Collider2D>();
       }

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

        if (overlapCollider.IsTouchingLayers(LayerMask.GetMask("Floor")))
        {
            print("Touching Something");
        }     
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
       
            print("Stackable");
        
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer > gameObject.layer)
        {
            print("Stackable");
        }
    }
    //
    // private void OnTriggerStay2D(Collider2D other)
    // {
    //     print("Floor Item Stacked");
    //
    // }
    //
    private void OnTriggerExit2D(Collider2D other)
    {
        // stackCount--;
        // if (stackCount==0)
        // {
        //     collider.isTrigger = false;
        // }

    }
    //
    // private void OnCollisionExit2D(Collision2D other)
    // {
    //     
    //   //  this.collider.isTrigger = false;
    //
    // }
}
