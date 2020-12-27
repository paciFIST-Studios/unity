using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using Button = UnityEngine.UIElements.Button;

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
        inputPort.portName = "Input";
        node.inputContainer.Add(inputPort);

        var button = new UnityEngine.UIElements.Button(clickEvent: () => { AddChoicePort(node);});
        button.text = "Add Choice";
        node.titleContainer.Add(button);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(position: Vector2.zero, size: defaultNodeSize));

        return node;
    }


    public void AddChoicePort(DialogueNode node, string overridePortName = "")
    {
        var port = GeneratePort(node, Direction.Output, Port.Capacity.Single);

        // this removes the visual label on the node's output ports.  However, when this is done,
        // the output ports stop being able to generate an edge.  they can still accept an edge, but
        // they don't make one when you try to use the mouse to pull one out
        //var oldLabel = port.contentContainer.Q<Label>(name: "type");
        //port.contentContainer.Remove(oldLabel);

        var outputPortCount = node.outputContainer.Query(name: "connector").ToList().Count;

        var portNameChoice = string.IsNullOrEmpty(overridePortName) ? $"idx: {outputPortCount}" : overridePortName;
        port.portName = portNameChoice;

        var textField = new TextField
        {
              name = string.Empty
            , value = portNameChoice
        };
        textField.RegisterValueChangedCallback((ChangeEvent<string> evt) => port.portName = evt.newValue);
        port.contentContainer.Add(new Label("  "));
        port.contentContainer.Add(textField);


        var removePortButton = new Button(clickEvent: () => RemovePort(node, port)) { text = "X"};
        port.contentContainer.Add(removePortButton);


        node.outputContainer.Add(port);
        node.RefreshPorts();
        node.RefreshExpandedState();

    }

    void RemovePort(DialogueNode node, Port port)
    {
        var targetEdge = edges.ToList().Where(
               (Edge x) => x.output.portName == port.portName
            && x.output.node == port.node
        );

        if (!targetEdge.Any()) { return; }

        var edge = targetEdge.First();
        edge.input.Disconnect(edge);
        RemoveElement(targetEdge.First());

        node.outputContainer.Remove(port);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }


}
