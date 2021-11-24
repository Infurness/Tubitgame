using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomInteractivityManager : MonoBehaviour
{
    [SerializeField] private Camera renderCamera;
    [SerializeField] private RawImage roomDisplayImage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetItemPositionInScreen (GameObject itemOnRenderCamera)
    {
        //Vector3 pos = renderCamera.WorldToViewportPoint (testItem.transform.position);
        Vector3 pos = roomDisplayImage.transform.position + (itemOnRenderCamera.transform.position - renderCamera.transform.position);
        pos.z = 0;
        return pos;
    }
}
