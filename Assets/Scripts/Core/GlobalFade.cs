using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalFade : Manager<GlobalFade>
{
    public GameObject fadePrefab;
    public CanvasGroup group;
    public float speed;
    
    float current;
    float target;

    Coroutine currentRoutine;

    public static float Duration => 1 / Instance.speed;

    private void Start()
    {
        SetTarget(0);
    }

    public void SetTarget (float _target)
    {
        target = _target;
        if (target != current)
        {
            if (currentRoutine != null)
            {
                StopCoroutine(currentRoutine);
            }
            currentRoutine = StartCoroutine(FadeRoutine());
        }
    }

    public void Attach (Transform head)
    {
        var canvas = Instantiate(fadePrefab, head.transform);
        group = canvas.GetComponentInChildren<CanvasGroup>();
        current = group.alpha;
    }

    public void Blink (float duration, float peakWait = 0.2f)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(BlinkRoutine(duration, peakWait));
    }

    IEnumerator FadeRoutine ()
    {
        while (target != current)
        {
            current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
            group.alpha = current;
            yield return null;
        }

        current = target;
        group.alpha = current;
        currentRoutine = null;
        yield break;
    }

    IEnumerator BlinkRoutine (float duration, float peakWait = 0.2f)
    {
        for (float f = current; f < 1; f += Time.deltaTime / duration)
        {
            current = f;
            group.alpha = current;
            yield return null;
        }

        yield return new WaitForSeconds(peakWait);

        for (float f = current; f > 0; f -= Time.deltaTime / duration)
        {
            current = f;
            group.alpha = current;
            yield return null;
        }
        currentRoutine = null;
        yield break;
    }
}
