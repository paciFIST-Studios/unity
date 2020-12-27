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

        // File Name
        var fileNameTextField = new TextField(label: "File Name");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(
            // outer param: (lambda param) => { lambda code; }    
            callback: (ChangeEvent<string> evt) => { _fileName = evt.newValue; } 
        );
        toolbar.Add(fileNameTextField);

        toolbar.Add(child: new Button(clickEvent: () => RequestDataOperation(save: true))  { text = "Save" });
        toolbar.Add(child: new Button(clickEvent: () => RequestDataOperation(save: false)) { text = "Load" });


        // Create Node
        var nodeCreateButton = new Button(
            // outer param: (lambda) => { lambda code; }    
            clickEvent: () => { _graphView.CreateNode("DialogueNode"); }
        );
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);


        rootVisualElement.Add(toolbar);

    }

    private void RequestDataOperation(bool save)
    {
        if(string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog(
                  title: "Invalid file name"
                , message: $"Filename invalid: \"{ _fileName }\"\n\nEnter valid file name"
                , ok: "ok"
            );
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if(save)
        {
            saveUtility.SaveGraph(_fileName);
        }
        else
        {
            saveUtility.LoadGraph(_fileName);
        }
    }

}
