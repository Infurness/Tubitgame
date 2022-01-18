using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_EnergyCoinVC : MonoBehaviour
{
    private Vector3 originPos;
    private Vector3 targetPos;
    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void SetCoinMovementValues(Vector3 origin, Vector3 target)
    {
        originPos = origin;
        targetPos = target;

        originPos.x += Random.Range(-1f, 1f);
        originPos.y += Random.Range(-1f, 1f);
        GetComponent<RectTransform>().position = originPos;
        gameObject.SetActive(true);
    }

    public void MoveCoin()
    {
        StartCoroutine(MoveCoinSteps(originPos, targetPos));
    }
    IEnumerator MoveCoinSteps(Vector3 origin, Vector3 target)
    {   
        RectTransform rect = GetComponent<RectTransform>();
        float lerp = 0;
        float speed = UnityEngine.Random.Range(5f, 8f);
        while (lerp < 1)
        {
            lerp += Time.deltaTime * (1 + speed);
            rect.position = Vector3.Lerp(origin, target, lerp);
            yield return null;
        }
        gameObject.SetActive(false);
    }

}
