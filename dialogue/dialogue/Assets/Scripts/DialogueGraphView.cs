
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;

public class DialogueGraphView : GraphView
{
    private readonly Vector2 defaultNodeSize = new Vector2(x: 150, y: 150);


    public DialogueGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(index: 0, grid);
        grid.StretchToParentSize();


        AddElement(GenerateEntryPointNode());
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach(
            funcCall: (port) => 
            {
                var portView = port;
                if(startPort != portView && startPort.node != portView.node)
                {
                    compatiblePorts.Add(port);
                }
            }
        );

        return compatiblePorts;
    }


    // capacity - connect 1 only, or connect several
    private Port GeneratePort(DialogueNode node, Direction ioDirection, Port.Capacity capacity=Port.Capacity.Single)
    {
        // final type, is the type of data that travels along the port
        return node.InstantiatePort(Orientation.Horizontal, ioDirection, capacity, typeof(float));
    }


    private DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode
        {
              title = "start"
            , GUID = GUID.Generate().ToString()
            , DialogueText = "enter here"
            , entryPoint = true
        };

        // Add "Next" port to entry node
        var port = GeneratePort(node, Direction.Output);
        port.portName = "Next";
        node.outputContainer.Add(port);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(10, 10, 100, 100));

        return node;
    }

    public void CreateNode(string name)
    {
        AddElement(CreateDialogueNode(name));
    }

    public DialogueNode CreateDialogueNode(string name)
    {
        var node = new DialogueNode
        {
            title = name
            , DialogueText = name
            , GUID = GUID.Generate().ToString()
        };

        var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
        inputPort.name = "Input";
        node.inputContainer.Add(inputPort);

        var button = new UnityEngine.UIElements.Button(clickEvent: () => { AddChoicePort(node);});
        button.text = "Add Choice";
        node.titleContainer.Add(button);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(position: Vector2.zero, size: defaultNodeSize));

        return node;
    }


    private void AddChoicePort(DialogueNode node)
    {
        var port = GeneratePort(node, Direction.Output, Port.Capacity.Single);

        var outputPortCount = node.outputContainer.Query(name: "connector").ToList().Count;
        port.portName = $"Choice: {outputPortCount}";

        node.outputContainer.Add(port);
        node.RefreshPorts();
        node.RefreshExpandedState();

    }

}
