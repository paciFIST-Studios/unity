using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInTitleOnTimerController : MonoBehaviour
{
    // title text ref
    [SerializeField] private Text titleText;

    [SerializeField] private float minAlpha = 0.0f;
    [SerializeField] private float maxAlpha = 1.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float fadeSpeed = 0.1f;

    [SerializeField] private float startFadeAfterSeconds = 3f;

    private float awakeTime = 0.0f;
    private float fadeStartedAt = 0.0f;

    bool isPerformingFade = false;
    bool fadeIsDone = false;

    private bool checkIsFadeAllowed()
    {
        if(fadeIsDone) { return false; }

        if (awakeTime + startFadeAfterSeconds < Time.time)
        {
            return true;
        }
        return false;
    }

    private void Start()
    {
        awakeTime = Time.time;

        var color = titleText.color;
        color.a = minAlpha;
        titleText.color = color;
    }

    private void Update()
    {
        isPerformingFade = checkIsFadeAllowed();
        if(!isPerformingFade) { return; }

        var color = titleText.color;
        color.a += fadeSpeed;
        color.a = Mathf.Clamp(color.a, minAlpha, maxAlpha);
        titleText.color = color;

        if (color.a == maxAlpha)
        {
            print("fade is done");
            fadeIsDone = true;
        }
    }


}
