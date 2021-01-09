using UnityEngine;
using UnityEngine.UI;

public class ResearchItemListButton : MonoBehaviour
{
    [SerializeField]
    private Text label;

    public void SetText(string text)
    {
        label.text = text;
    }

    public void OnClick()
    {
        print(string.Format($"Button: {0}", label.text));
    }

}
