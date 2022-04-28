using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_GraphSelection : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject mainLr;
    [SerializeField] GameObject particleTrail;
    float timeConversion;
    float valueConversion;
    float enlargerValue;

    public void SetAnimCurve(float time, float value, float enlarger)
    {
        timeConversion = time;
        valueConversion = value;
        enlargerValue = enlarger;
    }
    public void ThemeSelected()
    {
        particleTrail.SetActive(true);
        StartCoroutine(FollowTrail());
    }
    IEnumerator FollowTrail()
    {
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;
            if (time >= 1)
            {
                yield return new WaitForSeconds(0.7f);
                particleTrail.SetActive(false);
                yield return null;
                particleTrail.SetActive(true);
                time = 0;
            }
            particleTrail.transform.position = GetPositionInLineBytTime(time);
            yield return null;
        }
    }
    Vector3 GetPositionInLineBytTime(float time)
    {
        LineRenderer lr = mainLr.GetComponent<LineRenderer>();

        Vector3[] positions = new Vector3[lr.positionCount];
        lr.GetPositions(positions);
        Vector3 returnPos = lr.GetPosition(0);

        float totalDistance = 0;
        for(int i=0; i<positions.Length;i++) 
        {
            if(i>0)
            {
                totalDistance += Vector3.Distance(positions[i - 1], positions[i]);
            }   
        }
        float actualDistance = time * totalDistance;
        float tempDistance = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            if (i > 0)
            {
                tempDistance += Vector3.Distance(positions[i - 1], positions[i]);
            }
            if (tempDistance > actualDistance)
            {
                returnPos = positions[i] + (positions[i - 1] - positions[i]).normalized * (tempDistance - actualDistance);
                break;
            }
        }
        return returnPos;
    }
    public void ThemeUnselected()
    {
        StopAllCoroutines();
        particleTrail.SetActive(false);
    }
}
