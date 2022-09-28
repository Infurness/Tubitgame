using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckIfIsAppleButton : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup buttonGridLayout;
    // Start is called before the first frame update
    void Start()
    {
    #if !UNITY_IOS 
        if(buttonGridLayout!=null)
        {
            buttonGridLayout.constraintCount -= 1;
        }
        Destroy(gameObject);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
