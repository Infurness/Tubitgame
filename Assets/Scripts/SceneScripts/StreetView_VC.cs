using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetView_VC : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private Transform roomViewTransform;
    [SerializeField] private Transform houseViewTransform;
    [SerializeField] private Transform streetViewTransform;

    // Start is called before the first frame update
    void Start()
    {
        StartImage ();
    }
    void StartImage ()
    {
        ResizeForCameraWidth (houseViewTransform);
        MoveSpriteNextToOther (houseViewTransform, roomViewTransform);

        ResizeForCameraWidth (streetViewTransform);
        MoveSpriteNextToOther (streetViewTransform, houseViewTransform);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void ResizeForCameraHeight (Transform resizeableTransform)
    {
        SpriteRenderer sr = resizeableTransform.GetComponent<SpriteRenderer>();
        if (sr == null)
            return;

        // Set filterMode
        sr.sprite.texture.filterMode = FilterMode.Point;

        // Get stuff
        double height = sr.sprite.bounds.size.y;
        double worldScreenHeight = mainCamera.orthographicSize * 2.0;
        // Resize
        resizeableTransform.localScale = new Vector2 (1, 1) * (float)(worldScreenHeight / height);
    }
    void ResizeForCameraWidth (Transform resizeableTransform)
    {
        SpriteRenderer sr = resizeableTransform.GetComponent<SpriteRenderer> ();
        if (sr == null)
            return;

        // Set filterMode
        sr.sprite.texture.filterMode = FilterMode.Point;

        // Get stuff
        double width = sr.sprite.bounds.size.x;
        double worldScreenHeight = mainCamera.orthographicSize * 2.0;
        double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        // Resize
        resizeableTransform.localScale = new Vector2 (1, 1) * (float)(worldScreenWidth / width);
    }
    void MoveSpriteNextToOther(Transform transformToMove, Transform refTransform)
    {
        SpriteRenderer refSprite = refTransform.GetComponent<SpriteRenderer> ();
        SpriteRenderer spriteToMove = transformToMove.GetComponent<SpriteRenderer> ();

        transformToMove.position = new Vector3(refTransform.position.x, 0, refTransform.position.z);

        float refWidth = refSprite.sprite.bounds.size.x * refTransform.localScale.x;
        transformToMove.position += new Vector3 (refWidth / 2, 0, 0);

        float width = spriteToMove.sprite.bounds.size.x * transformToMove.localScale.x;
        transformToMove.position += new Vector3 (width / 2, 0, 0);
    }
}
