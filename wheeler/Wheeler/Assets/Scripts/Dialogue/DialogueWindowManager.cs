using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueWindowManager : MonoBehaviour
{
    [SerializeField] private RectTransform basePanel;
    [SerializeField] private RectTransform leftSpeakerImage;
    [SerializeField] private RectTransform leftSpeakerTextBox;
    [SerializeField] private RectTransform leftSpeakerNamePlate;
    [SerializeField] private RectTransform leftSpeakerName;
    [SerializeField] private RectTransform rightSpeakerImage;
    [SerializeField] private RectTransform rightSpeakerTextBox;
    [SerializeField] private RectTransform rightSpeakerNamePlate;
    [SerializeField] private RectTransform rightSpeakerName;

    private DialogueConversation currentConversation;

    private void ClearWindow()
    {
        leftSpeakerImage.GetComponent<Image>().sprite = null;
        leftSpeakerName.GetComponentInParent<TextMesh>().text = "";
        leftSpeakerTextBox.GetComponentInParent<TextMesh>().text = "";

        rightSpeakerImage.GetComponent<Image>().sprite = null;
        rightSpeakerName.GetComponentInParent<TextMesh>().text = "";
        rightSpeakerTextBox.GetComponentInParent<TextMesh>().text = "";
    }

    private void HideWindow()
    {
        basePanel.gameObject.SetActive(false);

        leftSpeakerImage.gameObject.SetActive(false);
        leftSpeakerTextBox.gameObject.SetActive(false);
        leftSpeakerNamePlate.gameObject.SetActive(false);
        leftSpeakerName.gameObject.SetActive(false);

        rightSpeakerImage.gameObject.SetActive(false);
        rightSpeakerTextBox.gameObject.SetActive(false);
        rightSpeakerNamePlate.gameObject.SetActive(false);
        rightSpeakerName.gameObject.SetActive(false);
    }

    private void SetLeftVisiblity(bool isVisisble)
    {
        basePanel.gameObject.SetActive(true);

        leftSpeakerImage.gameObject.SetActive(isVisisble);
        leftSpeakerTextBox.gameObject.SetActive(isVisisble);
        leftSpeakerNamePlate.gameObject.SetActive(isVisisble);
        leftSpeakerName.gameObject.SetActive(isVisisble);

        rightSpeakerImage.gameObject.SetActive(false);
        rightSpeakerTextBox.gameObject.SetActive(false);
        rightSpeakerNamePlate.gameObject.SetActive(false);
        rightSpeakerName.gameObject.SetActive(false);
    }

    private void SetRightVisiblity(bool isVisisble)
    {
        basePanel.gameObject.SetActive(isVisisble);

        leftSpeakerImage.gameObject.SetActive(false);
        leftSpeakerTextBox.gameObject.SetActive(false);
        leftSpeakerNamePlate.gameObject.SetActive(false);
        leftSpeakerName.gameObject.SetActive(false);

        rightSpeakerImage.gameObject.SetActive(isVisisble);
        rightSpeakerTextBox.gameObject.SetActive(isVisisble);
        rightSpeakerNamePlate.gameObject.SetActive(isVisisble);
        rightSpeakerName.gameObject.SetActive(isVisisble);
    }

    private void SetupStatement(DialogueStatement statement)
    {
        if(statement.isLeftSpeaker)
        {
            SetLeftVisiblity(true);

            leftSpeakerImage.GetComponent<Image>().sprite = statement.image;
            leftSpeakerName.GetComponentInParent<TextMesh>().text = statement.name;
            leftSpeakerTextBox.GetComponentInParent<TextMesh>().text = statement.text.value;
        }
        else
        {
            SetRightVisiblity(true);

            rightSpeakerImage.GetComponent<Image>().sprite = statement.image;
            rightSpeakerName.GetComponentInParent<TextMesh>().text = statement.name;
            rightSpeakerTextBox.GetComponentInParent<TextMesh>().text = statement.text.value;
        }
    }

    public void StartConversation(DialogueConversation conversation)
    {
        if(conversation.isFinished && !conversation.isRepeatable){ return; }
        currentConversation = conversation;

        ClearWindow();
        ShowNextStatement();
    }

    public void ShowNextStatement()
    {
        if(currentConversation.statements.Count > 0)
        {
            SetupStatement(currentConversation.statements.Dequeue());        
        }
    }

    public void EndConversation()
    {
        currentConversation.isFinished = true;
        HideWindow();
    }

}
