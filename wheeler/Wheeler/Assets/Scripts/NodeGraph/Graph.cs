using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Graph : ScriptableObject
{
    [SerializeField]
    private List<NodeBase> nodes;
    private List<NodeBase> Nodes
    {
        get
        {
            if (nodes == null)
            {
                nodes = new List<NodeBase>();
            }

            return nodes;
        }
    }

    public static Graph Create(string name)
    {
        var graph = CreateInstance<Graph>();

#if UNITY_EDITOR
        var path = $"Assets/Scripts/ScriptableObjects/NodeObjects/{name}.asset";
        AssetDatabase.CreateAsset(graph, path);
#endif

        return graph;
    }

    public void AddNode(NodeBase node)
    {
        Nodes.Add(node);
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
    }

}
