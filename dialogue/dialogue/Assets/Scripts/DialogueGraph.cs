using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "New Narrative";

    // menuItem sets it in the top level editor menu
    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent.text = "Dialogue Graph - paciFIST";
    }


    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }


    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView
        {
            name = "Ellie can do this"
        };

        _graphView.StretchToParentSize();        
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        //var fileNameTexField = new TextField(label: "File Name");
        //fileNameTexField.SetValueWithoutNotify(_fileName);
        //fileNameTexField.MarkDirtyRepaint();
        //fileNameTexField.RegisterValueChangedCallback(
        //    evt: ChangeEvent<string> => _fileName = evt.newValue
        //    );



        var nodeCreateButton = new Button(
            clickEvent: () => { _graphView.CreateNode("DialogueNode"); }
        );
        nodeCreateButton.text = "Create Node";

        toolbar.Add(nodeCreateButton);
        rootVisualElement.Add(toolbar);

    }

}
