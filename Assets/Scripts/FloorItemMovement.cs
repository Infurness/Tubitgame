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
    [SerializeField] private float speed = 0.5f;
    private Vector3 mospos;
    [SerializeField] private bool editMode = false;
    [SerializeField] private Collider2D collider;
    [SerializeField] private LayerMask mask;
    [SerializeField] private List<Transform> endPoints;
    
   private void Awake()
   {
       signalBus.Subscribe<RoomZoomStateChangedSignal>(((signal) =>
       {
           editMode = !signal.ZoomIn;
           print("Room Edit Mode ON");
       }));
    //   mask = LayerMask.GetMask("Floor");
   }

   void Start()
    {
        //    maxPos = transform.TransformVector(maxPos);
       //  minPos = transform.TransformVector(minPos);
       mospos = Input.mousePosition;
       collider = GetComponent<Collider2D>();
       endPoints = new List<Transform>();
       for (int i = 0; i < transform.childCount; i++)
       {
           endPoints.Add(transform.GetChild(i));
       }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
       
        if (editMode)
        {
            if (pointerDown)
            {
                var delta = Input.mousePosition - mospos;
                   
                    var newPos = transform.localPosition + (delta * speed * Time.deltaTime);
                    var normalizedDelta = (newPos-transform.localPosition).normalized;
                    Debug.DrawRay(transform.position,normalizedDelta,Color.red);
                    
                    foreach (var endPoint in endPoints)
                    {
                        if (Vector3.Angle(endPoint.position.normalized,normalizedDelta)<45)
                        {
                            if (Physics2D.Linecast(transform.position, endPoint.position, mask))
                            {
                                return;
                            }
                       

                        }

                    }
                    transform.localPosition = newPos;

                    

                    

                   


                
            }

        }
        mospos = Input.mousePosition;

    }



    public void OnPointerDown(PointerEventData eventData)
    {
        mospos = Input.mousePosition;
        pointerDown = true;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;

    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.DrawLine(transform.position,transform.GetChild(i).position);

        }
       

    }
}
