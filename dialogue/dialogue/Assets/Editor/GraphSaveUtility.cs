using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;


    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();


    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string filename)
    {
        // return if there are no edges in the graph
        if (!Edges.Any()) { return; }

        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
        var connectedPorts = Edges.Where((Edge x) => x.input.node != null).ToArray();

        // collect edges
        for(int i= 0; i < connectedPorts.Length; i++)
        {
            var outputNode = (DialogueNode) connectedPorts[i].output.node;
            var inputNode  = (DialogueNode) connectedPorts[i].input.node;

            dialogueContainer.NodeLinks.Add(
                new NodeLinkData
                {
                      BaseNodeGUID = outputNode.GUID
                    , linkingPortName = connectedPorts[i].output.portName
                    , TargetNodeGUID = inputNode.GUID
                }
            );
        }

        // collect nodes
        foreach(var dialogueNode in Nodes.Where(node=>!node.entryPoint))
        {
            dialogueContainer.DialogueNodeData.Add(
                new DialogueNodeData
                {
                      GUID = dialogueNode.GUID
                    , DialogueText = dialogueNode.DialogueText
                    , Position = dialogueNode.GetPosition().position
                }
            );
        }

        // check for resources folder first
        if(!AssetDatabase.IsValidFolder(path: "Assets/Resources"))
        {
            AssetDatabase.CreateFolder(parentFolder: "Assets", newFolderName: "Resources");
        }

        AssetDatabase.CreateAsset(dialogueContainer, path: $"Assets/Resources/{filename}.asset");
        AssetDatabase.SaveAssets();
    }


    public void LoadGraph(string filename)
    {
        _containerCache = Resources.Load<DialogueContainer>(filename);
        if(_containerCache == null)
        {
            EditorUtility.DisplayDialog(
                  title: "File Not Found!"
                , message: "Target dialogue graph file does not exist!"
                , ok: "ok"
            );
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();

    }

    private void ClearGraph()
    {
        // set entry point first
        Nodes.Find(match: (DialogueNode x) => x.entryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGUID;

        // toss existing data
        foreach(var node in Nodes)
        {
            if (node.entryPoint) { continue; }

            // remove edges connected to this node
            Edges.Where(x => x.input.node == node).ToList()
                .ForEach(edge => _targetGraphView.RemoveElement(edge));

            // remove node
            _targetGraphView.RemoveElement(node);
        }
    }


    private void CreateNodes()
    {
        foreach(var nodeData in _containerCache.DialogueNodeData)
        {
            // create node from save file
            var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueText);
            tempNode.GUID = nodeData.GUID;
            _targetGraphView.Add(tempNode);

            // add ports
            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == nodeData.GUID).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.linkingPortName));
        }
    }


    private void ConnectNodes() { }
}
