using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ThemeGraph_VC : MonoBehaviour
{
    [Inject] private ThemesManager themesManager;

    [SerializeField] private Transform baseObject;
    [SerializeField] private GameObject graphLine;
    [SerializeField] private float enlargerValue;

    // Start is called before the first frame update
    void Start()
    {
        foreach(AnimationCurve curve in themesManager.GetThemesPopuarityData ())
        {
            GameObject newGraphLine = Instantiate (graphLine, baseObject);
            newGraphLine.transform.localPosition = Vector3.zero;
            LineRenderer lr = newGraphLine.GetComponent<LineRenderer> ();
            lr.startWidth = enlargerValue / 100;
            lr.positionCount = curve.keys.Length;
            Color rndColor = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), 1);
            lr.startColor = rndColor;
            lr.endColor = rndColor;
            int i = 0;
            foreach(Keyframe key in curve.keys)
            {
                Vector3 lrNewPos = newGraphLine.transform.position;
                lrNewPos += new Vector3 (key.time, key.value, 0)* enlargerValue;
                lr.SetPosition (i, lrNewPos);
                i++;
            }
        }
    }
}
