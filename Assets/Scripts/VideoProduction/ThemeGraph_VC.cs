using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ThemeGraph_VC : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private ThemesManager themesManager;

    [SerializeField] private int refreshGraphSecondsRate = 300;

    [SerializeField] private Transform baseObject;
    [SerializeField] private GameObject graphLine;
    [SerializeField] private float enlargerValue;
    [Tooltip ("Variable used to make de animation curve line renderer fit the graph in scene")]
    [SerializeField] private float timeConversion;
    [Tooltip("Variable used to make de animation curve line renderer fit the graph in scene")]
    [SerializeField] private float valueConversion;

    Dictionary<LineRenderer, ThemeData> lineRenderers = new Dictionary<LineRenderer, ThemeData> ();
    // Start is called before the first frame update
    void Start()
    {
        foreach(ThemeData themedata in themesManager.GetThemesData ())
        {
            GameObject newGraphLine = Instantiate (graphLine, baseObject);
            newGraphLine.transform.localPosition = Vector3.zero;

            LineRenderer lr = newGraphLine.GetComponent<LineRenderer> ();
            lineRenderers.Add (lr, themedata);

            signalBus.Subscribe<SetLineColorInGraphSignal>((signal) => HighLightLineRenderer(signal.themeType, themedata.themeType, lr));
            signalBus.Subscribe<ResetLineColorInGraphSignal>(() => ResetLineColor(lr));
        }
        signalBus.Subscribe<OpenVideoCreationSignal> (StartGraph);
        signalBus.Subscribe<CloseVideoCreationSignal> (StopGraph);
    }

    void StartGraph ()
    {
        StartCoroutine( UpdateGraphPeriodically ());
    }

    void StopGraph ()
    {
        StopAllCoroutines ();
    }
    void UpdateLineRenderers ()
    {
        ThemeData[] themesData = themesManager.GetThemesData ();
        LineRenderer[] lines = new LineRenderer[lineRenderers.Keys.Count];
        lineRenderers.Keys.CopyTo(lines,0);

        for(int i=0; i< lineRenderers.Keys.Count;i++)
        {
            lineRenderers[lines[i]] = themesData[i];
        }
    }
    void UpdateGraph ()
    {
        signalBus.Fire<UpdateThemesGraphSignal>();
        UpdateLineRenderers ();
        foreach (KeyValuePair<LineRenderer,ThemeData> dictSlot in lineRenderers)
        {
            AnimationCurve themeCurve = themesManager.GetThemeAlgorithm (dictSlot.Value, GameClock.Instance.Now);
            dictSlot.Key.gameObject.GetComponent<VFX_GraphSelection>().SetAnimCurve(timeConversion, valueConversion, enlargerValue);
            //dictSlot.Key.startWidth = enlargerValue / 100;
            int hour = GameClock.Instance.Now.Hour % 6;
            float minutes = (float)GameClock.Instance.Now.Minute * 100f / 60f;
            float limitTimeValue = (hour + (minutes / 100)) / 6f;
            float alpha = dictSlot.Key.startColor.a;
            Color lrColor = dictSlot.Value.graphLineColor;
            lrColor.a = alpha;
            dictSlot.Key.startColor = lrColor;
            dictSlot.Key.endColor = lrColor;
            dictSlot.Key.positionCount = 0;
            int i = 0;
            foreach (Keyframe key in themeCurve.keys)
            {
                dictSlot.Key.positionCount += 1;
                Vector3 lrNewPos = dictSlot.Key.gameObject.transform.position;
                if (key.time < limitTimeValue)
                {
                    float timeInGraph = key.time / limitTimeValue;
                    lrNewPos += new Vector3 (timeInGraph * timeConversion, key.value * valueConversion, 0) * enlargerValue;
                    dictSlot.Key.SetPosition (i, lrNewPos);
                }
                else
                {
                    lrNewPos += new Vector3 (1 * timeConversion, themeCurve.Evaluate (limitTimeValue) * valueConversion, 0) * enlargerValue;
                    dictSlot.Key.SetPosition (i, lrNewPos);
                    break;
                }
                i++;
            }
            Vector3[] newPositions = new Vector3[dictSlot.Key.positionCount];
            dictSlot.Key.GetPositions(newPositions);
            foreach (LineRenderer lr in dictSlot.Key.gameObject.GetComponentsInChildren<LineRenderer>())
            {
                lr.positionCount = dictSlot.Key.positionCount;
                lr.SetPositions(newPositions);

                Color newColor = lrColor;
                newColor.a = lr.startColor.a;
                lr.startColor = newColor;
                lr.endColor = newColor;
            }
        }
    }

    IEnumerator UpdateGraphPeriodically ()
    {
        do
        {
            UpdateGraph ();
            yield return new WaitForSecondsRealtime (refreshGraphSecondsRate);//5 minutes
        } while (true);
    }

    void HighLightLineRenderer(ThemeType signalThemeType, ThemeType themeType, LineRenderer lr)
    {
        if (themeType == signalThemeType)
        {
            lr.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            lr.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            lr.gameObject.GetComponent<VFX_GraphSelection>().ThemeSelected();
        }
        else
        {
            lr.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            lr.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            lr.gameObject.GetComponent<VFX_GraphSelection>().ThemeUnselected();
        }
    }
    void ResetLineColor(LineRenderer lr)
    {
        lr.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        lr.gameObject.transform.GetChild(1).gameObject.SetActive(true);
        lr.gameObject.GetComponent<VFX_GraphSelection>().ThemeUnselected();
    }
}
