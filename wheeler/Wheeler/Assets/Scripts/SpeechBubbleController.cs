using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubbleController : MonoBehaviour
{
    [SerializeField] private Text textBox;
    [SerializeField] private Transform[] speechBubbleParts;

    List<string> speechText = new List<string>
    {
          "I loft a toof yesterday"
        , "It was in a fight"
        , "I tried to rob the tooth fairy"
        , "The word \"fairy\", in faerie, means \"breaker\""
        , "The tooth breaker is the real title of the toof fairy"
        , "Fo that's how I lost my toof"
    };

    private void Start()
    {
        HideSpeechBubble();
        Play();
    }

    public void Play()
    {
        StartCoroutine(SpeechCoroutine());
    }

    IEnumerator SpeechCoroutine()
    {
        for(int i = 0; i < speechText.Count; i++)
        {
            print("Coroutine: " + i);
            yield return new WaitForSeconds(1f);
            ShowSpeechBubble();
            textBox.text = speechText[i];
            yield return new WaitForSeconds(5f);
            HideSpeechBubble();
        }
    }


    private void HideSpeechBubble()
    {
        for(int i = 0; i < speechBubbleParts.Length; i++)
        {
            speechBubbleParts[i].gameObject.SetActive(false);
        }
    }

    private void ShowSpeechBubble()
    {
        for(int i = 0; i < speechBubbleParts.Length; i++)
        {
            speechBubbleParts[i].gameObject.SetActive(true);
        }
    }


}
